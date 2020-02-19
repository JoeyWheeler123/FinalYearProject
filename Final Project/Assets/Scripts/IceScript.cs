using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
      
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("box"))
        {
            other.material.dynamicFriction = 0;
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            other.SendMessage("IceOn");
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.SendMessage("IceOff");
        }
    }
}
