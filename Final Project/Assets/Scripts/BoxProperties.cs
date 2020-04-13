using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class BoxProperties : MonoBehaviour
{
    public Renderer rend;
    private Renderer rendWrist;
    private Color originalColor;

    private float h, s, v;

    public moveBoy moveScript;

    public ParticleSystem enchant, disenchant;
    public Color newColour;
    private Rigidbody rb;

    public float colourChangeRate;

    public float energy;

    public float energyPullMultiplier;

    public Coroutine rechargeCoroutine;
    public Coroutine bounceCoroutine;
    public bool stuck;
    public float snapRate,rechargeRate;
    public Vector3 magnetPos;
    public Transform magTransform;
    public bool recharging;
    public GameObject wandererWrist1,wandererWrist2;
    public GameObject ledgeLeft, ledgeRight;
    Vector3 initialWristBandSize;
    private bool particleSwitch;
    public GameObject collectibleIcon1, collectibleIcon2, collectibleIcon3;
    // Start is called before the first frame update
    public static int orbsCollected, totalOrbs;
  
    void Awake()
    {
        orbsCollected = 0;
        rb = GetComponent<Rigidbody>();
        originalColor = rend.material.GetColor("Color_C5A9FA1D");
        rb = GetComponent<Rigidbody>();
       
        
    }
    void Start()
    {
        energy = 100;

        wandererWrist1.gameObject.GetComponent<VisualEffect>().Stop();
        wandererWrist2.gameObject.GetComponent<VisualEffect>().Stop();

        //StartCoroutine(SwitchBoxColour(newColour));
        //StartCoroutine(Recharge());
    }

    // Update is called once per frame
    void Update()
    {
      
        if (collectibleIcon1 != null)
        {
            CollectibleObtained();
        }
        if (energy <= 0&&moveScript.energyFull)
        {
            moveScript.StartCoroutine("BoxCoolDown");
        }
        if (energy <= 100&&!moveScript.pressedThrow)
        {
            energy += Time.deltaTime * rechargeRate;
        }
        if (energy >= 100)
        {
            energy -= Time.deltaTime*rechargeRate;
        }

        if (moveScript.recalling&&!particleSwitch)
        {
            particleSwitch = true;
            
            wandererWrist1.gameObject.GetComponent<VisualEffect>().Play();
            wandererWrist2.gameObject.GetComponent<VisualEffect>().Play();
            // WristBandGrow();
        }
        if(!moveScript.recalling && particleSwitch)
        {
            particleSwitch = false;
            wandererWrist1.gameObject.GetComponent<VisualEffect>().Stop();
            wandererWrist2.gameObject.GetComponent<VisualEffect>().Stop();
        }
            Color tempColour = rend.material.GetColor("Color_C5A9FA1D");

            Color.RGBToHSV(tempColour, out h, out s, out v);
           // print(v);
            v = energy / 100;
            
          
           

                rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
                if (rendWrist != null)
                {
                    rendWrist.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
                    //print("updating");
                }
               
             
               

           
        
        if(stuck == true&&magTransform!=null)
        {
            
             Vector3 newPos = Vector3.MoveTowards(transform.position, magTransform.position, Time.deltaTime * snapRate);
             //rb.MovePosition(newPos);
            transform.position = newPos;

            energy = 150;
            
          
        }
        else
        {
            ledgeLeft.SetActive(false);
            ledgeRight.SetActive(false);
        }
        //print(stuck);
    }

    public void Apparate(float xPos,float yPos)
    {
        rb.isKinematic = true;
        rb.isKinematic = false;
        transform.position = new Vector3(xPos, yPos, 0);
    }

    public void SetOriginalColour(Color newColour)
    {
        originalColor = newColour;
    }

    public void MagnetStuck(Transform newTransform)
    {
        magTransform = newTransform;
        stuck = true;
        rb.isKinematic = true;
        
        StartCoroutine(RotateBox());
        ledgeLeft.SetActive(true);
        ledgeRight.SetActive(true);
       
    }
 
    public Color GetOriginalColour(Color outgoingColour)
    {
        outgoingColour = originalColor;
        return outgoingColour;
    }

    public Color GetCurrentColour(Color outgoingColour)
    {
        outgoingColour = rend.material.GetColor("Color_C5A9FA1D");
        return outgoingColour;
    }
    public void ResetRecharge()
    {
       if (rechargeCoroutine != null)
        {
            //StopCoroutine(rechargeCoroutine);
        }
        
    }
    public void ResetBounce(Color newColour)
    {
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
        }
        if (!global::heavyBox.GlobalHeavyBoxCheck)
        {
            bounceCoroutine = StartCoroutine("SwitchBoxColour", newColour);
        }
    }
   
    public void CollectibleObtained()
    {
        /*
        if (orbsCollected == 1)
        {
            collectibleIcon1.SetActive(true);
        }
        if (orbsCollected == 2)
        {
            collectibleIcon2.SetActive(true);
        }
        if (orbsCollected == 3)
        {
            collectibleIcon3.SetActive(true);
        }
        */
       
        
       
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && stuck&&moveScript.grounded)
        {
            other.gameObject.transform.SetParent(this.transform);

        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null);
            other.gameObject.transform.localScale= new Vector3(1,1,1);
            other.gameObject.transform.localRotation =Quaternion.Euler(0,0,0);
        }
    }

    IEnumerator Recharge()
    {
        enchant.Play();

        /*Color tempColour = rend.material.GetColor("Color_C5A9FA1D");

        Color.RGBToHSV(tempColour, out h, out s, out v);
        print(v);
        float originalV = v;
        float timeElapsed=0;
        v = 0.15f;
        while (timeElapsed <= moveScript.energyRechargeTime)
        {
            
            rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
            v += ( Time.deltaTime*0.85f)/moveScript.energyRechargeTime;
            timeElapsed += Time.deltaTime;
//            print(v);
            yield return null;
            
        }
        
        energy = 100;
        v = 10f;
        rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
        yield return new WaitForSeconds(0.5f);
        rend.material.SetColor("Color_C5A9FA1D", originalColor);
        yield return null;
        */
        float timeElapsed = 0;
        while (timeElapsed <= moveScript.energyRechargeTime)
        {

            energy = timeElapsed*100f / moveScript.energyRechargeTime;
            timeElapsed += Time.deltaTime;
            //            print(v);
            yield return null;

        }
        rend.material.SetColor("Color_C5A9FA1D", originalColor);
        energy = 200f;
       
        yield return null;
    }

    IEnumerator SwitchBoxColour(Color newColour)
    {
       
        float newH;
        Color tempColour;
        Color.RGBToHSV(newColour, out newH, out s, out v);
        Color.RGBToHSV(originalColor, out h, out s, out v);
        newColour = Color.HSVToRGB(newH, s, v);
       // h = newH;
        float timeElapsed=0;
        //float hueMagnitudeDifference;
        //hueMagnitudeDifference = (Mathf.Abs(Mathf.Abs(newH) - Mathf.Abs(h)));
        rend.material.SetColor("Color_C5A9FA1D", newColour);
       yield return new WaitForSeconds(1f);
        while (timeElapsed<=1.5f)
        {
            
            //rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(newH, s, v));
            newColour = Color.Lerp(newColour, originalColor, Time.deltaTime * colourChangeRate);
            rend.material.SetColor("Color_C5A9FA1D", newColour);

            
            timeElapsed += Time.deltaTime;
            //print(v);
            yield return null;
            
        }
        
        rend.material.SetColor("Color_C5A9FA1D", originalColor);
        print("completed");
        yield return null;
    }

    IEnumerator RotateBox()
    {
        float timeElapsed = 0;
        Quaternion rot = Quaternion.identity;
        while (timeElapsed <= 4f)
        {
           transform.rotation =
                Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
