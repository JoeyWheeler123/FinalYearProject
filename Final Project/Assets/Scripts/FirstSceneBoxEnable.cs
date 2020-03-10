using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneBoxEnable : MonoBehaviour
{
    public GameObject theBox;

    public Transform boxSpawnPos;

    public moveBoy moveScript;

    public Transform openingCameraTransform;

    public Transform camera;

    public float transitionTime, cameraMoveSpeed;

    public cursorMovement camScript;
    // Start is called before the first frame update
    void Start()
    {
        
        moveScript = FindObjectOfType<moveBoy>();
        camScript = FindObjectOfType<cursorMovement>();
        StartCoroutine(DelayBoxDisable());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            theBox.SetActive(true);
            theBox.transform.position = boxSpawnPos.position;

        }
    }

    IEnumerator DelayBoxDisable()
    {
       
        if (SavePoint.currentCheckpoint == 0)
        {
            theBox.transform.position = boxSpawnPos.position;
            theBox.SetActive(false);
            // StartCoroutine(OpeningScene()) ;
        }

        yield return null;
    }
    IEnumerator OpeningScene()
    {
       // moveScript.enabled = false;
        //camScript.enabled = false;
        Vector3 originalCameraPos = camera.position;
        Quaternion originalCameraRotation = camera.transform.rotation;
        camera.position = openingCameraTransform.position;
        camera.rotation = openingCameraTransform.rotation;
        
        float timeElapsed=0;
        while (timeElapsed <= transitionTime)
        {
            timeElapsed += Time.deltaTime;
            camera.position =   Vector3.MoveTowards(camera.position, originalCameraPos, Time.deltaTime*cameraMoveSpeed);
            camera.rotation =   Quaternion.Lerp (camera.rotation, originalCameraRotation, Time.deltaTime*cameraMoveSpeed/5);
            yield return null;
        }

        camera.position = originalCameraPos;
        camera.rotation = originalCameraRotation;
        moveScript.enabled = true;
        camScript.enabled = true;
        yield return null;
    }
}
