using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class StoryMonument : MonoBehaviour
{
    public moveBoy moveScript;

    private bool storyOn;

    public LineRenderer line;

    public GameObject imageToDisplay;

    public GameObject box;

    public throwScript throwS;

    public float boxMoveSpeed;

    public Transform rotatePosition;
    // Start is called before the first frame update
    void Start()
    {
        box = GameObject.FindWithTag("box");
        throwS = FindObjectOfType<throwScript>();
        line.enabled = false;
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        storyOn = false;
        moveScript = FindObjectOfType<moveBoy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (!storyOn)
        {
            if (!moveScript.thrown)
            {
                StartCoroutine(StoryActive());
            }
        }
        else
        {
            moveScript.inControl = true;
            line.enabled = false;
            storyOn = false;
            imageToDisplay.SetActive(false);
        }

        
    }

    IEnumerator StoryActive()
    {
        //moveScript.inControl = false;
        moveScript.energyFull = false;
        line.SetPosition(1, imageToDisplay.transform.position);
        storyOn = true;
           
        moveScript.ResetVelocity();
            
            
        
        moveScript.DropBox();
        throwS.freeMove = true;
        throwS.rb.isKinematic = true;
        throwS.gameObject.GetComponent<Collider>().enabled = false;
        while (storyOn)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position,rotatePosition.position,Time.deltaTime*boxMoveSpeed);
            Vector3 rotateAxis = new Vector3(1,1,1);
                box.transform.Rotate(Vector3.up*Time.deltaTime*boxMoveSpeed*10f);
            if (Vector3.Distance(box.transform.position, rotatePosition.position) <= 0.1f)
            {
                imageToDisplay.SetActive(true);
                line.enabled = true;
            }

            if (moveScript.pressedThrow)
            {
                Activate();
            }
               yield return null;
        }

        float timeElapsed=0;
        Vector3 returnPos = new Vector3(box.transform.position.x,box.transform.position.y,0);
        while(timeElapsed <=0.5f)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, returnPos, Time.deltaTime*boxMoveSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        moveScript.energyFull = true;
        throwS.gameObject.GetComponent<Collider>().enabled = true;
        throwS.freeMove = false;
        throwS.rb.isKinematic = false;
        yield return null;
    }
}
