using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform defaultCameraPosition;

    public bool aiming;

    public Vector3 projectedPoint;
    // Start is called before the first frame update
    void Start()
    {
        defaultCameraPosition.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiming)
        {
            transform.position = defaultCameraPosition.position + projectedPoint;
        }
        else
        {
            transform.position = defaultCameraPosition.position;
        }
    }
}
