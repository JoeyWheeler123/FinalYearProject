using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwScript : MonoBehaviour
{
    public Transform cursorTransform;
    public Transform playerTransform;

    public Rigidbody rb;
    public Transform spawnPointLeft, spawnPointRight;
    public float force;
    public GameObject rightBox, leftBox;
    public enum ThrowPoint
    {
        left,
        right
    }

    private ThrowPoint tp;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Throw()
    {
        if (cursorTransform.position.x <= playerTransform.position.x)
        {
            tp = ThrowPoint.left;
        }
        if (cursorTransform.position.x >= playerTransform.position.x)
        {
            tp = ThrowPoint.right;
        }
        
        if (tp == ThrowPoint.left)
        {
            transform.position = spawnPointLeft.position;
        }
        
        if (tp == ThrowPoint.right)
        {
            transform.position = spawnPointRight.position;
        }

        transform.parent = null;
        rb.velocity = Vector3.zero;
        Vector3 dir = cursorTransform.position - transform.position;
        dir.Normalize();
        print("Throw");
        rb.AddForce (dir * force, ForceMode.Impulse);
    }
}
