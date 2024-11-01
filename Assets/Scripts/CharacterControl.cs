using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    public float moveSpeed = 5f;  
    public float rotationSpeed = 720f;  

    public float rollSpeed = 12f;              
    public float rollDuration = 0.5f;          
    public float rollCooldown = 1f;   
    private bool isRolling = false;            
    private bool canRoll = true;  

    private Vector2 movementInput;  
    private Rigidbody rb;  
    private PlayerControl inputActions;  
    private Animator animator;

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
        
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        
            animator.SetBool("isMoving", true);
        
        }
        else{
            animator.SetBool("isMoving", false);
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
        isRolling = true;
        canRoll = false;

        Vector3 rollDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        
        
        animator.SetTrigger("Roll");


       
        float timer = 0f;
        while (timer < rollDuration)
        {
            rb.MovePosition(rb.position + rollDirection * rollSpeed * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return null;
        }

        isRolling = false;

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }
}