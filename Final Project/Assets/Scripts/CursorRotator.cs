using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorRotator : MonoBehaviour
{
    public bool directionRotation;
    public bool randomRotation;
    public float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (randomRotation)
        {
            transform.Rotate(transform.up*rotateSpeed*Time.deltaTime);
        }
    }
}
