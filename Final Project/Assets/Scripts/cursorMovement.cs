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
    public GameObject rightBox, leftBox, projectedBox;

    public bool aiming;

    public moveBoy moveScript;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        moveScript = GetComponent<moveBoy>();

    }

    // Update is called once per frame
    void Update()
    {
        if (aiming&&!moveScript.thrown)
        {
            projectedBox.SetActive(true);
            projectedPoint = new Vector3(point.x, point.y, 0);
            closePoint = projectedPoint - player.position;
            closePoint.Normalize();
            closePoint *= throwMarkerDistance;
            float boxCheck = closePoint.x;
            // transform.position = new Vector3(point.x,point.y,0);
            closePoint = player.position + closePoint;
            projectedBox.transform.position = Vector3.Lerp(projectedBox.transform.position, closePoint, Time.deltaTime * 30f);

            if (boxCheck < 0)
            {
                //leftBox.SetActive(true);
               // rightBox.SetActive(false);
                moveScript.facingLeft = true;
            }
            else if (boxCheck > 0)
            {
               // rightBox.SetActive(true);
                //leftBox.SetActive(false);
                moveScript.facingLeft = false;
            }
        }
        else
        {
           //leftBox.SetActive(false);
            //rightBox.SetActive(false);
            projectedBox.SetActive(false);
        }

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
