using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyGround : MonoBehaviour
{
    public UnityEvent destroyGround;
   
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
        if (other.gameObject.CompareTag("Player"))
        {
            destroyGround.Invoke();
            other.gameObject.GetComponent<moveBoy>().grounded = false;
            //Destroy(this.gameObject);
        }
    }
}
