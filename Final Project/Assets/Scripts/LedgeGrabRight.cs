using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabRight : MonoBehaviour
{
    public bool elevator;
    public Transform elevatorTransform;

    private moveBoy moveScript;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("hands"))
        {
            if (moveScript == null)
            {
                moveScript = other.gameObject.GetComponentInParent<moveBoy>();
            }

            moveScript.grabPos = new Vector3(transform.position.x+1.3f,transform.position.y-0.65f,transform.position.z);
            if (moveScript.moveInputX < -0.1f)
            {
                moveScript.LedgeGrab(1);
                if (elevator)
                {
                    moveScript.gameObject.transform.parent = transform;
                }
            }
        }
    }
}
