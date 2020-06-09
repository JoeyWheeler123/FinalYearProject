using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SkyChanger : MonoBehaviour
{
    public bool changing;
    private GradientSky changingSkybox, initialSkybox, endSkybox;
    private Color colourTop, colourMiddle, colourBottom;
   private Color newColourTop, newColourMiddle, newColourBottom;
    public float changeSpeed, changeTime;
    public float exposureChangeSpeed;
    public float initialDelay;
    [SerializeField] private VolumeProfile changingSkyboxProfile, initialSkyboxProfile, endSkyboxProfile;
    public static SkyChanger instance;

    // Start is called before the first frame update
    void Start()
    {
        changingSkyboxProfile.TryGet(out changingSkybox);
        initialSkyboxProfile.TryGet(out initialSkybox);
        endSkyboxProfile.TryGet(out endSkybox);
       /* colourTop = gSky.top.value;
        colourMiddle = gSky.middle.value;
        colourBottom = gSky.bottom.value;
        */
        changingSkybox.top.value = initialSkybox.top.value;
        changingSkybox.middle.value = initialSkybox.middle.value;
        changingSkybox.bottom.value = initialSkybox.bottom.value;
        changingSkybox.exposure.value = initialSkybox.exposure.value;
        newColourTop = endSkybox.top.value;
       newColourMiddle = endSkybox.middle.value;
        newColourBottom = endSkybox.bottom.value;

        instance = this;
        StartCoroutine(SkyChange());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (changing)
        {
            colourTop = changingSkybox.top.value;
            colourMiddle = changingSkybox.middle.value;
            colourBottom = changingSkybox.bottom.value;

            changingSkybox.top.value = Color.Lerp(colourTop, newColourTop, Time.deltaTime*changeSpeed);
            changingSkybox.middle.value = Color.Lerp(colourMiddle, newColourMiddle, Time.deltaTime * changeSpeed);
            changingSkybox.bottom.value = Color.Lerp(colourBottom, newColourBottom, Time.deltaTime * changeSpeed);

            
        }
        */
    }

    IEnumerator SkyChange()
    {
        yield return new WaitForSeconds(initialDelay);
        float timeLeft = changeTime;
       
        while (timeLeft>0)
        {
            colourTop = changingSkybox.top.value;
            colourMiddle = changingSkybox.middle.value;
            colourBottom = changingSkybox.bottom.value;

            changingSkybox.top.value = Color.Lerp(colourTop, newColourTop, Time.deltaTime/timeLeft);
            changingSkybox.middle.value = Color.Lerp(colourMiddle, newColourMiddle, Time.deltaTime/timeLeft);
            changingSkybox.bottom.value = Color.Lerp(colourBottom, newColourBottom, Time.deltaTime/timeLeft);
            float newExposure = Mathf.Lerp(changingSkybox.exposure.value, endSkybox.exposure.value, Time.deltaTime/timeLeft);
            changingSkybox.exposure.value = newExposure;
            timeLeft -= Time.deltaTime;
            Debug.Log(timeLeft);
            yield return new WaitForEndOfFrame();

        }
        yield return null;
    }
}
