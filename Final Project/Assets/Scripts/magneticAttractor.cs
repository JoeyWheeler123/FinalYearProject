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
    // Start is called before the first frame update
    void Start()
    {
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
            if (distanceMultiplier <= snapThreshold&&!Input.GetButton("Fire1"))
            {
                snapped = true;
            }
            
        }

        if (attracted != null & snapped)
        {
            attracted.transform.position = transform.position;
            attractedRb.isKinematic = true;
        }
        else
        {
           
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (attracted != null)
            {
                attractedRb.isKinematic = false;
            }

            ResetBox();
        }
        
        if(Input.GetKeyDown((KeyCode.G))&snapped)
        {
            
            rb.connectedBody = playerRb;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "metal")
        {
            attracted = other.gameObject;
            attractedRb = other.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        attracted = null;
    }

    public void ResetBox()
    {
        attracted = null;
        snapped = false;
    }
}
