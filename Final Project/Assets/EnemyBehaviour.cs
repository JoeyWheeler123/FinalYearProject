using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject box;

    public float aggroRadius;

    public int aggressionLevel;

    public float velocityToDestroy,
        minVelocitySafe,
        velocityKnockbackMultiplier,
        recoverySpeed,
        killDistance,
        boxGrabDistance;

    private Vector3 towardsBox, newDirection;

    private Rigidbody rb;

    private bool stunned, charging;

    public GameObject heart;

    private Quaternion originalRotation;

    private bool rotating;

    public Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend.material.SetColor("Color_C5A9FA1D",Color.red);
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        box=GameObject.FindWithTag("box");
    }

    // Update is called once per frame
    void Update()
    {
       float distanceToBox = Vector3.Distance(box.transform.position, transform.position);
        if (distanceToBox <= aggroRadius&&distanceToBox>=boxGrabDistance&&!stunned)
        {
            aggressionLevel = 1;
        }
        
       else if (distanceToBox <= boxGrabDistance)
        {
            aggressionLevel = 2;
        }

        else
        {
            
            
        }

        if (aggressionLevel >= 1&&!charging)
        {
            towardsBox = box.transform.position - transform.position;
            towardsBox.Normalize();
            
            newDirection  = Vector3.MoveTowards(transform.position, box.transform.position, Time.deltaTime);
            rb.MovePosition(newDirection);
            
            if (charging)
            {
                rb.isKinematic = false;
                rend.material.SetColor("Color_C5A9FA1D",Color.white);
                charging = false;
                StopCoroutine(GrabBox());
            }
        }
        else if (aggressionLevel == 2&&!charging)
        {
            //StartCoroutine("GrabBox");
        }

        if (aggressionLevel < 2&&charging)
        {
            rb.isKinematic = false;
           
            rend.material.SetColor("Color_C5A9FA1D",Color.red);
            charging = false;
            StopCoroutine("GrabBox");
        }
        
        
    }

    private void FixedUpdate()
    {
        if (rotating)
        {
            Quaternion rot = Quaternion.RotateTowards(transform.rotation, originalRotation, Time.deltaTime * 120f);
            rb.MoveRotation(rot);
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("box") && !stunned)
        {
            if (other.relativeVelocity.magnitude >= minVelocitySafe)
            {
                StartCoroutine(Recovery());
            }
            else if(!charging)
            {
                StartCoroutine("GrabBox");
            }
        }
        else if(!stunned)
        {
            rb.isKinematic = true;
            rb.isKinematic = false;
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        
    }

    IEnumerator Recovery()
    {
        rotating = false;
        rend.material.SetColor("Color_C5A9FA1D",Color.white);
        originalRotation = Quaternion.identity;
        stunned = true;
        aggressionLevel = 0;
        
        Vector3 recoveryPosition = transform.position;
        heart.transform.position = recoveryPosition;
        yield return new WaitForSeconds(0.2f);
        heart.SetActive(true);
       
       yield return new WaitForSeconds(1f);
       if (Vector3.Distance(heart.transform.position, transform.position) >= killDistance)
       {
           Destroy(this.gameObject);
       }
       rb.isKinematic = true;
       //rb.isKinematic = false;
       
       while (Vector3.Distance(heart.transform.position, transform.position) >= 0.5f)
       {
           
           Vector3 towardsHeart = heart.transform.position - transform.position;
           rotating = true;
           towardsHeart.Normalize();
           newDirection = Vector3.MoveTowards(transform.position, heart.transform.position,
               Time.deltaTime * recoverySpeed);
           rb.MovePosition(newDirection);
        
         
           yield return null;
       }

       yield return null;
       rend.material.color = Color.white;
       rb.isKinematic = false;
       stunned = false;
      heart.SetActive(false);
      rend.material.SetColor("Color_C5A9FA1D",Color.red);
     yield return new WaitForSeconds(1f);
     
       //rb.isKinematic = false;
       
        yield return null;
    }

    IEnumerator GrabBox()
    {
        rb.isKinematic = true;
        charging = true;
        rend.material.SetColor("Color_C5A9FA1D",Color.blue);
        yield return new WaitForSeconds(2f);

       
        rb.isKinematic = false;
        box.SetActive(false);
       yield return new WaitForSeconds(2f);
       Scene scene = SceneManager.GetActiveScene(); 
       SceneManager.LoadScene(scene.name);
        yield return null;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(255,255,255,0.2f);
        Gizmos.DrawSphere(transform.position, boxGrabDistance);
    }
}
