using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    float xforce;
    float zforce;

    [SerializeField]float moveSpeed =5f;
    [SerializeField]float lookSpeed=2;
    [SerializeField]GameObject cam;
    [SerializeField]Vector3 boxSize;
    [SerializeField]float maxDistance;
    [SerializeField]LayerMask layerMask;
    [SerializeField]float jumpForce=3;
    Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CameraControl();

        if(GroundCheck() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up*jumpForce);

            //Debug.Log("Jumped");
        }
    }


    bool GroundCheck()
    {
        if(Physics.BoxCast(transform.position,boxSize,-transform.up,transform.rotation,maxDistance,layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /*void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawCube(transform.position-transform.up*maxDistance,boxSize);
    }*/



    

//Player input
//----------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------

    //simple input, will need to be updated later
    void PlayerMovement()
    {
        xforce = Input.GetAxis("Horizontal")*moveSpeed;
        zforce = Input.GetAxis("Vertical")*moveSpeed;

        rb.linearVelocity = transform.forward*zforce+transform.right*xforce;
    }


//Camera input
//----------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------

    Vector3 cameraRot;
    Vector3 playerRot;

    
    //basic mouse input for player script
    void CameraControl()
    {
        cameraRot = cam.transform.rotation.eulerAngles;
        cameraRot.x += -Input.GetAxis("Mouse Y") *lookSpeed;
        //clamp camera to x axis to avoid looking upside down
        cameraRot.x = Mathf.Clamp((cameraRot.x <= 180) ? cameraRot.x : (-360 -cameraRot.x), -80f, 80f);
        playerRot.y = Input.GetAxis("Mouse X")*lookSpeed;

        transform.Rotate(playerRot);

    }
}
