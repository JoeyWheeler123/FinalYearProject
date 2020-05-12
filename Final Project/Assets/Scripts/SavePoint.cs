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
    private Vector3 boxScale;
    public Vector3 boxOffSet;

    private bool currentlyRespawning;
    public bool warpOn;
    public Transform newWarpPoint;
    public GameObject particles;
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
        currentlyRespawning = false;
        teleportScale = boxPositionTeleport.transform.localScale;
        boxScale = Vector3.one * 1.5f;
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
        if (moveBoyScript.thrown&&!currentlyRespawning)
        {
           // global::magneticAttractor.resetBoxBool = true;
          // boxProperties.gameObject.SetActive(false);
          StartCoroutine(RespawnBox());
          /*boxProperties.magTransform = null;
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
        if (warpOn)
        {
            transform.position = newWarpPoint.position;
            spawnPointBox = new Vector3(transform.position.x, transform.position.y, 0);
            SpawnPlayer(newWarpPoint.position);
            warpOn = false;
           
        }

    }

    void SpawnPlayer(Vector3 spawnPosition)
    {
        moveBoyScript.gameObject.transform.position = spawnPosition;
       // Activate();
        StartCoroutine(Initiate());
    }
    void UnstickBox()
    {
        boxProperties.magTransform = null;
        boxProperties.stuck = false;
        boxProperties.Apparate(spawnPointBox.x, spawnPointBox.y);
        
        boxProperties.magTransform = null;
        boxProperties.stuck = false;
        for (int i = 0; i < magnetScripts.Length; i++)
        {
            magnetScripts[i].gameObject.SendMessage("ResetBox");
        }
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
            
           // print(v);
            yield return null;
            
        }
       
        yield return null;
       
    }
    void WarpEngaged()
    {
        warpOn=true;
        if (particles != null)
        {
            particles.SetActive(true);
        }
    }
    IEnumerator RespawnBox()
    {
       
        currentlyRespawning = true;
        GameObject box = boxProperties.gameObject;
        Rigidbody boxRb = box.GetComponent<Rigidbody>();
       boxPositionTeleport.transform.localScale = Vector3.zero;
        Vector3 boxPos = box.transform.position;
        boxRb.isKinematic = true;
        boxPositionTeleport.transform.position = box.transform.position + boxOffSet;
        boxPositionTeleport.SetActive(true);
        while (boxPositionTeleport.transform.localScale.magnitude < teleportScale.magnitude)
        {
            boxProperties.energy += 300f*Time.deltaTime;
            boxPositionTeleport.transform.localScale = Vector3.MoveTowards(boxPositionTeleport.transform.localScale,
                teleportScale, Time.deltaTime*100f);
            yield return null;
        }

        boxProperties.energy = 1000f;
        float timeElapsed = 0;
        while (timeElapsed <= 0.3f)
        {
            box.transform.localScale =
                Vector3.MoveTowards(box.transform.localScale, new Vector3(0.1f,0.1f,0.1f), Time.deltaTime * 10f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        boxProperties.energy = 150f;
        
       // boxPositionTeleport.SetActive(false);
        boxProperties.magTransform = null;
        boxProperties.stuck = false;
        boxProperties.Apparate(spawnPointBox.x, spawnPointBox.y);
        box.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        timeElapsed = 0;
        box.GetComponent<Rigidbody>().isKinematic = true;
        UnstickBox();

        boxRb.isKinematic = true;
        while (timeElapsed <= 0.5f)
        {
            boxPositionTeleport.transform.localScale = Vector3.MoveTowards(boxPositionTeleport.transform.localScale,
                Vector3.zero, Time.deltaTime*50f);
            box.transform.localScale =
                Vector3.MoveTowards(box.transform.localScale, boxScale, Time.deltaTime * 5f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        boxPositionTeleport.SetActive(false);
        boxRb.isKinematic = false;
        boxProperties.transform.localScale = boxScale;

        
        currentlyRespawning = false;
        print("Teleport complete");
        yield return null;
    }
}
