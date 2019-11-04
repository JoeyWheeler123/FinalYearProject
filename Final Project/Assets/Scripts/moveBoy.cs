using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Build.Content;
using UnityEngine;

public class moveBoy : MonoBehaviour
{
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    public float airDeceleration;
    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;
    private Vector2 velocity;
    private Vector2 boxTowardPlayer;

    public groundCheck gcScript;

    private Rigidbody rb, rbox;

    public GameObject theBox;
    public magneticAttractor magScript;
    public cursorMovement curMov;
    public bool thrown, boxPrep;

    public float recoveryTime;

    public float wallKickForce;
    public float recallSpeed;
    public float grabDistance;
    private float moveInput;
    public float wallSlideModifier;
    public throwScript throwS;
    // Start is called before the first frame update
    void Start()
    {
        throwS = theBox.GetComponent<throwScript>();
        rb = GetComponent<Rigidbody>();
        rbox = theBox.GetComponent<Rigidbody>();
        curMov.aiming = true;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (gcScript.grounded == true)
        {
            
            if (moveInput != 0)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
            }

        }
        else
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

        
        
        
     //   float acceleration = grounded ? walkAcceleration : airAcceleration;
//        float deceleration = grounded ? groundDeceleration : 0;
       
        rb.velocity = new Vector3(velocity.x ,rb.velocity.y,0);
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (thrown == false)
            {
                ThrowBox();
                //thrown = true;
            }
            else
            {
                /*theBox.SetActive(false);
               curMov.aiming = true;
                thrown = false;
                */
            }
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            if (thrown == false&&boxPrep==true)
            {
                thrown = true;
                boxPrep = false;
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (thrown)
            {
                BoxRecall();
            }

            
        }
        
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("leftwall"))
        {
            
            if (Input.GetButtonDown("Jump")&&gcScript.grounded ==false)
            {
              LeftJump();
            }

            if (moveInput >= 0.1f && rb.velocity.y < 0)
            {
                rb.AddForce(Physics.gravity * -wallSlideModifier, ForceMode.Acceleration);
            }

            //rb.AddForce(Physics.gravity *-0.5f);
        }
        if (other.gameObject.CompareTag("rightwall"))
        {
            if (Input.GetButtonDown("Jump")&&gcScript.grounded ==false)
            {
                RightJump();
            }
            //rb.useGravity = false;
            if (moveInput <= -0.1f&&rb.velocity.y<0)
            {
                rb.AddForce(Physics.gravity * -wallSlideModifier, ForceMode.Acceleration);
            }
        }
    }

    public void Jumping()
    {
       // rb.AddForce (Vector2.up * jumpHeight, ForceMode.VelocityChange);
        rb.velocity = new Vector3(velocity.x ,jumpHeight,0);
    }

    public void LeftJump()
    {
        //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
        velocity.x = -wallKickForce;
        rb.velocity = new Vector3(velocity.x ,jumpHeight,0);
    }
    
    public void RightJump()
    {
        //rb.AddForce (Vector2.left * jumpHeight, ForceMode.Impulse);
        velocity.x = wallKickForce;
        rb.velocity = new Vector3(velocity.x ,jumpHeight,0);
    }

    public void ThrowBox()
    {
        boxPrep = true;
        theBox.SetActive(true);
        throwS.Throw();
        curMov.aiming = false;
    }

    public void BoxRecall()
    {
        print("pull");
        boxTowardPlayer = transform.position - theBox.transform.position;
        boxTowardPlayer.Normalize();
        rbox.AddForce(boxTowardPlayer*Time.deltaTime*recallSpeed,ForceMode.Impulse);
                
        if (Vector2.Distance(transform.position, theBox.transform.position) <= grabDistance)
        {
            theBox.SetActive(false);
            curMov.aiming = true;
            thrown = false;
        }
    }
}
