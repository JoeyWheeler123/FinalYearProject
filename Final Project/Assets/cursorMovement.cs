using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class cursorMovement : MonoBehaviour
{
    private Camera cam;
    Vector3 point = new Vector3();

    private Vector3 closePoint;

    private Vector3 projectedPoint;

    public Transform player;

    public float throwMarkerDistance;

    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        projectedPoint = new Vector3(point.x,point.y,0);
        closePoint = projectedPoint - player.position;
        closePoint.Normalize();
        closePoint*= throwMarkerDistance;
       // transform.position = new Vector3(point.x,point.y,0);
       closePoint = player.position + closePoint;
       transform.position = Vector3.Lerp(transform.position, closePoint, Time.deltaTime*30f);


    }
    
    void OnGUI()
    {
        
        Event   currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane+10));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
//        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();
    }
}
