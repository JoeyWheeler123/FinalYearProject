using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwScript : MonoBehaviour
{
    public Transform cursorTransform;
    public Transform playerTransform;

    public Rigidbody rb;
    public Transform spawnPointLeft, spawnPointRight;
    public float force;
    public GameObject rightBox, leftBox;
    public float maxVelocity;
    public moveBoy moveScript;
    public Vector3 velocityLastFrame;
    public GameObject respawn;
    public bool grounded;
    public bool freeMove;
    public enum ThrowPoint

    {
        left,
        right
    }

    private float tempX, tempY;

    private ThrowPoint tp;

    // Start is called before the first frame update
    void Start()
    {
        freeMove = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
  


    private void Update()
    {
        if (Mathf.Abs(transform.position.z) >=0.1f&&moveScript.thrown&&!freeMove)
        {
            Vector3 movPos = new Vector3(transform.position.x,transform.position.y,0);
           // rb.MovePosition(movPos);
           transform.position = Vector3.Lerp(transform.position, movPos, Time.deltaTime * 10f);
        }
        velocityLastFrame = rb.velocity;
        /*if (rb.velocity.x > maxVelocity)
        {
            tempX = maxVelocity;
        }
        else if (rb.velocity.x < -maxVelocity)
        {
            tempX = -maxVelocity;
        }
        else
        {
            tempX = rb.velocity.x;
        }
        
        if (rb.velocity.y > maxVelocity)
        {
            tempY = maxVelocity;
        }
        else if (rb.velocity.y < -maxVelocity)
        {
            tempY = -maxVelocity;
        }
        else
        {
            tempY = rb.velocity.y;
        }
    
        rb.velocity = new Vector3(tempX, tempY,0f);
        */
       
    }

    public void Throw()
    {
        rb.isKinematic = false;
        if (cursorTransform.position.x <= playerTransform.position.x)
        {
            tp = ThrowPoint.left;
        }

        if (cursorTransform.position.x >= playerTransform.position.x)
        {
            tp = ThrowPoint.right;
        }

        if (tp == ThrowPoint.left)
        {
            transform.position = spawnPointLeft.position;
        }

        if (tp == ThrowPoint.right)
        {
            transform.position = spawnPointRight.position;
        }

        transform.parent = null;
        rb.velocity = Vector3.zero;
        Vector3 dir = cursorTransform.position - transform.position;
        dir.Normalize();
        print("Throw");
        rb.AddForce(dir * force, ForceMode.Impulse);

    }

    public void DropBox()
    {
        rb.isKinematic = false;
        if (moveScript.facingLeft)
        {
            //transform.position = spawnPointLeft.position;
        }
        else
        {
            //transform.position = spawnPointRight.position;

        }
       
        transform.parent = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("boxgrabber"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                moveScript.StartCoroutine("CarryBox");
            }
        }

        if (other.gameObject.CompareTag("hands"))
        {
            
                moveScript.PushSpeed();
           
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("hands"))
        {
            moveScript.NormalSpeed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if( other.gameObject.CompareTag("killplane"))
        {
            gameObject.transform.position = respawn.transform.position;
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            grounded = false;
        }
    }
}
