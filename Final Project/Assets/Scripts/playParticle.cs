using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playParticle : MonoBehaviour
{
    public ParticleSystem jump;

    public void JumpParticle()
    {
        jump.Play();
    }

    public void LandParticle()
    {
        jump.Play();
    }
}
