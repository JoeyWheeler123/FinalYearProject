﻿using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.InputSystem;
public class cursorMovement : MonoBehaviour



{
    private Camera cam;
    Vector3 point = new Vector3();
    public float minY;
    private Vector3 closePoint;

    private Vector3 projectedPoint;

    public Transform player;

    public float throwMarkerDistance;
    public GameObject rightBox, leftBox, projectedBox;
    public Renderer boxRenderer;

    public bool aiming;

    public moveBoy moveScript;

    public bool controllerAim;

    private float boxCheck;

    public float lowestAimPoint;

    public Transform cameraDefaultPosition;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        moveScript = GetComponent<moveBoy>();
        aiming = false;
        cameraDefaultPosition.position = cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controllerAim)
        {
            float inputY = point.y;
            /*if (inputY < lowestAimPoint)
            {
                inputY = lowestAimPoint;
            }
            */
            projectedPoint = new Vector3(point.x, point.y, 0);



            closePoint = projectedPoint - player.position;
            closePoint.Normalize();
            closePoint *= throwMarkerDistance;
            boxCheck = closePoint.x;
            // transform.position = new Vector3(point.x,point.y,0);
            closePoint = player.position + closePoint;
            projectedBox.transform.position =
                Vector3.Lerp(projectedBox.transform.position, closePoint, Time.deltaTime * 10f);
            /*if (aiming)
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position,new Vector3(projectedBox.transform.position.x,
                    projectedBox.transform.position.y+cameraDefaultPosition.localPosition.y, cameraDefaultPosition.position.z),Time.deltaTime*10f);
            }
            else
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position,cameraDefaultPosition.position,Time.deltaTime*10f);
            }
            */
        }
        else
        {
            float inputY = moveScript.aimInput.y;
            if (inputY < lowestAimPoint)
            {
                inputY = lowestAimPoint;
            }
                projectedPoint = new Vector3(moveScript.aimInput.x,moveScript.aimInput.y,0f);
                projectedPoint.Normalize();
                projectedPoint *= throwMarkerDistance;
                closePoint = player.position+projectedPoint;
                
               
                boxCheck = moveScript.aimInput.x;
                //closePoint = player.position + closePoint;
                projectedBox.transform.position =
                    Vector3.Lerp(projectedBox.transform.position, closePoint, Time.deltaTime * 10f);
                
            }

        if (aiming&&!moveScript.thrown)
        {
           // projectedBox.SetActive(true);
            boxRenderer.enabled = true;
           
            //projectedBox.transform.position = closePoint;
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

          
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, new Vector3(
                        projectedBox.transform.position.x,
                        projectedBox.transform.position.y + cameraDefaultPosition.localPosition.y,
                        cameraDefaultPosition.position.z), Time.deltaTime * 10f);
               // Vector3 camPosClamped = new Vector3(cam.transform.position.x,Mathf.Clamp(cam.transform.position.y,minY,Mathf.Infinity),cam.transform.position.z);

               // cam.transform.position = Vector3.Lerp(cam.transform.position, camPosClamped, Time.deltaTime * 10f);
               
               cam.transform.position= new Vector3(cam.transform.position.x,Mathf.Clamp(cam.transform.position.y,minY,Mathf.Infinity),cam.transform.position.z);
        }
        else
        {
           //leftBox.SetActive(false);
            //rightBox.SetActive(false);
            //projectedBox.SetActive(false);
            cam.transform.position = Vector3.MoveTowards(cam.transform.position,cameraDefaultPosition.position,Time.deltaTime*10f);
            boxRenderer.enabled = false;
        }

    }

    public void ControllerAim()
    {
        controllerAim = true;
    }

    public void MouseAim()
    {
        controllerAim = false;
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

        //GUILayout.BeginArea(new Rect(20, 20, 250, 120));
       // GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
       // GUILayout.Label("Mouse position: " + mousePos);
//        GUILayout.Label("World position: " + point.ToString("F3"));
        //GUILayout.EndArea();
    }
}
