using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabRight : MonoBehaviour
{
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
            moveBoy moveScript = other.gameObject.GetComponentInParent<moveBoy>();
            moveScript.grabPos = new Vector3(transform.position.x+1f,transform.position.y,transform.position.z);
            if (moveScript.moveInputX < -0.1f)
            {
                moveScript.LedgeGrab(1);
            }
        }
    }
}
