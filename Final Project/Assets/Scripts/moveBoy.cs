using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using UnityEditor.Build.Content;
using UnityEditorInternal;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class moveBoy : MonoBehaviour
{
    public PlayerControls controls;
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    public float speed = 9;

    private float maxSpeed;
    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    public float airDeceleration;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    public float jumpHeight = 4;

    private Vector2 velocity;
    private Vector2 moveInput;
    public Vector2 aimInput;
    private Vector2 boxTowardPlayer;

    public groundCheck gcScript;

    public Rigidbody rb, rbox;

    public GameObject theBox;

    public GameObject heavyBox;

    //public magneticAttractor magScript;
    public cursorMovement curMov;
    public bool thrown, boxPrep, heavy;

    public float recoveryTime;

    public float wallKickForce;
    public float recallSpeed;
    public float grabDistance;
    public float moveInputX, moveInputY;
    public float wallSlideModifier;

    public throwScript throwS;
    private bool inControl;

    private bool mantling;

    private bool grabbingLedge;

    private Vector3  grabPos;
    private int mantlePos;

    private bool recalling, canJump, hasBox, dismantling;
    public bool facingLeft;

    public float coyoteTime;

    private float timeLeftToJump;

    public float inverseRecallMultiplier; //how much of the force that is given to the box pulling toward the player acts on the player themselves.

    public Animator anim;

    private int movingHash, jumpHash, recallHash, groundedHash,throwHash;

    public GameObject playerModel;
    public GameObject boxSwingParent;
    public Transform boxHoldPos;

    public float turnSpeed, aimWalkSpeed;

    private Collider boxCol;

    public bool autoPickup;

    public float ledgeGrabGraceTime;

    private float jumpCall;

  public bool pressedJump, pressedThrow, onLeftWall, onRightWall;
    // Start is called before the first frame update

    void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Movement.performed += ctx => moveInput =ctx.ReadValue<Vector2>();
        controls.Gameplay.Movement.canceled += ctx => moveInput = Vector2.zero;
        controls.Gameplay.RightStick.performed += ctx => aimInput =ctx.ReadValue<Vector2>();
        //controls.Gameplay.Movement.canceled += ctx => moveInput = Vector2.zero;
        controls.Gameplay.Jump.started += ctx => StartJumpDebug();
        controls.Gameplay.Jump.performed += JumpingDebug;
        
        controls.Gameplay.Jump.canceled += ctx => JumpCancelled();
        controls.Gameplay.Throw.started += ctx => ThrowPressed();
        controls.Gameplay.Throw.canceled += ctx => ThrowReleased();
        controls.Gameplay.AimMouse.performed += ctx => AimMousePressed();
        controls.Gameplay.AimMouse.canceled += ctx => AimReleased();
        controls.Gameplay.AimController.performed += ctx => AimControllerPressed();
        controls.Gameplay.AimController.canceled += ctx => AimReleased();
        boxCol = theBox.GetComponent<Collider>();
        movingHash = Animator.StringToHash("Moving");
        jumpHash = Animator.StringToHash("Jump");
        recallHash = Animator.StringToHash("Recall");
        groundedHash = Animator.StringToHash("Grounded");
        throwHash = Animator.StringToHash("Throw");
        curMov.aiming = false;
    }

    private void StartJumpDebug()
    {
        Debug.Log("Start jump");
    }

    private void JumpingDebug(InputAction.CallbackContext ctx)
    {
        if (timeLeftToJump>=0&&!gcScript.jumping)
        {
            Jumping();
            gcScript.jumping = true;
            timeLeftToJump = 0;
            //jumping = true;
            //jumpCoolDown = 0;
            //print("jumpboy");
        }

        //inControl = true;
        rb.useGravity = true;
        rb.isKinematic = false;
        if (grabbingLedge)
        {
                
            StartCoroutine(Dismantle());
        }
        //Debug.Log("Jumping");
        pressedJump = true;
        //Debug.Log(jumpCall);
    }

    private void JumpCancelled()
    {
       // Debug.Log("JumpCancelled");
        pressedJump = false;
        
    }

    private void ThrowPressed()
    {
        pressedThrow = true;
        if (curMov.aiming&&!thrown)
        {
            ThrowBox();
            ThrowReleased();
        }
    }

    private void ThrowReleased()
    {
        if (thrown == false && boxPrep == true)
        {
            thrown = true;
            boxPrep = false;
        }
        pressedThrow = false;
    }

    private void AimMousePressed()
    {
        
           curMov.MouseAim();
            if (!thrown)
            {
                curMov.aiming = true;
                speed = aimWalkSpeed;
            }
        
    }

    private void AimControllerPressed()
    {
        curMov.ControllerAim();
        if (!thrown)
        {
            curMov.aiming = true;
            speed = aimWalkSpeed;
        }
    }
    private void AimReleased()
    {
        curMov.aiming = false;
        speed = maxSpeed; 
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
    
    void Start()
    {
        maxSpeed = speed;
        theBox.transform.parent = null;
        anim = GetComponent<Animator>();
        inControl = true;
        heavy = false;
        throwS = theBox.GetComponent<throwScript>();
        rb = GetComponent<Rigidbody>();
        rbox = theBox.GetComponent<Rigidbody>();
        curMov.aiming = false;
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInputX = moveInput.x;
       // print(moveInput);
        moveInputY = moveInput.y;
        DirectionCheck();
        if(inControl){
        if (gcScript.grounded == true)
        {

           GroundMovement();

        }
        else
        {
            AirMovement();
            
        }
        }

        if (gcScript.grounded)
        {
            timeLeftToJump = coyoteTime;
        }
        else
        {
            timeLeftToJump -= Time.deltaTime;
        }


        //   float acceleration = grounded ? walkAcceleration : airAcceleration;
//        float deceleration = grounded ? groundDeceleration : 0;

        rb.velocity = new Vector3(velocity.x, rb.velocity.y, 0);

      

        if (Input.GetButtonDown("Jump"))
        {
         
            /*if (timeLeftToJump>=0&&!gcScript.jumping)
            {
                Jumping();
                gcScript.jumping = true;
                timeLeftToJump = 0;
                //jumping = true;
                //jumpCoolDown = 0;
                //print("jumpboy");
            }

            //inControl = true;
            rb.useGravity = true;
            rb.isKinematic = false;
            if (grabbingLedge)
            {
                
                StartCoroutine(Dismantle());
            }
            */

        }
        if (Input.GetButtonUp("Fire1"))
        {
           /* if (thrown == false && boxPrep == true)
            {
                thrown = true;
                boxPrep = false;
            }
            */
        }

      

        /*if (Input.GetButton("Fire2"))
        {
           
            if (!thrown)
            {
                curMov.aiming = true;
                speed = aimWalkSpeed;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (thrown == false)
                {
                    ThrowBox();
           
                }
           
            }
        }
        else
        {
            curMov.aiming = false;
            speed = maxSpeed;
        }
        */
        
        if (pressedThrow)
        {
            if (thrown&&!gcScript.onBox)
            {
                BoxRecall();
            }

           // recalling = false;
        }
     

        if (grabbingLedge&&!dismantling)
        {
            Vector3 pos =  Vector3.MoveTowards(transform.position,grabPos,Time.deltaTime*10f);
            rb.MovePosition(pos);
            if (moveInputY>0.7)
            {
                StartCoroutine(Mantle(mantlePos));
            }

            if (moveInputY < -0.7)
            {
                print("dismantle");
                rb.isKinematic = false;
                StartCoroutine(Dismantle());
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.CompareTag("ledgeleft")&&!mantling&&!gcScript.grounded&&inControl)
        {
            int direction = 0;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);     
        }
        
        if (other.gameObject.CompareTag("ledgeright")&&!mantling&&!gcScript.grounded&&inControl)
        {
            int direction = 1;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);
        }
        */

        if (other.gameObject.CompareTag("killplane"))
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ledgeleft")&&!mantling&&!gcScript.grounded&&inControl&&!grabbingLedge)
        {
            int direction = 0;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);     
        }
        
        if (other.gameObject.CompareTag("ledgeright")&&!mantling&&!gcScript.grounded&&inControl&&!grabbingLedge)
        {
            int direction = 1;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);
        }
        
        if (other.gameObject.CompareTag("leftwall"))
        {

            if (controls.Gameplay.Jump.triggered && gcScript.grounded == false)
            {
                StartCoroutine(LeftJump());
            }

            if (moveInputX >= 0.1f && rb.velocity.y < 0)
            {
                //rb.AddForce(Physics.gravity * -wallSlideModifier, ForceMode.Acceleration);
                rb.useGravity = false;
                rb.velocity = new Vector3(0,-wallSlideModifier,0);
            }
            else
            {
                rb.useGravity = true;
            }

            //rb.AddForce(Physics.gravity *-0.5f);
        }

        if (other.gameObject.CompareTag("rightwall"))
        {
            if (controls.Gameplay.Jump.triggered&& gcScript.grounded == false)
            {
                StartCoroutine(RightJump());
            }

            //rb.useGravity = false;
            if (moveInputX <= -0.1f && rb.velocity.y < 0)
            {
                rb.useGravity = false;
                //rb.AddForce(Physics.gravity * -wallSlideModifier, ForceMode.Acceleration);
                rb.velocity = new Vector3(0,-wallSlideModifier,0);
            }
            else
            {
                rb.useGravity = true;
            }
        }

        
    }

    private void GroundMovement()
    {
        if (moveInputX != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInputX, walkAcceleration * Time.deltaTime);
            anim.SetBool(movingHash,true);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
            anim.SetBool(movingHash, false);
        }
    }

    private void AirMovement()
    {
        if (moveInputX != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInputX, airAcceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, airDeceleration * Time.deltaTime);
        }
    }
    private void Jumping()
    {
        inControl = true;
        // rb.AddForce (Vector2.up * jumpHeight, ForceMode.VelocityChange);
        rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
        anim.SetBool(jumpHash,true);
        if (gcScript.onBox)
        {
            rbox.velocity =  new Vector3(0, -jumpHeight, 0);
        }
    }

   

   
    
    private void LedgeGrab(int direction)
    {
        if (direction == 0)
        {
            facingLeft = false;
        }
        else if (direction == 1)
        {
            facingLeft = true;
        }

        grabbingLedge = true;
        
        mantlePos = direction;
        rb.isKinematic = true;
    }
    public void ThrowBox()
    {
        anim.SetTrigger(throwHash);
        boxPrep = true;
        theBox.SetActive(true);
        throwS.Throw();
        curMov.aiming = false;
    }

    public void BoxRecall()
    {
        recalling = true;
        //print("pull");
        boxTowardPlayer = transform.position - theBox.transform.position;
        boxTowardPlayer.Normalize();
        rbox.AddForce(boxTowardPlayer * Time.deltaTime * recallSpeed, ForceMode.Force);
        rb.AddForce(-boxTowardPlayer*Time.deltaTime*inverseRecallMultiplier*recallSpeed,ForceMode.Force);
        if (autoPickup)
        {
            if (Vector2.Distance(transform.position, theBox.transform.position) <= grabDistance)
            {
                //theBox.SetActive(false);
                //curMov.aiming = true;
                StartCoroutine(CarryBox());
                //thrown = false;
            }
        }
    }

   

    public void DirectionCheck()
    {
        
        if (moveInputX < 0&&!grabbingLedge&&!curMov.aiming&&inControl)
        {
            facingLeft = true;
            
        }
        if(moveInputX>0&&!grabbingLedge&&!curMov.aiming&&inControl)
        {
            facingLeft = false;
        }        
        
        if (facingLeft)
        {
            Vector3 leftRot = new Vector3(0, -180, 0);
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation,
                Quaternion.Euler(leftRot),  Time.deltaTime*turnSpeed);
            
            
            boxSwingParent.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation,
                Quaternion.Euler(leftRot),  Time.deltaTime*turnSpeed);
        }

        if (!facingLeft)
        {
            Vector3 rightRot = new Vector3(0, 0, 0);
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation,
                Quaternion.Euler(rightRot),  Time.deltaTime*turnSpeed);
            
            boxSwingParent.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation,
                Quaternion.Euler(rightRot),  Time.deltaTime*turnSpeed);
        }
    }

    IEnumerator Mantle(int mantlePosition)
    {
        float transformY = transform.position.y;
        float timeSpent = 0;
        grabbingLedge = false;
        //rb.isKinematic = false;
        while(timeSpent<=0.5f)
        {

            rb.MovePosition(transform.position+(Vector3.up*Time.deltaTime*5f));
            timeSpent += Time.deltaTime;
            //print("mantling");
            yield return null;
        }

        while (timeSpent <= 0.7f)
        {
            if (mantlePosition == 0) //mantleposition 0 means player is on the left side of the ledge
            {
                rb.MovePosition(transform.position + (Vector3.right * Time.deltaTime * 5f));
            }

            if (mantlePosition == 1) // player is on the right
            {
                rb.MovePosition(transform.position + (Vector3.left * Time.deltaTime * 5f));
            }
            timeSpent += Time.deltaTime;
            yield return null;
        }
        mantling = false;
        rb.isKinematic = false;
        yield return new WaitForSeconds(0.1f);
        //rb.AddForce(Vector3.right*20,ForceMode.Impulse);
        grabbingLedge = false;
        yield return null;
    }

   IEnumerator CarryBox()
   {
       thrown = false;
        float timeSpent = 0;
        rbox.isKinematic = true;
        
        
        //theBox.transform.eulerAngles = new Vector3(0, 0, 0);
        
        
        
        boxCol.enabled = false;
        theBox.transform.SetParent(boxSwingParent.transform);
        while (timeSpent <0.4f)
        {
            Quaternion rot = boxSwingParent.transform.rotation;
            theBox.transform.position = Vector3.MoveTowards(theBox.transform.position,boxHoldPos.position,Time.deltaTime*10f);
            theBox.transform.rotation =
                Quaternion.Slerp(theBox.transform.rotation, rot,Time.deltaTime*10f);
            timeSpent += Time.deltaTime;
            yield return null;
        }
        
        boxCol.enabled = true;

        theBox.transform.rotation = boxSwingParent.transform.rotation;
        yield return null;
    }

    IEnumerator LeftJump()
    {
        
            //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
            inControl = false;
            print(inControl);
            velocity.x = -wallKickForce;
            rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
            facingLeft = true;
           
            yield return new WaitForSeconds(0.3f);
            print(inControl);
            inControl = true;
            print(inControl);
        
        yield return null;
    }
    
   IEnumerator RightJump()
    {
        //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
        inControl = false;
        print(inControl);
        velocity.x = wallKickForce;
        rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
        facingLeft = false;
           
        yield return new WaitForSeconds(0.3f);
        print(inControl);
        inControl = true;
        print(inControl);
        
        yield return null;
    }

   IEnumerator Dismantle()
   {
       dismantling = true;
       yield return new WaitForSeconds(ledgeGrabGraceTime);
       grabbingLedge = false;
       dismantling = false;
       yield return null;
   }
}

