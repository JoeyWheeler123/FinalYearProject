using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using UnityEditor.Build.Content;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class moveBoy : MonoBehaviour
{
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    public float speed = 9;

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
    public float moveInput;
    public float wallSlideModifier;

    public throwScript throwS;
    private bool inControl;

    private bool mantling;

    private bool grabbingLedge;

    private Vector3  grabPos;
    private int mantlePos;

    private bool recalling, canJump, hasBox;
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
    // Start is called before the first frame update

    void Awake()
    {
        movingHash = Animator.StringToHash("Moving");
        jumpHash = Animator.StringToHash("Jump");
        recallHash = Animator.StringToHash("Recall");
        groundedHash = Animator.StringToHash("Grounded");
        throwHash = Animator.StringToHash("Throw");
    }
    void Start()
    {
        theBox.transform.parent = null;
        anim = GetComponent<Animator>();
        inControl = true;
        heavy = false;
        throwS = theBox.GetComponent<throwScript>();
        rb = GetComponent<Rigidbody>();
        rbox = theBox.GetComponent<Rigidbody>();
        curMov.aiming = true;
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
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

            if (timeLeftToJump>=0&&!gcScript.jumping)
            {
                Jumping();
                gcScript.jumping = true;
                timeLeftToJump = 0;
                //jumping = true;
                //jumpCoolDown = 0;
                print("jumpboy");
            }

            inControl = true;
            rb.useGravity = true;
            rb.isKinematic = false;
            grabbingLedge = false;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (thrown == false && boxPrep == true)
            {
                thrown = true;
                boxPrep = false;
            }
        }

      

        if (Input.GetButton("Fire2"))
        {
            curMov.aiming = true;
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
        }
        
        if (Input.GetButton("Fire1"))
        {
            if (thrown&&!gcScript.onBox)
            {
                BoxRecall();
            }

            recalling = false;
        }
     

        if (grabbingLedge)
        {
            Vector3 pos =  Vector3.MoveTowards(transform.position,grabPos,Time.deltaTime*10f);
            rb.MovePosition(pos);
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(Mantle(mantlePos));
            }
            
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ledgeleft")&&!mantling&&!gcScript.grounded)
        {
            int direction = 0;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);     
        }
        
        if (other.gameObject.CompareTag("ledgeright")&&!mantling&&!gcScript.grounded)
        {
            int direction = 1;
            grabPos = other.gameObject.transform.position;
            LedgeGrab(direction);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("leftwall"))
        {

            if (Input.GetButtonDown("Jump") && gcScript.grounded == false)
            {
                LeftJump();
            }

            if (moveInput >= 0.1f && rb.velocity.y < 0)
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
            if (Input.GetButtonDown("Jump") && gcScript.grounded == false)
            {
                RightJump();
            }

            //rb.useGravity = false;
            if (moveInput <= -0.1f && rb.velocity.y < 0)
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
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
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
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, airAcceleration * Time.deltaTime);
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

    private void LeftJump()
    {
        //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
        velocity.x = -wallKickForce;
        rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
        facingLeft = true;
    }

    private void RightJump()
    {
        //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
        velocity.x = wallKickForce;
        rb.velocity = new Vector3(velocity.x, jumpHeight, 0);
        facingLeft = false;
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
        print("pull");
        boxTowardPlayer = transform.position - theBox.transform.position;
        boxTowardPlayer.Normalize();
        rbox.AddForce(boxTowardPlayer * Time.deltaTime * recallSpeed, ForceMode.Force);
        rb.AddForce(-boxTowardPlayer*Time.deltaTime*inverseRecallMultiplier*recallSpeed,ForceMode.Force);

        if (Vector2.Distance(transform.position, theBox.transform.position) <= grabDistance)
        {
            //theBox.SetActive(false);
            //curMov.aiming = true;
            StartCoroutine(CarryBox());
            thrown = false;
        }
    }

   

    public void DirectionCheck()
    {
        
        if (moveInput < 0&&!grabbingLedge&&!curMov.aiming)
        {
            facingLeft = true;
            
        }
        if(moveInput>0&&!grabbingLedge&&!curMov.aiming)
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
            print("mantling");
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
        float timeSpent = 0;
        rbox.isKinematic = true;
        
        
        //theBox.transform.eulerAngles = new Vector3(0, 0, 0);
        
        
        Collider boxCol = theBox.GetComponent<Collider>();
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
}

