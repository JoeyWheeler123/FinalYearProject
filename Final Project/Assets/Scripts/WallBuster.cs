using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuster : MonoBehaviour
{
    private Vector3 desctructionVector;
    public GameObject[] collisionPieces;

    private Collider thisCollider;

    public float speedToDestroy;
    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
