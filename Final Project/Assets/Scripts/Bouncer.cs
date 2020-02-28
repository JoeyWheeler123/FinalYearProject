using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float defaultBounceStrength =25f;

    public float overallModifier = 35f;

    public GameObject box;

    public Renderer emissiveRenderer;

    public Color bounceColour;

    private BoxProperties boxProperties;

    private Coroutine changeColour;
    // Start is called before the first frame update
    void Start()
    {
        boxProperties = FindObjectOfType<BoxProperties>();
        //rend = GetComponent<Renderer>();
        bounceColour = emissiveRenderer.material.GetColor("_EmissiveColor");
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
           if(changeColour !=null) boxProperties.StopCoroutine("SwitchBoxColour");
            boxProperties.StopCoroutine("Recharge");
            if (!global::heavyBox.GlobalHeavyBoxCheck)
            {
                changeColour= boxProperties.StartCoroutine("SwitchBoxColour", bounceColour);
            }

            boxRb.isKinematic = true;
            boxRb.isKinematic = false;
            box = other.gameObject;
            print(other.relativeVelocity.magnitude);
            float bounceHeight = defaultBounceStrength + (other.relativeVelocity.magnitude * overallModifier);
            boxRb.velocity = new Vector3(boxRb.velocity.x,bounceHeight,boxRb.velocity.z);
           // boxRb.AddForce((Vector3.up*overallModifier*other.relativeVelocity.magnitude));
            //(Vector3.up*overallModifier*defaultBounceStrength)
            StartCoroutine(RotateBox());
        }
    }

    IEnumerator RotateBox()
    {
        float timeElapsed = 0;
        Quaternion rot = Quaternion.identity;
        while(timeElapsed <= 1f)
        {
            box.transform.rotation =
                Quaternion.Slerp(box.transform.rotation, rot,Time.deltaTime*4f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
