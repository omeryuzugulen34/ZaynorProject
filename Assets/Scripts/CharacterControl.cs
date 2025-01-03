using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    public CharacterSettingSO settings;

    private TargetingSystem targetingSystem;

    private bool canRoll = true;
    private bool canAttack = true;
    private bool isAttacking = false;

    private Vector2 movementInput;
    private Rigidbody rb;
    private PlayerControl inputActions;
    private Animator animator;

    int isMovingHash = Animator.StringToHash("isMoving");
    int isRollingHash = Animator.StringToHash("Roll");
    int isLightAttackHash = Animator.StringToHash("LightAttack");
    int isHeavyAttackHash = Animator.StringToHash("HeavyAttack");

    public enum AttackType { Light, Heavy }

    

    void Awake()
    {
        inputActions = new PlayerControl();
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        inputActions.Player.Roll.performed += ctx => StartRoll();

        inputActions.Player.LightAttack.performed += ctx => PerformAttack(AttackType.Light);
        inputActions.Player.HeavyAttack.performed += ctx => PerformAttack(AttackType.Heavy);

        rb = GetComponent<Rigidbody>();
        animator = gameObject.transform.GetChild(0).GetComponent<Animator>();

        targetingSystem = GetComponent<TargetingSystem>();

    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        rb.MovePosition(rb.position + moveDirection * settings.moveSpeed * Time.fixedDeltaTime);

        if (moveDirection != Vector3.zero && !isAttacking)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, settings.rotationSpeed * Time.fixedDeltaTime);

            animator.SetBool(isMovingHash, true);
        }
        else
        {
            animator.SetBool(isMovingHash, false);
        }
    }

    private void StartRoll()
    {
        if (canRoll && movementInput != Vector2.zero)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private void PerformAttack(AttackType attackType)
    {
        if (!canAttack) return;

        isAttacking = true;

        if (targetingSystem.currentTarget != null)
        {
            RotateTowardsTarget(targetingSystem.currentTarget);
        }
        else
        {
            RotateTowardsMouse();
        }
        float damage = attackType == AttackType.Light ? settings.lightAttackDamage : settings.heavyAttackDamage;
        int attackHash = attackType == AttackType.Light ? isLightAttackHash : isHeavyAttackHash;

        animator.SetTrigger(attackHash);
        StartCoroutine(AttackCooldown());
        StartCoroutine(EndAttackAfterAnimation());

        if (targetingSystem.currentTarget != null)
        {
            EnemyAttack enemy = targetingSystem.currentTarget.GetComponent<EnemyAttack>();
            if (enemy != null)
            {
                Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage, knockbackDirection);
                Debug.Log($"Player attacked {enemy.name}, dealing {damage} damage.");
            }
            else
            {
                Debug.LogWarning("Target does not have an EnemyAttack component.");
            }
        }
        else
        {
            Debug.LogWarning("No target in range for attack.");
        }
    }

    private void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;

        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, settings.rotationSpeed * Time.deltaTime);
        }
    }

    private void RotateTowardsMouse()
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);

            Vector3 direction = new Vector3(mouseWorldPosition.x - transform.position.x, 0f, mouseWorldPosition.z - transform.position.z).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f);
            }

            Debug.DrawLine(transform.position, mouseWorldPosition, Color.green);
            Debug.Log($"Mouse Position: {mouseWorldPosition}, Direction: {direction}, Target Rotation: {Quaternion.LookRotation(direction)}");
        }
        else
        {
            Debug.LogWarning("Mouse raycast did not hit the ground plane.");
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(settings.attackCooldown);
        canAttack = true;
    }


    private IEnumerator RollCoroutine()
    {
        canRoll = false;

        Vector3 rollDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        animator.SetTrigger(isRollingHash);

        float timer = 0f;
        while (timer < settings.rollDuration)
        {
            rb.MovePosition(rb.position + rollDirection * settings.rollSpeed * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return null;
        }


        yield return new WaitForSeconds(settings.rollCooldown);
        canRoll = true;
    }

    private IEnumerator EndAttackAfterAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(stateInfo.length);

        isAttacking = false;
    }
}