using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public GameObject cameraPosition;

    public float zoomLevel;
    private float originalZoom;
    private Vector3 originalPos, newPos;

    public bool inArea;

    public float cameraSpeed;
    // Start is called before the first frame update
    void Start()
    {
        originalZoom = cameraPosition.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            originalPos = cameraPosition.transform.position;
            newPos = new Vector3(originalPos.x,originalPos.y,originalPos.z-zoomLevel);
            cameraPosition.transform.position = newPos;
        }
       
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            originalPos = cameraPosition.transform.position;
            newPos = new Vector3(originalPos.x,originalPos.y,originalZoom);
            cameraPosition.transform.position = newPos;
        }
       
    }
}
