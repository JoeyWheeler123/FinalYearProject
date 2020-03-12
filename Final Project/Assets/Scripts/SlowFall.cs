using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowFall : MonoBehaviour
{
    private Rigidbody rb;
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
            rb = other.gameObject.GetComponent<Rigidbody>();
            StartCoroutine(SlowDescent());
            print("sloooow");
        }
    }
    IEnumerator SlowDescent()
    {
        float slowRate = 4;
        float timeElapsed = 0;   
        while (timeElapsed <=4f)
        {
            float newY = Mathf.Lerp(rb.velocity.y, 0, slowRate*Time.deltaTime);
            rb.velocity = new Vector3(rb.velocity.x, newY, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return null;
        Destroy(this.gameObject);
    }
}
