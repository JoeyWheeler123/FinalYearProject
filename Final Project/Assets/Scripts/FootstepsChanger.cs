using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsChanger : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string footsteps = "event:/3D_Events/RoughFootsteps";
    public FMOD.Studio.EventInstance RoughFootsteps;
    //public FMOD.Studio.PARAMETER_ID outside;

    private void Start()
    {
        RoughFootsteps = FMODUnity.RuntimeManager.CreateInstance(footsteps);
        //RoughFootsteps.setParameterByName("Outside", 1f);
    }

    public void OnTriggerEnter(Collider other)
    {
        RoughFootsteps.setParameterByName("Outside", 0f);
        RoughFootsteps.setParameterByName("Inside", 1f);
    }
    public void OnTriggerExit(Collider other)
    {
        RoughFootsteps.setParameterByName("Outside", 1f);
        RoughFootsteps.setParameterByName("Inside", 0f);
        //footsteps.setParameterByName("Inside", 0f);
    }
}
