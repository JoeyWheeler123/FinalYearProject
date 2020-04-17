using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEventEvoke : MonoBehaviour
{
    public UnityEvent eventToinvoke;
    public bool repeatableEvent;
    public bool introEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (introEvent)
            {
                if (SavePoint.currentCheckpoint == 0)
                {
                    eventToinvoke.Invoke();
                }
            }
            else
            {
                eventToinvoke.Invoke();
            }
            if (!repeatableEvent)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
