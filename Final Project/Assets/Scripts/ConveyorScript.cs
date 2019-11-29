using System;
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

    private bool sending, returning;
    
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
            transform.position = Vector3.MoveTowards(transform.position, endPos.position, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, endPos.position) < 0.1f)
            {
                print("arrived");
                if (autoReturn)
                {
                    returning = true;
                }
                sending = false;
            }
        }

        if (returning)
        {
            //print("returning");
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, startPos) < 0.1f)
            {
                if (moveOnStart)
                {
                    sending = true;
                }
                returning = false;
                
            }
        }
        
        
        
    }

    private void OnCollisionEnter(Collision other)
    {
        other.transform.parent = (this.transform);
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            moveBoy moveScript = other.gameObject.GetComponent<moveBoy>();
            if (!moveScript.mantling && !moveScript.grabbingLedge)
            {
                other.transform.SetParent(null);
            }
        }
        else
        {
            other.transform.SetParent(null);
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
