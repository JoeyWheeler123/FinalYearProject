using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class StoryMonument : MonoBehaviour
{
    public moveBoy moveScript;

    private bool storyOn;

    //public LineRenderer line;

    public GameObject imageToDisplay;

    public GameObject box;

    public throwScript throwS;

    public float boxMoveSpeed;

    public Transform rotatePosition;

    public VisualEffect vfx;
    
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        imageToDisplay.SetActive(false);
        vfx.Stop();
        box = GameObject.FindWithTag("box");
        throwS = FindObjectOfType<throwScript>();
        //line.enabled = false;
        //line.positionCount = 2;
       // line.SetPosition(0, rotatePosition.position);
        storyOn = false;
        moveScript = FindObjectOfType<moveBoy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(box.transform.position, rotatePosition.position) <= 0.3f&&moveScript.thrown)
        {
            //StartCoroutine(StoryActive());
        }
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
            //line.enabled = false;
            storyOn = false;
            imageToDisplay.SetActive(false);
        }

        
    }

   

    IEnumerator StoryActive()
    {
        //moveScript.inControl = false;
        moveScript.energyFull = false;
        //line.SetPosition(1, imageToDisplay.transform.position);
        storyOn = true;
           
        moveScript.ResetVelocity();
            
            
        
        moveScript.DropBox();
        throwS.freeMove = true;
        throwS.rb.isKinematic = true;
        throwS.gameObject.GetComponent<Collider>().enabled = false;
        vfx.Play();
        anim.SetTrigger("activate");
        float timeElapsed = 0;
        while (storyOn)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position,rotatePosition.position,Time.deltaTime*boxMoveSpeed);
            Vector3 rotateAxis = new Vector3(1,1,1);
                
            if (Vector3.Distance(box.transform.position, rotatePosition.position) <= 0.1f&&timeElapsed>=5f)
            {
                imageToDisplay.SetActive(true);
                //line.enabled = true;
                box.transform.Rotate(Vector3.up*Time.deltaTime*boxMoveSpeed*10f);
               
            }

            if (moveScript.pressedThrow)
            {
                Activate();
                anim.SetTrigger("deactivate");
               
            }
            timeElapsed += Time.deltaTime;
               yield return null;
        }
        vfx.Stop();
        timeElapsed=0;
        Vector3 returnPos = new Vector3(box.transform.position.x,box.transform.position.y,0);
        while(timeElapsed <=0.5f)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, returnPos, Time.deltaTime*boxMoveSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetTrigger("deactivate");
        
        moveScript.energyFull = true;
        throwS.gameObject.GetComponent<Collider>().enabled = true;
        throwS.freeMove = false;
        throwS.rb.isKinematic = false;
        yield return null;
    }
}
