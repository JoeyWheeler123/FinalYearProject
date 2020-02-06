using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("box"))
        {
            print("BOUNCY BOX");
            Rigidbody boxRb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 collisionDir = other.contacts[0].normal;
            boxRb.AddForce(-collisionDir*70f*other.relativeVelocity.magnitude);
        }
    }
}
