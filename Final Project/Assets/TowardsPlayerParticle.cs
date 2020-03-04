using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowardsPlayerParticle : MonoBehaviour
{
    public ParticleSystem pSys;
    public GameObject player;
    public float minDistance;
    private bool farEnough;
    private moveBoy moveScript;
    private bool particleTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
       // pSys = GetComponent<ParticleSystem>();
       // pSys.Pause();
       
        player = GameObject.FindGameObjectWithTag("Player");
        moveScript = player.GetComponent<moveBoy>();
        pSys.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveScript.pressedThrow&&!particleTriggered)
        {
            pSys.Play();
            particleTriggered = true;
        }
        if (!moveScript.pressedThrow && particleTriggered)
        {
            pSys.Stop();
            particleTriggered = false;
        }
        else if(!moveScript.thrown && particleTriggered)
        {
            pSys.Stop();
            particleTriggered = false;
        }
        Vector3 towardsPos = player.transform.position - transform.position;
        Vector3 newDirection= Vector3.RotateTowards(transform.forward, towardsPos, Time.deltaTime * 50f, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
