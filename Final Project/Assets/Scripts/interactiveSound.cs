using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactiveSound : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string selectSound;
    FMOD.Studio.EventInstance sound;

    public GameObject player;
    Transform playerTransform;

   // public KeyCode key;

    private void Awake()
    {
        sound = FMODUnity.RuntimeManager.CreateInstance(selectSound);
        playerTransform = player.transform;
    }

    private void Update()
    {
       // FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound, GetComponent<Transform>(), GetComponent<Rigidbody>());
       // Playsound();
    }

    void Playsound()
    {
        //sound.start();
        FMODUnity.RuntimeManager.PlayOneShot(selectSound, this.gameObject.transform.position);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Playsound();
    }
}
