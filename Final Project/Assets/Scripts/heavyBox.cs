using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heavyBox : MonoBehaviour
{
    private BoxCollider boxCollider;

    public GameObject player;

    public GameObject box;

    public GameObject rendererBox;

    public Color heavyColor;

    public Color normalColor;

    public ParticleSystem disenchant;

    public ParticleSystem enchant;

    public float boxMass;

    float normalForce;
    public float inverseForce = 3f;

    public float fat = 5f;

    private float normalSpeed; 
    public float heavySpeed = 6f;

    private float normalJump;
    public float heavyJump = 2f;

    public bool heavyActive;

    private moveBoy moveScript;

    private Rigidbody rBox;

    public static bool GlobalHeavyBoxCheck;

    public bool experimentalRecharge;

    public float energyDrainRate;

    

    private BoxProperties boxProperties;
    // Start is called before the first frame update
    void Start()
    {
        boxProperties = FindObjectOfType<BoxProperties>();
        normalColor = boxProperties.GetOriginalColour(normalColor);
        GlobalHeavyBoxCheck = false;
        moveScript = FindObjectOfType<moveBoy>();
        rBox = box.GetComponent<Rigidbody>();
        heavyActive = false;

        normalForce = player.GetComponent<moveBoy>().inverseRecallMultiplier;

        boxCollider = GetComponent<BoxCollider>();

        normalSpeed = player.GetComponent<moveBoy>().speed;

        normalJump = player.GetComponent<moveBoy>().jumpHeight;

        boxMass = box.GetComponent<Rigidbody>().mass;

        //rendererBox = box.GetComponent<Renderer>();

        //moveScript = player.GetComponent<moveBoy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (heavyActive && !moveScript.thrown)
        {
            moveScript.speed = heavySpeed;
            moveScript.jumpHeight = heavyJump;
            moveScript.heavy = true;
           
        }

        if(heavyActive && moveScript.thrown)
        {
            moveScript.speed = normalSpeed;
            moveScript.jumpHeight = normalJump;
            moveScript.heavy = false;
            
        }

        if (Input.GetKey(KeyCode.E) && heavyActive == true)
        {
           // Normal();
        }

/*        if (moveScript.pressedThrow&&heavyActive&&experimentalRecharge)
        {
            boxProperties.energy -= energyDrainRate*Time.deltaTime;
            if (boxProperties.energy <= 0)
            {
                moveScript.SuperPull();
            }
        }
        else
        {
           // boxProperties.energy += energyDrainRate*Time.deltaTime;
        }
        */
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.CompareTag("Player") && heavyActive != true)
        {
            Debug.Log("HIT");
            Heavy();
        }

        /*if(coll.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            heavy = false;
            boxMass = 1.5f;
        }*/
    }

    public void Heavy()
    {
        GlobalHeavyBoxCheck = true;
        heavyActive = true;
        moveScript.heavyPull = true;
        player.GetComponent<moveBoy>().heavy = true;
        player.GetComponent<moveBoy>().inverseRecallMultiplier = inverseForce;
        //boxMass = 5f;
        box.GetComponent<Rigidbody>().mass = fat;
        enchant.Play();
        rendererBox.GetComponent<Renderer>().material.SetColor("Color_C5A9FA1D", heavyColor);
        boxProperties.SetOriginalColour(heavyColor);
    }

    public void Normal()
    {
        GlobalHeavyBoxCheck = false;
        heavyActive = false;
        moveScript.heavyPull = false;
        moveScript.speed = normalSpeed;
        moveScript.jumpHeight = normalJump;
        moveScript.inverseRecallMultiplier = normalForce;
        moveScript.heavy = false;
        rBox.mass = boxMass;
        disenchant.Play();
        rendererBox.GetComponent<Renderer>().material.SetColor("Color_C5A9FA1D", normalColor);
        boxProperties.SetOriginalColour(normalColor);
    }

    public void Activate()
    {
        if (heavyActive)
        {
            Normal();
        }
        else
        {
            Heavy();
        }
    }
}
