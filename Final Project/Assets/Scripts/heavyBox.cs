using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heavyBox : MonoBehaviour
{
    private BoxCollider boxCollider;

    public GameObject player;

    public GameObject box;

    public float boxMass;

    private float normalSpeed; 
    public float heavySpeed = 6f;

    private float normalJump;
    public float heavyJump = 2f;

    public bool heavyActive;


    // Start is called before the first frame update
    void Start()
    {
        heavyActive = false; 

        boxCollider = GetComponent<BoxCollider>();

        normalSpeed = player.GetComponent<moveBoy>().speed;

        normalJump = player.GetComponent<moveBoy>().jumpHeight;

        boxMass = box.GetComponent<Rigidbody>().mass;
        
        //moveScript = player.GetComponent<moveBoy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (heavyActive && !player.GetComponent<moveBoy>().thrown)
        {
            player.GetComponent<moveBoy>().speed = heavySpeed;
            player.GetComponent<moveBoy>().jumpHeight = heavyJump;
            player.GetComponent<moveBoy>().heavy = true;
        }

        if(heavyActive && player.GetComponent<moveBoy>().thrown)
        {
            player.GetComponent<moveBoy>().speed = normalSpeed;
            player.GetComponent<moveBoy>().jumpHeight = normalJump;
            player.GetComponent<moveBoy>().heavy = false;
        }

        if (Input.GetKey(KeyCode.E))
        {
            heavyActive = false;
            player.GetComponent<moveBoy>().speed = normalSpeed;
            player.GetComponent<moveBoy>().jumpHeight = normalJump;
            player.GetComponent<moveBoy>().heavy = false;
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Player" && !player.GetComponent<moveBoy>().thrown)
        {
            Debug.Log("HIT");
            heavyActive = true;
            player.GetComponent<moveBoy>().heavy = true;
            boxMass = 5f;
        }

        /*if(coll.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            heavy = false;
            boxMass = 1.5f;
        }*/
    }
}
