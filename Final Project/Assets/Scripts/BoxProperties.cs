using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxProperties : MonoBehaviour
{
    public Renderer rend;

    private Color originalColor;

    private float h, s, v;

    private moveBoy moveScript;

    public ParticleSystem enchant, disenchant;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveScript = FindObjectOfType<moveBoy>();
        //StartCoroutine(Recharge());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Apparate(float xPos,float yPos)
    {
        rb.isKinematic = true;
        rb.isKinematic = false;
        transform.position = new Vector3(xPos, yPos, 0);
    }
    IEnumerator Recharge()
    {
        enchant.Play();
        originalColor = rend.material.GetColor("Color_C5A9FA1D");
        Color tempColour;

        Color.RGBToHSV(originalColor, out h, out s, out v);
        print(v);
        float originalV = v;
        float timeElapsed=0;
        v = 0.15f;
        while (timeElapsed <= moveScript.energyRechargeTime)
        {
            
            rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
            v += ( Time.deltaTime*0.85f)/moveScript.energyRechargeTime;
            timeElapsed += Time.deltaTime;
            print(v);
            yield return null;
            
        }
        
        v = 10f;
        rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
        yield return new WaitForSeconds(0.5f);
        rend.material.SetColor("Color_C5A9FA1D", originalColor);
        yield return null;
    }
}
