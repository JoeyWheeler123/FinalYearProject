using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactiveSound : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string selectSound;
    FMOD.Studio.EventInstance sound;

    public KeyCode key;

    private void Start()
    {
        sound = FMODUnity.RuntimeManager.CreateInstance(selectSound);
    }

    private void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound, GetComponent<Transform>(), GetComponent<Rigidbody>());
        Playsound();
    }

    void Playsound ()
    {
        if (Input.GetKeyDown(key))
        {
            FMOD.Studio.PLAYBACK_STATE state;
            sound.getPlaybackState(out state);
            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                sound.start();
            }
        }
        if (Input.GetKeyUp(key))
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
