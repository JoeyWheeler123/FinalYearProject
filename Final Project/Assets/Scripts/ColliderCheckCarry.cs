using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheckCarry : MonoBehaviour
{
    private throwScript throwS;
    private moveBoy moveScript;
    private Rigidbody playerRb;
    public float distanceCheck;

    public Transform rayPos;
    // Start is called before the first frame update
    void Start()
    {
        throwS = FindObjectOfType<throwScript>();
       playerRb = GetComponentInParent<Rigidbody>();
       moveScript = GetComponentInParent<moveBoy>();
    }

    // Update is called once per frame
    void Update()
    {
       // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rayPos.position, transform.TransformDirection(Vector3.down), out hit, distanceCheck, layerMask))
        {
            Debug.DrawRay(rayPos.position, transform.TransformDirection(Vector3.down) * distanceCheck, Color.yellow);
            Debug.Log("Did Hit");
            if (playerRb.velocity.y < 0)
            {
                print("falltime");
                moveScript.DropBox();
            }
        }
        else
        {
            Debug.DrawRay(rayPos.position, transform.TransformDirection(Vector3.down) * distanceCheck, Color.white);
          // Debug.Log("Did not Hit");
        }
        
    }
    
   

   
}
