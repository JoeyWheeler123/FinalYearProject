using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityRepulsor : MonoBehaviour
{
    public GameObject player;
    public float maxDistance;
    private Rigidbody rb;
    public float konoPowa;
    private moveBoy moveScript;
    public groundCheck gc;
    Vector3 gravity;
    // Start is called before the first frame update
    void Start()
    {
        rb = player.gameObject.GetComponent<Rigidbody>();
        moveScript = player.gameObject.GetComponent<moveBoy>();
        gc = player.gameObject.GetComponentInChildren<groundCheck>();
        gravity = Physics.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 towardsPlayer = player.transform.position - transform.position;
        towardsPlayer.Normalize();
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= maxDistance)
        {
            /* float forceToAdd = konoPowa / (distance*distance);
             rb.AddForce(konoPowa*towardsPlayer*Time.deltaTime);
             print(forceToAdd);
             */
        
           
           Physics.gravity = gravity / 2f;
            
        }
        else
        {
            Physics.gravity = gravity;
           
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position,maxDistance);
    }
}
