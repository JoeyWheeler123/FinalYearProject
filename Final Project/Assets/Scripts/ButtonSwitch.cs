using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    public GameObject door, button;

    public bool resetOnRelease;

    public float doorOpenSpeed, switchSpeed;

    public float pressDistance;

    public Transform doorOpenPos;

    private Vector3 doorInitialPos, switchInitialPos, switchPressedPos;

    public bool opening, closing;
    // Start is called before the first frame update
    void Start()
    {
        switchInitialPos = button.transform.position;
        switchPressedPos = new Vector3(switchInitialPos.x, switchInitialPos.y - pressDistance, switchInitialPos.z);
        doorInitialPos = door.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (opening)
        {
            closing = false;
            door.transform.position = Vector3.MoveTowards(door.transform.position, doorOpenPos.position,
                Time.deltaTime * doorOpenSpeed);
            button.transform.position = Vector3.MoveTowards(button.transform.position, switchPressedPos, Time.deltaTime * switchSpeed);
            if(Vector3.Distance(door.transform.position, doorOpenPos.position) <= 0.1f)
            {
                opening = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("box")||other.gameObject.CompareTag("Player"))
        {
            opening = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("box") || other.gameObject.CompareTag("Player"))
        {
            if (resetOnRelease)
            {
                print("close door");
                    StartCoroutine(CloseDoor());
                
            }
        }
    }

    IEnumerator OpenDoor()
    {
        opening = true;
        closing = false;
        while (Vector3.Distance(door.transform.position, doorOpenPos.position) >= 0.1f&&opening)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, doorOpenPos.position,
                Time.deltaTime * doorOpenSpeed);
            button.transform.position = Vector3.MoveTowards(button.transform.position, switchPressedPos, Time.deltaTime * switchSpeed);
            yield return null;
        }

        opening = false;
        yield return null;
    }

    IEnumerator CloseDoor()
    {
        closing = true;
        float closeTime=0;
        opening = false;
        while (closeTime<1f&&closing)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, doorInitialPos,
                Time.deltaTime * doorOpenSpeed);
            button.transform.position = Vector3.MoveTowards(transform.position, switchInitialPos, Time.deltaTime * switchSpeed);
            closeTime += Time.deltaTime;
            yield return null;
        }

        closing = false;
        yield return null;
    }
    
    
}
