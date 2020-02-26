﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxProperties : MonoBehaviour
{
    public Renderer rend;

    private Color originalColor;

    private float h, s, v;

    private moveBoy moveScript;

    public ParticleSystem enchant, disenchant;
    public Color newColour;
    private Rigidbody rb;

    public float colourChangeRate;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = rend.material.GetColor("Color_C5A9FA1D");
        rb = GetComponent<Rigidbody>();
        moveScript = FindObjectOfType<moveBoy>();
        //StartCoroutine(SwitchBoxColour(newColour));
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

        Color tempColour = rend.material.GetColor("Color_C5A9FA1D");

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
            print(v);
            yield return null;
            
        }
        
        v = 10f;
        rend.material.SetColor("Color_C5A9FA1D", Color.HSVToRGB(h, s, v));
        yield return new WaitForSeconds(0.5f);
        rend.material.SetColor("Color_C5A9FA1D", originalColor);
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
}