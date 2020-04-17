using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InterruptableAnimation : MonoBehaviour
{
    private PlayerControls controls;
    public UnityEvent playerSleeping, returnControl,beginPopup,destroyPopup;
    public Animator anim;
    private bool jumpPushed=false;
    public float wakingAnimationTime;
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;
        StartCoroutine(Prompt());
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
        jumpPushed = true;
        destroyPopup.Invoke();
    }

    IEnumerator ReturnControl()
    {
        yield return new WaitForSeconds(wakingAnimationTime);
        returnControl.Invoke();
        Destroy(this.gameObject);
        yield return null;
    }
    IEnumerator Prompt()
    {
        yield return new WaitForSeconds(2f);
        if (!jumpPushed)
        {
            beginPopup.Invoke();
        }
        yield return null;
    }
}
