using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    public CharacterSettingSO settings;     

    private TargetingSystem targetingSystem;

    private bool canRoll = true;  
    private bool canAttack = true;

    private Vector2 movementInput;  
    private Rigidbody rb;  
    private PlayerControl inputActions;  
    private Animator animator;

    int isMovingHash = Animator.StringToHash("isMoving");
    int isRollingHash = Animator.StringToHash("Roll");
    int isLightAttackHash = Animator.StringToHash("LightAttack");
    int isHeavyAttackHash = Animator.StringToHash("HeavyAttack");


    void Awake()
    {
        inputActions = new PlayerControl();
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        inputActions.Player.Roll.performed += ctx => StartRoll();
        
        inputActions.Player.LightAttack.performed += ctx => PerformAttack("Light");
        inputActions.Player.HeavyAttack.performed += ctx => PerformAttack("Heavy");
        
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

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, settings.rotationSpeed * Time.fixedDeltaTime);
        
            animator.SetBool(isMovingHash, true); 
        }
        else{
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
     
    void PerformAttack(string attackType){
       // if(!canAttack) return;

        float damage = attackType == "Light" ? settings.lightAttackDamage : settings.heavyAttackDamage;
        int attackHash = attackType == "Light" ? isLightAttackHash : isHeavyAttackHash;

        animator.SetTrigger(attackHash);
       // StartCoroutine(AttackCooldown());

        if (targetingSystem.currentTarget != null)
    {
        Health targetHealth = targetingSystem.currentTarget.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            
            Debug.Log($"{attackType} Attack dealt {damage} damage to {targetingSystem.currentTarget.name}");
        }
    }
    else
    {
        Debug.Log("No target to attack.");
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
}