using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float defaultBounceStrength =25f;

    public float overallModifier = 35f;
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
            //print("BOUNCY BOX");
            Rigidbody boxRb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 collisionDir = other.contacts[0].normal;
            boxRb.isKinematic = true;
            boxRb.isKinematic = false;
            print(other.relativeVelocity.magnitude);
            boxRb.AddForce((Vector3.up*overallModifier*defaultBounceStrength)+(Vector3.up*overallModifier*other.relativeVelocity.magnitude));
        }
    }
}
