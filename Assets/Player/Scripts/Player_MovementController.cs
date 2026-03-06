using UnityEngine;

// Movement state
public enum MovementState { JOGGING, CROUCHING, SWIMMING }

[RequireComponent(typeof(Rigidbody))]
public class Player_MovementController : MonoBehaviour
{
    // General variables
    [HideInInspector] public bool inControl = true;
    // Movement variables
    private Vector3 movementInput;
    private MovementState movementState = MovementState.JOGGING;
    // Jogging variables
    [SerializeField] private float jogSpeed;
    [SerializeField] private float jogSprintSpeed;
    private bool isSprinting = false;
    // Crouching variables
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchSprintSpeed;

    // Jumping variables
    [SerializeField] private float jumpForce;

    // Physics variables
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float stepHeight;
    private Rigidbody rb;

    // Collision detection variables
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Transform groundCheckTransform;
    private bool isGrounded;

    // Input variables
    private Player_InputActions input;

    //== On Start
    private void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody>();

        // Get input actions
        input = InputHandler.instance.input;

        //equip weapon
        //EquipWeapon(1);
    }

    //== On Update
    private void Update()
    {
        // If the player is not in control, return
        if (!inControl) return;

        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);
        // Set rigidbody damping to stop movement when not touching anything
        if (isGrounded)
            rb.linearDamping = 10;
        else if (!IsOnSlope())
            rb.linearDamping = 0;
        else if (hasJumped)
            rb.linearDamping = 0;

        // Handle movement
        HandleMovement();
        // Handle sprinting
        HandleSprinting();

        // Handle jumping
        HandleJumping();

        //handle weapon
        SwitchWeapon();

    }
    //== On Fixed Update
    private void FixedUpdate()
    {
        // Set default max speed
        float maxSpeed = movementState switch
        {
            // Jogging
            MovementState.JOGGING => isSprinting ? jogSprintSpeed : jogSpeed,
            // Crouching
            MovementState.CROUCHING => isSprinting ? crouchSprintSpeed : crouchSpeed,
            // Default
            _ => jogSpeed
        };

        // Get movement velocity
        Vector3 movementVelocity = movementInput * maxSpeed;
        // Get ground normal
        Vector3 groundNormal = GetGroundNormal();

        // Add movement velocity
        if (IsOnSlope() && !hasJumped)
            rb.AddForce(Vector3.ProjectOnPlane(movementVelocity, groundNormal), ForceMode.VelocityChange);
        else
            rb.AddForce(movementVelocity, ForceMode.VelocityChange);

        // If player is on a slope, disable gravity
        if (isGrounded && !hasJumped)
            rb.useGravity = !IsOnSlope();
        else
            rb.useGravity = true;

        // Get current horizontal velocity
        Vector3 horizontalVelocity = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // If the horizontal velocity is greater than the max speed, limit velocity
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            // Set max velocity
            Vector3 maxVelocity = horizontalVelocity.normalized * maxSpeed;

            // If the player is on a slope, limit total velocity to max speed
            if (isGrounded && IsOnSlope() && !hasJumped)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            // Else, limit horizontal velocity to max speed
            else
                rb.linearVelocity = new Vector3(maxVelocity.x, rb.linearVelocity.y, maxVelocity.z);
        }

        // If the player is at the top of a ramp in the air, apply downward force
        if (!isGrounded && rb.linearVelocity.y > 0 && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f, groundLayer) && !hasJumped)
            rb.AddForce(Vector3.down * 80, ForceMode.Force);

        // If player is moving and there is an object infront, check for stairs
        if (movementInput.magnitude > 0 && Physics.Raycast(transform.position, movementInput, out RaycastHit lowerHit, 0.35f, groundLayer))
        {
            // If the object is a stair, move the player up
            if (Physics.Raycast(lowerHit.point + (Vector3.up * stepHeight) + (movementInput * 0.1f), Vector3.down, out RaycastHit upperHit, stepHeight, groundLayer))
            {
                // Prevent detecting ramps
                if (Vector3.Angle(upperHit.normal, Vector3.up) == 0)
                    rb.MovePosition(transform.position + Vector3.up * (upperHit.point.y - lowerHit.point.y));
            }
        }
    }

    //== Handle Movement
    private void HandleMovement()
    {
        // Get move input
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();
        // Transform movement input into world space direction
        movementInput = transform.TransformDirection(new(moveInput.x, 0, moveInput.y)).normalized;
    }
    //== Handle Sprinting
    private void HandleSprinting()
    {   
        // If the player is not sprinting, set sprinting data to false
        if (!input.Player.Sprint.IsPressed() || !isGrounded || rb.linearVelocity.magnitude < 0.1f)
        {

            // Stop sprinting
            isSprinting = false;

            // Return
            return;
        }

        // Start sprinting
        isSprinting = true;
    }

    //Jump handling variables
    private bool hasJumped;
    private float jumpDelay;
    //== Handle Jumping
    private void HandleJumping()
    {
        // If player has jumped & is grounded, apply jump force
        if (isGrounded && input.Player.Jump.WasPressedThisFrame())
        {
            // Apply jump force
            rb.AddForce(new(0, jumpForce, 0), ForceMode.Impulse);

            // Set has jumped to true
            hasJumped = true;

            // Enable gravity
            rb.useGravity = true;

        }

        // If has jumped is true, increase jump delay
        if (hasJumped)
        {
            // Increase jump delay over time
            jumpDelay += Time.deltaTime;
            // If the jump delay has reached the end, reset jump data
            if (jumpDelay >= 0.2)
            {
                // Reset jump data
                hasJumped = false;
                jumpDelay = 0;
            }
        }
    }

    //== Get Ground Normal
    private Vector3 GetGroundNormal()
    {
        // If ground is detected, return ground normal
        if (Physics.Raycast(groundCheckTransform.position, Vector3.down, out RaycastHit hit, groundCheckRadius * 2, groundLayer))
            return hit.normal;

        // Return default normal
        return Vector3.up;
    }
    //== Is On Slope
    private bool IsOnSlope()
    {
        // If ground is below player, check if angled
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckRadius * 2, groundLayer))
        {
            // Get ground angle
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        // Return default
        return false;
    }


    //---------------------------------- Weapon Implementation -------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------

    public GameObject weapon1;
    public GameObject weapon2;
    private int currentWep = 1;


    //weapon switch handler
    public void SwitchWeapon()
    {
        //if more weapons are to be added this will need to be changed
        if (input.Player.Weapon1.WasPressedThisFrame())
        {
            EquipWeapon(1);
        }
        else if (input.Player.Weapon2.WasPressedThisFrame())
        {
            EquipWeapon(2);
        }
    }

    void EquipWeapon(int weaponNum)
    {
        currentWep = weaponNum;

        weapon1.SetActive(weaponNum == 1);
        weapon2.SetActive(weaponNum == 2);

        currentWeaponScript = (weaponNum == 1)
        ? weapon1.GetComponent<WeaponBase>()
        : weapon2.GetComponent<WeaponBase>();



        Debug.Log("Equipped Weapon " + weaponNum);
    }


    private WeaponBase currentWeaponScript;

    
    //---------------------------------- Player interaction ----------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------
}
