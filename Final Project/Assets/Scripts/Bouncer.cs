﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class Bouncer : MonoBehaviour
{
    public float defaultBounceStrength =25f;

    public float overallModifier = 35f;

    public GameObject box;

    public Renderer emissiveRenderer;

    public Color bounceColour;

    private BoxProperties boxProperties;

    private Coroutine changeColour;

    private moveBoy moveScript;
    public VisualEffect bounceEffect;
    // Start is called before the first frame update
    void Start()
    {
        if (box == null)
        {
            box = GameObject.FindGameObjectWithTag("box");
        }
        boxProperties = box.GetComponent<BoxProperties>();

        moveScript = boxProperties.moveScript;
        //rend = GetComponent<Renderer>();
        bounceColour = emissiveRenderer.material.GetColor("_EmissiveColor");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("box"))
        {
            //print("BOUNCY BOX");
           // box = other.gameObject;
            Rigidbody boxRb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 collisionDir = other.contacts[0].normal;
            if (changeColour != null)
            {
                box.gameObject.SendMessage("ResetBounce", bounceColour);
                boxProperties.StopCoroutine(changeColour);
               // boxProperties.StopCoroutine("SwitchBoxColour");
            }
            box.gameObject.SendMessage("ResetRecharge");
           
            if (!global::heavyBox.GlobalHeavyBoxCheck)
            {
                changeColour= boxProperties.StartCoroutine("SwitchBoxColour", bounceColour);
            }

            boxRb.isKinematic = true;
            boxRb.isKinematic = false;
            
            print(other.relativeVelocity.magnitude);
            float bounceHeight = defaultBounceStrength + (other.relativeVelocity.magnitude * overallModifier);
            Vector3 bounceDir = transform.up;
           bounceDir= bounceDir* bounceHeight;
            //boxRb.velocity = new Vector3(boxRb.velocity.x,bounceHeight,boxRb.velocity.z);
            boxRb.velocity = bounceDir;
            boxProperties.energy = 150;
            if (moveScript != null)
            {
                moveScript.energyFull = true;
            }
            else
            {
                moveScript = boxProperties.moveScript;
            }
            bounceEffect.Play();
           // boxRb.AddForce((Vector3.up*overallModifier*other.relativeVelocity.magnitude));
            //(Vector3.up*overallModifier*defaultBounceStrength)
            StartCoroutine(RotateBox());
        }
    }

    IEnumerator RotateBox()
    {
        float timeElapsed = 0;
        Quaternion rot = Quaternion.identity;
        while(timeElapsed <= 1f)
        {
            box.transform.rotation =
                Quaternion.Slerp(box.transform.rotation, rot,Time.deltaTime*4f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
