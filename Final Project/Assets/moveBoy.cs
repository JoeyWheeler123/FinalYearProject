using System.Collections;
using System.Collections.Generic;
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

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;
    private Vector2 velocity;

    public groundCheck gcScript;

    private Rigidbody rb;

    public GameObject theBox;
    public magneticAttractor magScript;
    public cursorMovement curMov;
    public bool thrown;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        curMov.aiming = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gcScript.grounded == true)
        {
           
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        
        
     //   float acceleration = grounded ? walkAcceleration : airAcceleration;
//        float deceleration = grounded ? groundDeceleration : 0;
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
        }
        
        rb.velocity = new Vector3(velocity.x ,rb.velocity.y,0);
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (thrown == false)
            {
                theBox.SetActive(true);
                theBox.GetComponent<throwScript>().Throw();
                curMov.aiming = false;
                thrown = true;
            }
            else
            {
                theBox.SetActive(false);
               curMov.aiming = true;
                thrown = false;
            }
        }
    }
    
    public void Jumping()
    {
       // rb.AddForce (Vector2.up * jumpHeight, ForceMode.VelocityChange);
        rb.velocity = new Vector3(velocity.x ,jumpHeight,0);
    }
}
