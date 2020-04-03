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
    public GameObject player;
    Transform playerTransform;

    [FMODUnity.EventRef]
    public string selectSound;
    FMOD.Studio.EventInstance sound;

    // Start is called before the first frame update

    void Awake()
    {
        controls = new PlayerControls();
        sound = FMODUnity.RuntimeManager.CreateInstance(selectSound);
        playerTransform = player.transform;
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
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound, GetComponent<Transform>(), GetComponent<Rigidbody>()); 

        if (controls.Gameplay.Interact.triggered && playerInRange)
        {
            Interact();
            Playsound();
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

    void Playsound()
    {
        //sound.start();
        FMODUnity.RuntimeManager.PlayOneShot(selectSound, player.transform.position);
    }
}
