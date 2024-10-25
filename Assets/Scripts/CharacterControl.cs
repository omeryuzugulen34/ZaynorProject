using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public float moveSpeed = 5f;  
    public float rotationSpeed = 720f;  
    private Vector2 movementInput;  
    private Rigidbody rb;  
    private PlayerControl inputActions;  

    void Awake()
    {
        // Initialize the new input system actions
        inputActions = new PlayerControl();
        
        // Subscribe to the "Move" action
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        rb = GetComponent<Rigidbody>();  // Get the Rigidbody component (3D)
    }

    void OnEnable()
    {
        // Enable the input actions
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        // Disable the input actions
        inputActions.Player.Disable();
    }

    void FixedUpdate()
    {
        // Convert 2D input (X, Y) into 3D movement (X, Z)
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        
        // Move the player in the direction of input
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        // Rotate the player to face the movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}