using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlowFall : MonoBehaviour
{
    private Rigidbody rb;

    private moveBoy moveScript;

    public GameObject slowFallParticle;

    public BoxProperties boxProperties;

    public ParticleSystem recallParticle;
    public UnityEvent enableTutorial, disableControl;
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
        slowFallParticle.SetActive(true);
       // recallParticle.Play();
        rb.velocity = Vector3.zero;
        while (timeElapsed <=6f)
        {
            boxProperties.energy = 300;
            float newY = Mathf.Lerp(rb.velocity.y, 0, slowRate*Time.deltaTime);
            rb.velocity = new Vector3(rb.velocity.x, newY, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        //recallParticle.Stop();
        boxProperties.energy = 0;
        moveScript.enabled = true;
        slowFallParticle.SetActive(false);
        enableTutorial.Invoke();
        yield return null;
        Destroy(this.gameObject);
    }
}
