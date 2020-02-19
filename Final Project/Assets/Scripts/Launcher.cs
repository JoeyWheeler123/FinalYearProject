using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Launcher : MonoBehaviour
{
    private Vector3 startPos;

    public Transform endPos;

    public float speed;
    public bool moveOnStart;
    public bool autoReturn;

    public bool sending, returning;


    private Rigidbody rb;
   
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sending = false;
        returning = false;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (sending)
        {
            // returning = false;
            Vector3 currentPos;
            currentPos = Vector3.MoveTowards(transform.position, endPos.position, Time.deltaTime * speed);
            rb.MovePosition(currentPos);
            
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
           
                Vector3 currentPos = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
                rb.MovePosition(currentPos);
                if (Vector3.Distance(transform.position, startPos) < 0.1f)
                {
                    if (moveOnStart)
                    {
                        sending = true;
                        returning = false;
                    }
                
                
                }
            
          
            
        }
    }
    public void Send()
    {
        returning = false;
        sending = true;
    }
    public void Activate()
    {
        if (!sending)
        {
            Send();
        }
        else
        {
           // Return();
        }
    }
}
