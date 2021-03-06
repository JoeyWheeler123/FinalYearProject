﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorScript : MonoBehaviour
{

    private Vector3 startPos;

    public Transform endPos;

    public float speed;
    public bool moveOnStart;
    public bool autoReturn;

    public bool sending, returning;

    public bool nestedSwitch;

    public GameObject nestedLayer;

    public bool launcher = false;
    // Start is called before the first frame update
    void Start()
    {
       
        sending = false;
        returning = false;
        startPos = transform.position;
        
        if (moveOnStart)
        {
            sending = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sending)
        {
           // returning = false;
            transform.position = Vector3.MoveTowards(transform.position, endPos.position, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, endPos.position) < 0.1f)
            {
//                print("arrived");
                if (autoReturn)
                {
                    returning = true;
                    sending = false;
                }
                
            }
        }

        if (returning)
        {
            //sending = false;
            //print("returning");
            if (!nestedLayer)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
                if (Vector3.Distance(transform.position, startPos) < 0.1f)
                {
                    if (moveOnStart)
                    {
                        sending = true;
                        returning = false;
                    }
                
                
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, nestedLayer.transform.position, Time.deltaTime * speed);
                if (Vector3.Distance(transform.position, nestedLayer.transform.position) < 0.1f)
                {
                    if (moveOnStart)
                    {
                        sending = true;
                        returning = false;
                    }
                
                
                }
            }
            
        }
        
        
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!launcher)
        {
            other.transform.parent = (this.transform);
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            moveBoy moveScript = other.gameObject.GetComponent<moveBoy>();
            if (!moveScript.mantling && !moveScript.grabbingLedge)
            {
                other.transform.SetParent(null);
                //other.transform.localScale = new Vector3(1f,1f,1f);
            }
        }
        else if(other.gameObject.CompareTag("box"))
        {
            other.transform.SetParent(null);
           // other.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        }
    }

    public void Send()
    {
        returning = false;
        sending = true;
    }

    public void Return()
    {
        sending = false;
        returning = true;
    }

    public void Activate()
    {
        if (!sending)
        {
            Send();
        }
        else
        {
            Return();
        }
    }
}
