using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magneticAttractor : MonoBehaviour
{
    private GameObject attracted;

    public float attractiveForce;

    private Rigidbody attractedRb;

    private bool snapped;

    public float snapThreshold;

    public HingeJoint rb;

    public Rigidbody playerRb;

    public moveBoy moveScript;

    private bool firstTimeSnap;
    // Start is called before the first frame update
    void Start()
    {
        firstTimeSnap = true;
        moveScript = FindObjectOfType<moveBoy>();
        
        print(moveScript);
        rb = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attracted != null&!snapped)
        {
            Vector3 dir = attracted.transform.position - transform.position;
            float distanceMultiplier = dir.sqrMagnitude;
            float stronger = Mathf.Sqrt(distanceMultiplier);//electromagnetic force weakens a rate proportional to square of distance. I think. Maybe cubed but squared will do for now?
            attractedRb.AddForce(-dir*attractiveForce*Time.deltaTime/stronger);
            if (distanceMultiplier <= snapThreshold&&firstTimeSnap)
            {
                snapped = true;
               

            }
            else if (distanceMultiplier <= snapThreshold&&!moveScript.pressedThrow&&!firstTimeSnap)
            {
                snapped = true;
                
            }
            
        }

        if (attracted != null & snapped)
        {
            attracted.transform.position = transform.position;
            attractedRb.isKinematic = true;
            if (!moveScript.pressedThrow)
            {
                firstTimeSnap = false;
            }
        }
        else
        {
           
        }

        if (moveScript.pressedThrow&&!firstTimeSnap)
        {
            if (attracted != null&&attractedRb!=null&&moveScript.thrown)
            {
                print("pulloff");
                Debug.Log(firstTimeSnap);
                attractedRb.isKinematic = false;
                ResetBox();
            }

           
        }
        
        if(Input.GetKeyDown((KeyCode.G))&snapped)
        {
            
            rb.connectedBody = playerRb;
        }

        if (attracted != null)
        {
            if (Vector3.Distance(attracted.transform.position, this.transform.position)>=snapThreshold)
            {
                attracted = null;
                attractedRb = null;
                firstTimeSnap = true;
                print("reset");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "box"&&moveScript.thrown)
        {
            attracted = other.gameObject;
            attractedRb = other.gameObject.GetComponent<Rigidbody>();
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
        }
    }

    public void ResetBox()
    {
        attracted = null;
        snapped = false;
    }
}
