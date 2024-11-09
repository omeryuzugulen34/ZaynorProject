using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    public CharacterSettingSO settings;     //

    private bool canRoll = true;  

    private Vector2 movementInput;  
    private Rigidbody rb;  
    private PlayerControl inputActions;  
    private Animator animator;

    int isMovingHash = Animator.StringToHash("isMoving");
    int isRollingHash = Animator.StringToHash("Roll");

    void Awake()
    {
        inputActions = new PlayerControl();
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        inputActions.Player.Roll.performed += ctx => StartRoll();

        rb = GetComponent<Rigidbody>();  
        animator = gameObject.transform.GetChild(0).GetComponent<Animator>();

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
            animator.SetBool(isMovingHash, false); //
        }
    }

    private void StartRoll()
    {
        if (canRoll && movementInput != Vector2.zero)  
        {
            StartCoroutine(RollCoroutine());
        }
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