using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowFall : MonoBehaviour
{
    private Rigidbody rb;

    private moveBoy moveScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            moveScript = other.gameObject.GetComponent<moveBoy>();
            rb = other.gameObject.GetComponent<Rigidbody>();
            StartCoroutine(SlowDescent());
            print("sloooow");
        }
    }
    IEnumerator SlowDescent()
    {
        
        float slowRate = 4;
        float timeElapsed = 0;
       
        moveScript.enabled = false;
        rb.velocity = Vector3.zero;
        while (timeElapsed <=6f)
        {
            
            float newY = Mathf.Lerp(rb.velocity.y, 0, slowRate*Time.deltaTime);
            rb.velocity = new Vector3(rb.velocity.x, newY, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        moveScript.enabled = true;
        yield return null;
        Destroy(this.gameObject);
    }
}
