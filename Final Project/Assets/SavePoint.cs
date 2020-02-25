using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    
    Vector3 spawnPoint;
    private BoxProperties boxProperties;

    private moveBoy moveBoyScript;
    // Start is called before the first frame update
   
    
    void Start()
    {
        moveBoyScript = FindObjectOfType<moveBoy>();
        boxProperties = FindObjectOfType<BoxProperties>();
        
        spawnPoint = new Vector3(transform.position.x,transform.position.y+3f,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void Activate()
    {
        if (moveBoyScript.thrown)
        {
            boxProperties.Apparate(spawnPoint.x, spawnPoint.y);
        }

    }
}
