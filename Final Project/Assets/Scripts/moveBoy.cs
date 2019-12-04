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

    private float boxPushSpeed;
    public float boxPushMultiplier = 0.5f;
    public float maxAirborneSpeed;
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
    public bool thrown, boxPrep, heavy, heavyPull;

    public float recoveryTime;

    public float wallKickForce;
    public float recallSpeed;
    public float grabDistance;
    public float moveInputX, moveInputY;
    public float wallSlideModifier;

    public throwScript throwS;
    private bool inControl;

    public bool mantling;

    public bool grabbingLedge;

    public Vector3  grabPos;
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

  private Collider theBoxCollider;

  private float boxFrictionInitial;
  private bool interactPressed;
  
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
        theBoxCollider = theBox.GetComponent<Collider>();
        boxFrictionInitial = theBoxCollider.material.dynamicFriction;
    }

    private void StartJumpDebug()
    {
        //Debug.Log("Start jump");
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
        //rb.useGravity = true;
        //rb.isKinematic = false;
        if (grabbingLedge)
        {
                
           // StartCoroutine(Dismantle());
        }

        if (onRightWall&&!gcScript.grounded)
        {
            StartCoroutine(RightJump());
        }

        if (onLeftWall&&!gcScript.grounded)
        {
            StartCoroutine(LeftJump());
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

        if (!curMov.aiming && !thrown)
        {
            throwS.DropBox();
            ThrowReleased();
            thrown = true;
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
               // speed = aimWalkSpeed;
            }
        
    }

    private void AimControllerPressed()
    {
        curMov.ControllerAim();
        if (!thrown)
        {
            curMov.aiming = true;
           // speed = aimWalkSpeed;
        }
    }
    private void AimReleased()
    {
        if (curMov.aiming&&!thrown)
        {
            ThrowBox();
            ThrowReleased();
        }
        curMov.aiming = false;
        speed = maxSpeed; 
        //release to throw box code below
       
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
        boxPushSpeed = speed * boxPushMultiplier;
        theBox.transform.parent = null;
        //anim = GetComponent<Animator>();
        inControl = true;
        heavy = false;
        throwS = theBox.GetComponent<throwScript>();
        rb = GetComponent<Rigidbody>();
        rbox = theBox.GetComponent<Rigidbody>();
        curMov.aiming = false;
        
        
        
    }

    // Update is called once per frame
    private void Update()

    {
        //print(rb.velocity.magnitude);
        if (controls.Gameplay.Interact.triggered)
        {
            interactPressed = true;
          //  print("activated");
            //other.SendMessage("Interact");
        }
        else
        {
            interactPressed = false;
        }
        
        if (controls.Gameplay.Jump.triggered)
        {
           //print("Mantling");
           // StartCoroutine(Mantle(mantlePos));
        }
       
        //print(heavy);
        if (aimInput.magnitude >= 0.3f)
        {
            curMov.ControllerAim();
            if (!thrown)
            {
                curMov.aiming = true;
                speed = aimWalkSpeed;
            }
        }

        else if(aimInput.magnitude <=0.3f &&curMov.controllerAim)
        {
            {
               curMov.aiming = false;
               speed = maxSpeed; 
            }
        }
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
            anim.SetBool(groundedHash,true);
            timeLeftToJump = coyoteTime;
        }
        else
        {
            anim.SetBool(groundedHash,false);
            timeLeftToJump -= Time.deltaTime;
        }


        //   float acceleration = grounded ? walkAcceleration : airAcceleration;
//        float deceleration = grounded ? groundDeceleration : 0;

        rb.velocity = new Vector3(velocity.x, rb.velocity.y, 0);

      

     
      

      
        if (pressedThrow)
        {
            if (thrown&&!gcScript.onBox)
            {
                BoxRecall();
            }
            else
            {
                recalling = false;
            }
           // recalling = false;
        }

        else
        {
            recalling = false;
        }

        if (recalling)
        {
            if (theBoxCollider.material.dynamicFriction > 0.1f)
            {
                theBoxCollider.material.dynamicFriction -= Time.deltaTime;
            }
        }
        else
        {
            theBoxCollider.material.dynamicFriction = boxFrictionInitial;
        }
        
        //print(theBoxCollider.material.dynamicFriction);
        if (grabbingLedge&&!dismantling)
        {
            Vector3 pos =  Vector3.MoveTowards(transform.position,grabPos,Time.deltaTime*10f);
            float ledgeDistance = Vector3.Distance(transform.position, grabPos);
            bool withinRange;
            if (ledgeDistance <= 0.05f)
            {
                withinRange = true;
            }
            else
            {
                withinRange = false;
            }
            
            //rb.MovePosition(pos);
            if (!withinRange)
            {
                transform.position = pos;
            }

            if (controls.Gameplay.Jump.triggered&&withinRange)
            {
                transform.position = grabPos;
                print("Mantling");
                StartCoroutine(Mantle(mantlePos));
            }

            if (moveInputY < -0.7)
            {
                transform.parent = null;
               // print("dismantle");
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
        
        if (other.gameObject.CompareTag("leftwall")&&!grabbingLedge&&!mantling)
        {
            onLeftWall = true;
         
        }

        if (other.gameObject.CompareTag("rightwall")&&!grabbingLedge&&!mantling)
        {
            onRightWall = true;
           
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ledgeleft")&&!mantling&&!gcScript.grounded&&inControl&&!grabbingLedge&&moveInput.x>0f)
        {
            int direction = 0;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);     
        }
        
        if (other.gameObject.CompareTag("ledgeright")&&!mantling&&!gcScript.grounded&&inControl&&!grabbingLedge&&moveInput.x<0f)
        {
            int direction = 1;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);
        }
        
        if (other.gameObject.CompareTag("leftwall")&&!grabbingLedge&&!mantling)
        {
            onLeftWall = true;
            /*if (controls.Gameplay.Jump.triggered && gcScript.grounded == false)
            {
                StartCoroutine(LeftJump());
            }
            */

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

        if (other.gameObject.CompareTag("rightwall")&&!grabbingLedge&&!mantling)
        {
            onRightWall = true;
            /*if (controls.Gameplay.Jump.triggered&& gcScript.grounded == false)
            {
                StartCoroutine(RightJump());
            }
            */

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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("rightwall"))
        {
            onRightWall = false;
                rb.useGravity = true;
            
        }
        if (other.gameObject.CompareTag("leftwall"))
        {
            onLeftWall = false;
            rb.useGravity = true;
            
        }
    }

 

  

    private void GroundMovement()
    {
        if (moveInputX != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInputX, walkAcceleration * Time.deltaTime);
            anim.SetBool(movingHash,true);
            //anim.SetBool("Moving",true);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
            anim.SetBool(movingHash, false);
        }
    }

    private void AirMovement()
    {
        if (recalling && heavyPull)
        {
            velocity.x = rb.velocity.x;
            if (rb.velocity.magnitude > maxAirborneSpeed)
            {
                Vector3 playerMov = rb.velocity;
                playerMov.Normalize();
                rb.velocity = playerMov * maxAirborneSpeed;
                print("tOO FAST");
            }
            
        }
       else if (moveInputX != 0)
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
        anim.SetTrigger(jumpHash);
        if (gcScript.onBox)
        {
            rbox.velocity =  new Vector3(0, -jumpHeight, 0);
        }
    }

   

   
    
    public void LedgeGrab(int direction)
    {
        if (!mantling && !gcScript.grounded && inControl && !grabbingLedge)
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
    }
    public void ThrowBox()
    {
        anim.SetTrigger(throwHash);
        boxPrep = true;
       // theBox.SetActive(true);
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

    public void BoxFriction()
    {
        if (recalling)
        {
            
        }
    }

    public void PushSpeed()
    {
        speed = boxPushSpeed;
    }

    public void NormalSpeed()
    {
        speed = maxSpeed;
    }
    public void DirectionCheck()
    {
        
        if (moveInputX < 0&&!grabbingLedge&&!curMov.aiming&&inControl&&!mantling)
        {
            facingLeft = true;
            
        }
        if(moveInputX>0&&!grabbingLedge&&!curMov.aiming&&inControl&&!mantling)
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
        mantling = true;
       // inControl = false;
        //rb.isKinematic = false;
        while(timeSpent<=0.35f)
        {

            //rb.MovePosition(transform.position+(Vector3.up*Time.deltaTime*5f)); doesn't work on moving ledges, revert to this later if buggy for better code
            transform.position += Vector3.up * Time.deltaTime * 5f; //messier option but works on moving platform
            timeSpent += Time.deltaTime;
           // print("mantling");
            yield return null;
        }

        while (timeSpent <= 0.6f)
        {
            
            if (mantlePosition == 0) //mantleposition 0 means player is on the left side of the ledge
            {
                //rb.MovePosition(transform.position + (Vector3.right * Time.deltaTime * 5f));
                transform.position += (Vector3.right * Time.deltaTime * 5f);
            }

            if (mantlePosition == 1) // player is on the right
            {
               // rb.MovePosition(transform.position + (Vector3.left * Time.deltaTime * 5f));
                transform.position += (Vector3.left * Time.deltaTime * 5f);
            }
            timeSpent += Time.deltaTime;
            yield return null;
        }

        transform.parent = null;
        mantling = false;
        rb.isKinematic = false;
        yield return new WaitForSeconds(0.1f);
        //rb.AddForce(Vector3.right*20,ForceMode.Impulse);
        grabbingLedge = false;
        //inControl = true;
        yield return null;
    }

   IEnumerator CarryBox()
   {
     
       thrown = false;
       NormalSpeed();
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
            //print(inControl);
            velocity.x = -wallKickForce;
            rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
            facingLeft = true;
           
            yield return new WaitForSeconds(0.3f);
           // print(inControl);
            inControl = true;
           // print(inControl);
        
        yield return null;
    }
    
   IEnumerator RightJump()
    {
        //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
        inControl = false;
        //print(inControl);
        velocity.x = wallKickForce;
        rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
        facingLeft = false;
           
        yield return new WaitForSeconds(0.3f);
       // print(inControl);
        inControl = true;
        //print(inControl);
        
        yield return null;
    }

   IEnumerator Dismantle()
   {
       dismantling = true;
       yield return new WaitForSeconds(ledgeGrabGraceTime);
       grabbingLedge = false;
       dismantling = false;
       transform.parent = null;
       yield return null;
   }
}

