using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    
    Vector3 spawnPointBox;
    private Vector3 spawnPointPlayer;
    private BoxProperties boxProperties;

    private moveBoy moveBoyScript;
    private Color originalColor;

    private float h, s, v;
    // Start is called before the first frame update
    private Renderer rend;
    public static int currentCheckpoint=0;
    public int checkPointNumber;
    public float originalBrightness, activationBrightness, finalBrightness, decayRate;
    public bool customStart;

    void Awake()
    {
        if (customStart)
        {
            currentCheckpoint = checkPointNumber;
            
        }
    }
    void Start()
    {
        
        rend = GetComponent<Renderer>();
        Color tempColour;
        
        Color.RGBToHSV(originalColor, out h, out s, out v);
        v = originalBrightness;
        rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
        
        
        moveBoyScript = FindObjectOfType<moveBoy>();
        boxProperties = FindObjectOfType<BoxProperties>();
        
        spawnPointBox = new Vector3(transform.position.x,transform.position.y+3f,0);
        spawnPointPlayer = new Vector3(transform.position.x - 1f, transform.position.y, 0);
        if (checkPointNumber == currentCheckpoint)
        {
            SpawnPlayer(spawnPointPlayer);


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void Activate()
    {
        if (moveBoyScript.thrown)
        {
            boxProperties.Apparate(spawnPointBox.x, spawnPointBox.y);
        }

    }

    void SpawnPlayer(Vector3 spawnPosition)
    {
        moveBoyScript.gameObject.transform.position = spawnPosition;
        Activate();
        StartCoroutine(Initiate());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (checkPointNumber != currentCheckpoint)
            {
                StartCoroutine(Initiate());
            }
        }
    }

    
    IEnumerator Initiate()
    {
       // boxProperties.gameObject.SetActive(false);
        currentCheckpoint = checkPointNumber;
        originalColor = rend.material.GetColor("Color_C5A9FA1D");
        Color tempColour;

        Color.RGBToHSV(originalColor, out h, out s, out v);
        print(v);
        float originalV = v;
        
        v = activationBrightness;
        while (v>=finalBrightness)
        {
            
            rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
            v -= Time.deltaTime*decayRate;
            
            print(v);
            yield return null;
            
        }
       
        yield return null;
        yield return null;
    }
}
