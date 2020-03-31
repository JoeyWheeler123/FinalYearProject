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
    private bool initialActivator;
    public float activationDistance;
    public GameObject holographic;
    private Vector3 initialHolographicScale, shrunkHolographicScale;
    private Quaternion holographicRotationTransform;
    private BoxProperties boxPropertiesScript;
    // Start is called before the first frame update
    void Start()
    {
        boxPropertiesScript = box.GetComponent<BoxProperties>();
        holographicRotationTransform = holographic.transform.rotation;
        initialHolographicScale = Vector3.one;
        shrunkHolographicScale = Vector3.zero;
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
        float distance = Vector3.Distance(rotatePosition.position, box.transform.position);
        if (distance <= activationDistance&&!initialActivator)
        {
            initialActivator = true;
            Activate();
        }
        if (distance >=6f)
        {
            initialActivator = false;
        }
        if (storyOn)
        {
            holographic.transform.localScale = Vector3.MoveTowards(holographic.transform.localScale,
              Vector3.zero, Time.deltaTime *2f);
        }
        else
        {
            holographic.transform.localScale = Vector3.MoveTowards(holographic.transform.localScale,
             Vector3.one, Time.deltaTime * 2f);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("box")&& !moveScript.pressedThrow)
        {
           // Activate();
            print("activate");
        }
    }
    public void Activate()
    {
        if (!storyOn)
        {
            if (moveScript.thrown)
            {
               // holographic.SetActive(false);
                StartCoroutine(StoryActive());
            }
        }
        else
        {
            moveScript.inControl = true;
            //line.enabled = false;
            storyOn = false;
            imageToDisplay.SetActive(false);
            //holographic.SetActive(true);
        }

        
    }

   

    IEnumerator StoryActive()
    {
        //moveScript.inControl = false;
        moveScript.energyFull = false;
        //line.SetPosition(1, imageToDisplay.transform.position);
        storyOn = true;
           
        //moveScript.ResetVelocity();
            
            
        
        moveScript.DropBox();
        throwS.freeMove = true;
        throwS.rb.isKinematic = true;
        throwS.gameObject.GetComponent<Collider>().enabled = false;
        vfx.Play();
        anim.SetBool("activate",true);
        float timeElapsed = 0;
        float theta = 2.3f;
        while (storyOn)
        {
            theta += Time.deltaTime*2f;
            boxPropertiesScript.energy = 100f + (Mathf.Sin(theta) * 50f);
            box.transform.position = Vector3.MoveTowards(box.transform.position,rotatePosition.position,Time.deltaTime*boxMoveSpeed);
            Vector3 rotateAxis = new Vector3(1,1,1);
                
            if (Vector3.Distance(box.transform.position, rotatePosition.position) <= 0.1f&&timeElapsed>=5f)
            {
                imageToDisplay.SetActive(true);
                //line.enabled = true;
                box.transform.Rotate(Vector3.up*Time.deltaTime*50f);
               
                //box.transform.Rotate(Vector3.forward * Time.deltaTime * boxMoveSpeed * 10f);
               // box.transform.Rotate(Vector3.left * Time.deltaTime * boxMoveSpeed * 10f);

            }
            else
            {

                // Determine which direction to rotate towards
                //Vector3 targetDirection = holographicRotationTransform.position - box.position;

                // The step size is equal to speed times frame time.
                //float singleStep = speed * Time.deltaTime;

                // Rotate the forward vector towards the target direction by one step
                Vector3 newDirection = Vector3.RotateTowards(box.transform.forward, holographicRotationTransform.eulerAngles, Time.deltaTime, 0.0f);

                // Draw a ray pointing at our target in
                Debug.DrawRay(transform.position, newDirection, Color.red);

                // Calculate a rotation a step closer to the target and applies rotation to this object
                box.transform.rotation = Quaternion.LookRotation(newDirection);
            }

            if (moveScript.pressedThrow)
            {
                Activate();
                anim.SetBool("activate",false);
               
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
        anim.SetBool("activate", false);

        moveScript.energyFull = true;
        throwS.gameObject.GetComponent<Collider>().enabled = true;
        throwS.freeMove = false;
        throwS.rb.isKinematic = false;
        yield return null;
    }
}
