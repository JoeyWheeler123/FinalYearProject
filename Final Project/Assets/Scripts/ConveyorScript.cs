using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right*Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        other.transform.parent = (this.transform);
    }
    
    private void OnCollisionExit(Collision other)
    {
        other.transform.SetParent(null);
    }
}
