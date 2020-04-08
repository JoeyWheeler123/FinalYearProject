using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class magneticAttractor : MonoBehaviour
{
    public GameObject box;
    //private GameObject attracted;

    public float attractiveForce;

    //private Rigidbody attractedRb;

    private bool snapped;

    public float snapThreshold;

   /// public HingeJoint rb;

    //public Rigidbody playerRb;

    public moveBoy moveScript;

    private bool firstTimeSnap;
    

    public static bool resetBoxBool;
    private BoxProperties boxProperties;

    private Rigidbody boxRb;

    private bool withinRange;

    public float attractRange =4.2f;

    private Vector3 forceToAdd;
    // Start is called before the first frame update
    void Awake()
    {
        box = GameObject.FindGameObjectWithTag("box");
    }
    void Start()
    {
        
        boxProperties = box.GetComponent<BoxProperties>();
        firstTimeSnap = true;
        if (moveScript == null)
        {
            moveScript = FindObjectOfType<moveBoy>();
        }

       // print(moveScript);
        boxRb = box.GetComponent<Rigidbody>();
        //rb = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (attracted!=null&&resetBoxBool ==true)
        {
            ResetBox();
            attracted = null;
            attractedRb = null;
            firstTimeSnap = true;
            print("reset");
            resetBoxBool = false;
        }
        */
        
        if (withinRange & !snapped)
        {
            //print("inrange");
            Vector3 dir = box.transform.position - transform.position;
            float distanceMultiplier = dir.sqrMagnitude;
            float
                stronger = Mathf.Sqrt(
                    distanceMultiplier); //electromagnetic force weakens a rate proportional to square of distance. I think. Maybe cubed but squared will do for now?
            if (stronger > 0)
            {
                 forceToAdd = -dir * attractiveForce * Time.deltaTime / stronger;
            }
            else
            {
                forceToAdd = -dir * attractiveForce * Time.deltaTime / 0.01f;
            }

            boxRb.AddForce(forceToAdd);
            if (dir.magnitude <= snapThreshold && firstTimeSnap)
            {
                snapped = true;


            }
            else if (dir.magnitude <= snapThreshold && !moveScript.pressedThrow && !firstTimeSnap)
            {
                snapped = true;

            }

        }

        if (withinRange&& snapped)
        {

            boxProperties.MagnetStuck(transform);
                // attracted = other.gameObject;
                //  attractedRb = other.gameObject.GetComponent<Rigidbody>();
            
           // attracted.transform.position = transform.position;
            //attractedRb.isKinematic = true;
            if (!moveScript.pressedThrow)
            {
                firstTimeSnap = false;
            }
        }
        else
        {

        }

        if (moveScript.pressedThrow && !firstTimeSnap)
        {
            if (withinRange && moveScript.thrown)
            {
                boxProperties.stuck = false;
                boxRb.isKinematic = false;
                //print("pulloff");
                //Debug.Log(firstTimeSnap);
               //attractedRb.isKinematic = false;
                snapped = false;
            }


        }

        if (Input.GetKeyDown((KeyCode.G)) & snapped)
        {

            //rb.connectedBody = playerRb;
        }

        if (withinRange== true)
        {
            if (Vector3.Distance(box.transform.position, this.transform.position) >= snapThreshold * 2)
            {
                ResetBox();
                //attracted = null;
               // attractedRb = null;
                firstTimeSnap = true;
                //print("reset");
            }
        }

        if (Vector3.Distance(box.transform.position, transform.position) <= attractRange)
        {
            withinRange = true;
        }
        else
        {
            withinRange = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "box"&&moveScript.thrown)
        {
            //other.gameObject.SendMessage("MagnetStuck");
           // attracted = other.gameObject;
          // attractedRb = other.gameObject.GetComponent<Rigidbody>();
          //withinRange = true;
         // print("E N T E R");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("box") && moveScript.thrown)
        {
           // Vector3 magPos = transform.position;
           // other.gameObject.SendMessage("MagnetStuck",magPos);
            // attracted = other.gameObject;
            //  attractedRb = other.gameObject.GetComponent<Rigidbody>();
           // withinRange = true;
            //print("E N T E R");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("box"))
        {
           // attracted = null;
          //  attractedRb = null;
            //firstTimeSnap = true;
           // print("reset");
          // withinRange = false;
        }
    }

    public void ResetBox()
    {
       // attracted = null;
        snapped = false;
        withinRange = false;
    }

    private void OnDrawGizmosSelected()
    {
        Color newColour = new Color(255,255,255,0.2f);
        Gizmos.color = newColour;
        Gizmos.DrawSphere(transform.position,attractRange);
    }
}
