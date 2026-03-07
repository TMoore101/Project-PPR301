using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    FPS_Control inputControls;
    public GameObject ground;
    Rigidbody rb;

    bool isGrounded;
    public float moveForce = 20000f;
    public float jumpForce = 500f;

    Vector2 move = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputControls = new FPS_Control();
        rb = GetComponent<Rigidbody>();

        inputControls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        PlayerMove();


        if(isGrounded && inputControls.Player_map.Jump.ReadValue<float>() >0){
            PlayerJump();
        }

    }


    void PlayerMove()
    {
        move = inputControls.Player_map.Movement.ReadValue<Vector2>();
        rb.AddForce(transform.forward *move.y *moveForce, ForceMode.Force);
        rb.AddForce(transform.right* move.x *moveForce);
    }

    void GroundCheck()
    {
        if(transform.position.y-ground.transform.position.y > 1)
        {
            isGrounded = false;
            rb.linearDamping = 0.1f;
        }
        else
        {
            isGrounded = true;
            rb.linearDamping = 3f;
        }
    }

    void PlayerJump()
    {
        rb.AddForce(Vector3.up*jumpForce);
    }


    
}
