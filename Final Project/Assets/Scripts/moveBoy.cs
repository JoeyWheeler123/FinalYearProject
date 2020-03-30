using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using UnityEditor;
/*
using UnityEditor.Build.Content;

using UnityEditorInternal;
*/
using UnityEngine;
using UnityEngine.Animations.Rigging;
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

    private bool onIce;
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
    public float iceAcceleration;
    public float iceDeceleration;
    
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
    public BoxProperties boxPropertiesScript;
    public Rig rigScript, pushIk;
    public bool inControl;
    
    public bool mantling;

    public bool grabbingLedge;
    public float boxJumpMultiplier;
    public Vector3  grabPos;
    private int mantlePos;

    public bool recalling, canJump, hasBox, dismantling, gamePaused;
    public bool facingLeft;
    public bool energyFull,energyAffectsRecallSpeed;
    public float coyoteTime, energyRechargeTime,energyDrainRate,heavyBoxEnergyMultiplier;

    private float timeLeftToJump;

    public float inverseRecallMultiplier; //how much of the force that is given to the box pulling toward the player acts on the player themselves.

    public Animator anim;

    private int movingHash,
        jumpHash,
        recallHash,
        groundedHash,
        throwHash,
        ledgeGrabHash,
        fallHash,
        slideHash,
        walkMultiplierHash;

    public GameObject playerModel;
    public GameObject boxSwingParent;
    public Transform boxHoldPos;
    public GameObject respawn;

    public float turnSpeed, aimWalkSpeed;

    private Collider boxCol;

    public bool autoPickup, grounded;

    public float ledgeGrabGraceTime;

    private float jumpCall, recallCoolDown;

    public bool pressedJump, pressedThrow, onLeftWall, onRightWall, pushing;

  private Collider theBoxCollider;

  public Collider leftCollider, rightCollider;
  private float boxFrictionInitial;

  public GameObject pauseCanvas;

    public GameObject boxSpawn;

  //private bool interactPressed;
  
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
        controls.Gameplay.Pause.performed += ctx => PauseTime();
        controls.Gameplay.Restart.performed += ctx => RestartCurrentLevel();
        controls.Gameplay.Quit.performed += ctx => QuitGame();
        
        boxCol = theBox.GetComponent<Collider>();
        movingHash = Animator.StringToHash("Moving");
        jumpHash = Animator.StringToHash("Jump");
        recallHash = Animator.StringToHash("Recall");
        groundedHash = Animator.StringToHash("Grounded");
        throwHash = Animator.StringToHash("Throw");
        ledgeGrabHash = Animator.StringToHash("LedgeGrab");
        fallHash = Animator.StringToHash("Fall");
        slideHash = Animator.StringToHash("Sliding");
        walkMultiplierHash = Animator.StringToHash("WalkMultiplier");
        curMov.aiming = false;
        theBoxCollider = theBox.GetComponent<Collider>();
        boxFrictionInitial = theBoxCollider.material.dynamicFriction;

        energyFull = true;
        pushing = false;
        gamePaused = false;
        
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
            boxCol.enabled = true;
            
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
        Time.timeScale = 1f;
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
        global::heavyBox.GlobalHeavyBoxCheck = false;
        // boxPropertiesScript = FindObjectOfType<BoxProperties>();

    }

    // Update is called once per frame
    private void Update()

    {
        if (gcScript.grounded)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        float boxDistance = Vector3.Distance(transform.position,theBox.transform.position);
        if (recalling && boxDistance <= 2)
        {
            rbox.useGravity = false;
           // print("Gravityoff");
        }
        else
        {
            rbox.useGravity = true;
        }
        recallCoolDown += Time.deltaTime;
        if(recalling)
        //print(rb.velocity.magnitude);
        if (controls.Gameplay.Interact.triggered)
        {
           // interactPressed = true;
          //  print("activated");
            //other.SendMessage("Interact");
        }
        else
        {
//            interactPressed = false;
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
                //speed = aimWalkSpeed;
            }
        }

        else if(aimInput.magnitude <=0.3f &&curMov.controllerAim)
        {
            {
               curMov.aiming = false;
               //speed = maxSpeed; 
            }
        }
        moveInputX = moveInput.x;
       // print(moveInput);
        moveInputY = moveInput.y;
        DirectionCheck();
        CheckColliders();
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
            if (theBoxCollider.material.dynamicFriction > 0.1f&&!global::heavyBox.GlobalHeavyBoxCheck)
            {
                theBoxCollider.material.dynamicFriction -= Time.deltaTime;
            }
            
            if (theBoxCollider.material.dynamicFriction < 4f&&global::heavyBox.GlobalHeavyBoxCheck)
            {
                theBoxCollider.material.dynamicFriction += Time.deltaTime;
                print("Grinding");
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

        /// check if the player has thrown the box for ik
        if (!thrown)
        {
            rigScript.weight += Time.deltaTime * 5f;
        }
        else
        {
            rigScript.weight -= Time.deltaTime * 5f;
        }

        if (pushing&&Mathf.Abs(moveInputX)>0.1f)
        {
            pushIk.weight += Time.deltaTime * 5f;
            
        }
        else
        {
            pushIk.weight -= Time.deltaTime * 5f;
           
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("box")&&recalling)
        {
            if (!gcScript.onBox)
            {
                //StartCoroutine(CarryBox());
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
            //Scene scene = SceneManager.GetActiveScene(); 
            //SceneManager.LoadScene(scene.name);
            gameObject.transform.position = respawn.transform.position;
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            theBox.transform.position = boxSpawn.transform.position;
        }

        
        /*if (other.gameObject.CompareTag("leftwall")&&!grabbingLedge&&!mantling&&thrown)
        {
            onLeftWall = true;
         
        }

        if (other.gameObject.CompareTag("rightwall")&&!grabbingLedge&&!mantling&&thrown)
        {
            onRightWall = true;
           
        }
        */
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ledgeleft")&&thrown&&!mantling&&!gcScript.grounded&&inControl&&!grabbingLedge&&moveInput.x>0f)
        {
            int direction = 0;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);     
            anim.SetTrigger(ledgeGrabHash);
        }
        
        if (other.gameObject.CompareTag("ledgeright")&&thrown&&!mantling&&!gcScript.grounded&&inControl&&!grabbingLedge&&moveInput.x<0f)
        {
            int direction = 1;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);
            anim.SetTrigger(ledgeGrabHash);
        }
        
        if (other.gameObject.CompareTag("leftwall")&&!grabbingLedge&&!mantling&&thrown)
        {
            
            onLeftWall = true;
            onRightWall = false;
            /*if (controls.Gameplay.Jump.triggered && gcScript.grounded == false)
            {
                StartCoroutine(LeftJump());
            }
            */

            if (moveInputX >= 0.1f && rb.velocity.y < 0)
            {
                anim.SetBool(slideHash,true);
                
                //rb.AddForce(Physics.gravity * -wallSlideModifier, ForceMode.Acceleration);
                rb.useGravity = false;
                rb.velocity = new Vector3(0,-wallSlideModifier,0);
            }
            else
            {
                rb.useGravity = true;
                anim.SetBool(slideHash, false);
            }

            //rb.AddForce(Physics.gravity *-0.5f);
        }

        if (other.gameObject.CompareTag("rightwall")&&!grabbingLedge&&!mantling&&thrown)
        {
           
            onRightWall = true;
            onLeftWall = false;
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
                anim.SetBool(slideHash,true);
                
            }
            else
            {
                rb.useGravity = true;
                anim.SetBool(slideHash, false);
            }
        }

       

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("rightwall"))
        {
            onRightWall = false;
                rb.useGravity = true;
                anim.SetBool(slideHash,false);
            
        }
        if (other.gameObject.CompareTag("leftwall"))
        {
            onLeftWall = false;
            rb.useGravity = true;
            anim.SetBool(slideHash, false);

        }
    }

 

  

    private void GroundMovement()
    {
        if (recalling && heavyPull)
        {
            velocity.x = rb.velocity.x;
        }
        else if (moveInputX != 0)
        {
            if (onIce)
            {
                if (moveInputX < 0)
                {
                    if (velocity.x >= -speed)
                    {
                        velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInputX, iceAcceleration * Time.deltaTime);
                        anim.SetBool(movingHash, false);
                    }
                    else
                    {
                        velocity.x = Mathf.MoveTowards(velocity.x, 0, iceDeceleration * Time.deltaTime);
                    }
                }
                
                if (moveInputX > 0)
                {
                    if (velocity.x <= speed)
                    {
                        velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInputX, iceAcceleration * Time.deltaTime);
                        anim.SetBool(movingHash, false);
                    }
                    else
                    {
                        velocity.x = Mathf.MoveTowards(velocity.x, 0, iceDeceleration * Time.deltaTime);
                    }
                }
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInputX, walkAcceleration * Time.deltaTime);
            }

            anim.SetBool(movingHash,true);
            anim.SetFloat(walkMultiplierHash,(Mathf.Abs(moveInputX)*speed/maxSpeed));
            //anim.SetBool("Moving",true);
        }
        else if(!onIce)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
            anim.SetBool(movingHash, false);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, iceDeceleration * Time.deltaTime);
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
        //rb.AddForce (Vector2.up * jumpHeight, ForceMode.VelocityChange);
        if (gcScript.onBox&&!throwS.grounded)
        {
            //this was to reduce jump box spam but feels clunky needs revision.
            //recallCoolDown = 0f;
           /* if (rbox.velocity.magnitude >= 5f)
            {
                print("fastbox");
                rb.velocity = new Vector3(velocity.x, jumpHeight/1.5f, 0);
            }
            else
            {
                rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
            }
            */
           if (energyFull&&!boxPropertiesScript.stuck)
           {
               rb.velocity = new Vector3(velocity.x, boxJumpMultiplier * jumpHeight, 0);
               rbox.velocity = new Vector3(0, -jumpHeight, 0);
               StartCoroutine(BoxCoolDown());
           }
           else
           {
               rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
           }

        }
        else
        {
            rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
        }

        anim.SetTrigger(jumpHash);
        if (gcScript.onBox)
        {
           // rbox.velocity =  new Vector3(0, -jumpHeight*2f, 0);
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
            anim.SetTrigger(ledgeGrabHash);
            //anim.Play("Ledge Grab");
            grabbingLedge = true;

            mantlePos = direction;
            rb.isKinematic = true;
        }
    }
    public void ThrowBox()
    {
//        anim.SetTrigger(throwHash);
        
        boxCol.enabled = true;
        boxPrep = true;
       // theBox.SetActive(true);
        throwS.Throw();
        curMov.aiming = false;
    }

    public void BoxRecall()
    {
        if (energyFull)
        {
            recalling = true;
            if (global::heavyBox.GlobalHeavyBoxCheck)
            {
                boxPropertiesScript.energy -= Time.deltaTime * energyDrainRate*heavyBoxEnergyMultiplier;
            }
            else
            {
                boxPropertiesScript.energy -= Time.deltaTime * energyDrainRate;
            }
            //print("pull");
            boxTowardPlayer = transform.position - theBox.transform.position;
            boxTowardPlayer.Normalize();
            if (energyAffectsRecallSpeed)
            {
                boxTowardPlayer = boxTowardPlayer * boxPropertiesScript.energy / 100f;
            }
            rbox.AddForce(boxTowardPlayer * Time.deltaTime * recallSpeed, ForceMode.Force);
            rb.AddForce(-boxTowardPlayer * Time.deltaTime * inverseRecallMultiplier * recallSpeed, ForceMode.Force);
            if (autoPickup)
            {
                if (Vector2.Distance(transform.position, theBox.transform.position) <= grabDistance&&!gcScript.onBox)
                {
                    //theBox.SetActive(false);
                    //curMov.aiming = true;
                   StartCoroutine(CarryBox());
                    //thrown = false;
                }
            }
        }

    }

    public void BoxFriction()
    {
        if (recalling)
        {
            
        }
    }

    public void CheckColliders()
    {
        if (facingLeft && !thrown)
        {
            leftCollider.gameObject.SetActive(true);
            rightCollider.gameObject.SetActive(false);
        }
        else if(!facingLeft&&!thrown)
        {
            leftCollider.gameObject.SetActive(false);
            rightCollider.gameObject.SetActive(true);
        }

        else
        {
            leftCollider.gameObject.SetActive(false);
            rightCollider.gameObject.SetActive(false);
        }
    }
    public void PushSpeed()
    {
        pushing = true;
        speed = boxPushSpeed;
        
    }

    public void NormalSpeed()
    {
        pushing = false;
        speed = maxSpeed;
        
    }

    public void DropBox()
    {
        throwS.DropBox();
        boxCol.enabled = true;
            
        ThrowReleased();
        thrown = true;
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

    private void PauseTime()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0;
            gamePaused = true;
            pauseCanvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            gamePaused = false;
            pauseCanvas.SetActive(false);
        }
    }

    private void QuitGame()
    {
        if (gamePaused)
        {
            Application.Quit();
        }
    }

    private void RestartCurrentLevel()
    {
        if (gamePaused)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void IceOn()
    {
        onIce = true;
    }

    public void IceOff()
    {
        onIce = false;
    }

    public void SuperPull()
    {
        rb.AddForce(-boxTowardPlayer * Time.deltaTime * inverseRecallMultiplier * recallSpeed*boxPropertiesScript.energyPullMultiplier, ForceMode.Force);
    }

    public void ResetVelocity()
    {
        velocity = Vector2.zero;
        moveInput = Vector2.zero;
        anim.SetBool(movingHash,false);
    }
    IEnumerator Mantle(int mantlePosition)
    {
        float transformY = transform.position.y;
        float timeSpent = 0;
        grabbingLedge = false;
        mantling = true;
        anim.SetTrigger(fallHash);
       // inControl = false;
        //rb.isKinematic = false;
        while(timeSpent<=0.35f)
        {

            //rb.MovePosition(transform.position+(Vector3.up*Time.deltaTime*5f)); doesn't work on moving ledges, revert to this later if buggy for better code
            transform.position += Vector3.up * Time.deltaTime * 7f; //messier option but works on moving platform
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
        
        //boxCol.enabled = true;

        theBox.transform.rotation = boxSwingParent.transform.rotation;
        yield return null;
    }

    IEnumerator LeftJump()
    {
        anim.SetTrigger(jumpHash);
            //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
            inControl = false;
            //print(inControl);
            velocity.x = -wallKickForce;
            rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
            facingLeft = true;
           
            yield return new WaitForSeconds(0.35f);
           // print(inControl);
            inControl = true;
           // print(inControl);
        
        yield return null;
    }
    
   IEnumerator RightJump()
    {
        anim.SetTrigger(jumpHash);
        //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
        inControl = false;
        //print(inControl);
        velocity.x = wallKickForce;
        rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
        facingLeft = false;
           
        yield return new WaitForSeconds(0.35f);
       // print(inControl);
        inControl = true;
        //print(inControl);
        
        yield return null;
    }

   IEnumerator Dismantle()
   {
       anim.SetTrigger(fallHash);
       dismantling = true;
       yield return new WaitForSeconds(ledgeGrabGraceTime);
       grabbingLedge = false;
       dismantling = false;
       transform.parent = null;
       yield return null;
   }

   IEnumerator BoxCoolDown()
   {
       energyFull = false;
      boxPropertiesScript.rechargeCoroutine= boxPropertiesScript.StartCoroutine("Recharge");
       float coolDownTimer = 0;
       while (coolDownTimer < energyRechargeTime)
       {
           coolDownTimer += Time.deltaTime;
           yield return null;
       }

       if (coolDownTimer >= energyRechargeTime)
       {
           energyFull = true;
       }
       yield return null;
   }
   
   
}

