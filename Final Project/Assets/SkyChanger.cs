using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class SkyChanger : MonoBehaviour
{
    public bool changing;
    public GradientSky gSky;
    private Color colourTop, colourMiddle, colourBottom;
    public Color newColourTop, newColourMiddle, newColourBottom;
    public float changeSpeed;
    // Start is called before the first frame update
    void Start()
    {
        colourTop = gSky.top.value;
        colourMiddle = gSky.middle.value;
        colourBottom = gSky.bottom.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (changing)
        {
            gSky.top.value = Color.Lerp(colourTop, newColourTop, Time.deltaTime*changeSpeed);
            gSky.middle.value = Color.Lerp(colourMiddle, newColourMiddle, Time.deltaTime * changeSpeed);
            gSky.bottom.value = Color.Lerp(colourBottom, newColourBottom, Time.deltaTime * changeSpeed);

            colourTop = gSky.top.value;
            colourMiddle = gSky.middle.value;
            colourBottom = gSky.bottom.value;
        }
    }
}
