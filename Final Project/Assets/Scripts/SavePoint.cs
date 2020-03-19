using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    
    Vector3 spawnPointBox;
    private Vector3 spawnPointPlayer;
    public BoxProperties boxProperties;

    public moveBoy moveBoyScript;
    private Color originalColor;

    private float h, s, v;
    // Start is called before the first frame update
    private Renderer rend;
    public static int currentCheckpoint=0;
    public int checkPointNumber;
    public float originalBrightness, activationBrightness, finalBrightness, decayRate;
    public bool customStart;

    public magneticAttractor[] magnetScripts;

    public gameManager gmScript;

    public GameObject boxPositionTeleport;

    private Vector3 teleportScale;

    public Vector3 boxOffSet;
    //public bool resetStuckBox;
    void Awake()
    {
        if (customStart)
        {
            currentCheckpoint = checkPointNumber;
            
        }

        if (gmScript == null)
        {
            gmScript = FindObjectOfType<gameManager>();
        }
    }
    void Start()
    {
        teleportScale = boxPositionTeleport.transform.localScale;
        magnetScripts = FindObjectsOfType<magneticAttractor>();
        rend = GetComponent<Renderer>();
        Color tempColour;
        
        Color.RGBToHSV(originalColor, out h, out s, out v);
        v = originalBrightness;
        rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
        
        
       // moveBoyScript = FindObjectOfType<moveBoy>();
       // boxProperties = FindObjectOfType<BoxProperties>(); //find object of type is buggy set everything in inspector instead
     
        
        spawnPointBox = new Vector3(transform.position.x,transform.position.y,0);
        spawnPointPlayer = new Vector3(transform.position.x - 1f, transform.position.y, 0);
        if (checkPointNumber == currentCheckpoint)
        {
            SpawnPlayer(spawnPointPlayer);
            boxProperties.Apparate(spawnPointBox.x, spawnPointBox.y);
            
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
           // global::magneticAttractor.resetBoxBool = true;
          // boxProperties.gameObject.SetActive(false);
          StartCoroutine(RespawnBox());
          /* boxProperties.magTransform = null;
            boxProperties.stuck = false;
             boxProperties.Apparate(spawnPointBox.x, spawnPointBox.y);
             boxProperties.magTransform = null;
             boxProperties.stuck = false;
             for (int i = 0; i < magnetScripts.Length; i++)
             {
                 magnetScripts[i].gameObject.SendMessage("ResetBox");
             }
             
             */

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
            if (checkPointNumber != currentCheckpoint || customStart)
            {
                StartCoroutine(Initiate());
                customStart = false;
                gmScript.StoreProgress();
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
       
    }

    IEnumerator RespawnBox()
    {
        GameObject box = boxProperties.gameObject;
        boxPositionTeleport.transform.localScale = Vector3.zero;
        Vector3 boxPos = box.transform.position;
        box.GetComponent<Rigidbody>().isKinematic = true;
        boxPositionTeleport.transform.position = box.transform.position + boxOffSet;
        boxPositionTeleport.SetActive(true);
        while (boxPositionTeleport.transform.localScale.magnitude < teleportScale.magnitude)
        {
            boxProperties.energy += 300f*Time.deltaTime;
            boxPositionTeleport.transform.localScale = Vector3.MoveTowards(boxPositionTeleport.transform.localScale,
                teleportScale, Time.deltaTime*50f);
            yield return null;
        }

        boxProperties.energy = 1000f;
        yield return new WaitForSeconds(0.5f);
        boxProperties.energy = 150f;
        boxPositionTeleport.SetActive(false);
        boxProperties.magTransform = null;
        boxProperties.stuck = false;
        boxProperties.Apparate(spawnPointBox.x, spawnPointBox.y);
        boxProperties.magTransform = null;
        boxProperties.stuck = false;
        for (int i = 0; i < magnetScripts.Length; i++)
        {
            magnetScripts[i].gameObject.SendMessage("ResetBox");
        }

        print("Teleport complete");
        yield return null;
    }
}
