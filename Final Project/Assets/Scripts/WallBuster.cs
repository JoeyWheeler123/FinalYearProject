using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuster : MonoBehaviour
{
    private Vector3 desctructionVector;
    public GameObject[] collisionPieces;

    private Collider thisCollider;

    public float speedToDestroy;

    [FMODUnity.EventRef]
    public string selectSound;
    FMOD.Studio.EventInstance sound;

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<Collider>();
        sound = FMODUnity.RuntimeManager.CreateInstance(selectSound);
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }
    public void BeingDestruction()
    {
        StartCoroutine(Destruction());
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody orb = other.gameObject.GetComponent<Rigidbody>();
        if (orb != null)
        {
            if (orb.velocity.magnitude > speedToDestroy)

            {
               StartCoroutine(Destruction());
                desctructionVector = other.gameObject.GetComponent<Rigidbody>().velocity;
            }
        }
    }
    */
    private void OnCollisionEnter(Collision collision)
    {
        print(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude >= speedToDestroy)
        {
            StartCoroutine(Destruction());
            Playsound();
        }
    }
    IEnumerator Destruction()
    {
        thisCollider.enabled = false;
        foreach (GameObject piece in collisionPieces)
        {
          Rigidbody prb = piece.GetComponent<Rigidbody>();
          prb.isKinematic = false;
           // prb.AddForce(desctructionVector*100f);
        }
        yield return new WaitForSeconds(3f);
        foreach (GameObject piece in collisionPieces)
        {
            Destroy(piece);
        }
        yield return null;
    }

    void Playsound()
    {
        //sound.start();
        FMODUnity.RuntimeManager.PlayOneShot(selectSound, this.gameObject.transform.position);
    }
}
