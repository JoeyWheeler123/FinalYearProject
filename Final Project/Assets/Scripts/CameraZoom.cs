using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public GameObject cameraPosition;

    public float zoomLevel;
    private float originalZoom;
    private Vector3 originalPos, newPos;

    //public bool inArea;

    public float cameraSpeed;

    private bool inZone;

    public cursorMovement cmScript;
    // Start is called before the first frame update
    void Start()
    {
        cmScript = FindObjectOfType<cursorMovement>();
        if (cameraPosition == null)
        {
            cameraPosition = Camera.main.gameObject;
        }
        originalZoom = cameraPosition.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (inZone)
        {
            Vector3 cameraZoomPosition = new Vector3(cameraPosition.transform.position.x, cameraPosition.transform.position.y, zoomLevel);
            cameraPosition.transform.position = Vector3.Lerp(cameraPosition.transform.position,cameraZoomPosition,Time.deltaTime*cameraSpeed);
        }
        else
        {
            Vector3 cameraZoomPosition = new Vector3(cameraPosition.transform.position.x, cameraPosition.transform.position.y, originalZoom);
            cameraPosition.transform.position = Vector3.Lerp(cameraPosition.transform.position, cameraZoomPosition,
                Time.deltaTime * cameraSpeed);
        }
        */
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //originalPos = cameraPosition.transform.position;
            
            //newPos = new Vector3(originalPos.x,originalPos.y,zoomLevel);
           // inZone = true;
            cmScript.NewZoomLevel(zoomLevel);

        }
       
    }
    private void OnTriggerStay(Collider other)
    {
       
       
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           // originalPos = cameraPosition.transform.position;
           // newPos = new Vector3(originalPos.x,originalPos.y,originalZoom);
            
            //inZone = false;
            cmScript.NewZoomLevel(originalZoom);
        }
       
    }
}
