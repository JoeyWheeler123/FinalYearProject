using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InterruptableAnimation : MonoBehaviour
{
    private PlayerControls controls;
    public UnityEvent playerSleeping, returnControl;
    public Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Jump.performed += ctx => Interrupt();
        playerSleeping.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Interrupt()
    {
        print("iNTERRUPT");
        anim.SetBool("Waking", true);
        StartCoroutine(ReturnControl());
    }

    IEnumerator ReturnControl()
    {
        yield return new WaitForSeconds(12f);
        returnControl.Invoke();
        yield return null;
    }
}
