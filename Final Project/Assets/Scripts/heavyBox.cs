using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heavyBox : MonoBehaviour
{
    private BoxCollider boxCollider;

    public GameObject player;

    public GameObject box;

    private Color normalColor;

    public Color heavyColor;

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
    // Start is called before the first frame update
    void Start()
    {
        GlobalHeavyBoxCheck = false;
        moveScript = player.GetComponent<moveBoy>();
        rBox = box.GetComponent<Rigidbody>();
        heavyActive = false;

        normalForce = player.GetComponent<moveBoy>().inverseRecallMultiplier;

        boxCollider = GetComponent<BoxCollider>();

        normalSpeed = player.GetComponent<moveBoy>().speed;

        normalJump = player.GetComponent<moveBoy>().jumpHeight;

        boxMass = box.GetComponent<Rigidbody>().mass;

        normalColor = box.GetComponent<MeshRenderer>().material.color;
        
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

        if (Input.GetKey(KeyCode.E))
        {
            Normal();
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.CompareTag("Player"))
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
        //box.GetComponent<Material>().color = heavyColor;
        box.GetComponent<MeshRenderer>().material.color = heavyColor;
    }

    public void Normal()
    {
        GlobalHeavyBoxCheck = false;
        heavyActive = false;
        moveScript.heavyPull = true;
        moveScript.speed = normalSpeed;
        moveScript.jumpHeight = normalJump;
        moveScript.inverseRecallMultiplier = normalForce;
        moveScript.heavy = false;
        rBox.mass = boxMass;
        box.GetComponent<MeshRenderer>().material.color = normalColor;
    }
}
