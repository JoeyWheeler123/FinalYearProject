using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtPos : MonoBehaviour
{
    public GameObject playTarget;

    void PlaySound(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path, playTarget.transform.position);
    }
}
