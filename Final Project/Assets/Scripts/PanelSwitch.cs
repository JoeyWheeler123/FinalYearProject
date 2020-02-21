using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitch : MonoBehaviour
{
    private PlayerControls controls;
    public GameObject objectToActivate;
    public bool tutorialObject;
    private bool playerInRange;
    public TutorialPopUp tutorialScript;
    private float backUpTime;
    // Start is called before the first frame update

    void Awake()
    {
        controls = new PlayerControls();
    }
    void Start()
    {
        
    }

    private void OnEnable()
    {
        controls.Gameplay.Interact.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Interact.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (controls.Gameplay.Interact.triggered && playerInRange)
        {
            Interact();
        }

        backUpTime -= Time.deltaTime;
        if (backUpTime <= 0)
        {
            playerInRange = false;
        }
    }

    public void Interact()
    {
       // print("Interact");
        objectToActivate.SendMessage("Activate");
        
        if (tutorialObject)
        {
            tutorialScript.actionCompleted = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            backUpTime = 1f;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
