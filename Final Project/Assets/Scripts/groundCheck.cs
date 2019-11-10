using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour
{
    public bool grounded;

    public moveBoy ms;
    
    private float jumpCoolDown;
   public float jumpCoolDownInitial;

    public bool jumping;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (jumping)
        {
            jumpCoolDown += Time.deltaTime;
        }

        if (jumpCoolDown >= jumpCoolDownInitial)
        {
            jumping = false;
            jumpCoolDown = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.CompareTag("ground"))
        {
            grounded = true;
            
            
            /* if (Input.GetButtonDown("Jump"))
             {
                 ms.Jumping();
                 jumping = true;
                 jumpCoolDown = 0;
                 print("jumpboy");
             }
             */
        }
    }


    private void OnTriggerExit(Collider other)
    {
        grounded = false;
        //print(grounded);
    }
}
