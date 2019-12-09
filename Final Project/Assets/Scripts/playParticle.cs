using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playParticle : MonoBehaviour
{
    public ParticleSystem jump;

    public ParticleSystem ledge;

    public ParticleSystem walk;

    public void JumpParticle()
    {
        jump.Play();
    }

    public void LandParticle()
    {
        jump.Play();
    }

    public void LedgeParticle()
    {
        ledge.Play();
    }

    public void WalkParticle()
    {
       walk.Play();
    }
}
