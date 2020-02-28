using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopUp : MonoBehaviour
{
    public GameObject canvas;
    public GameObject imageToDisplay;
    private GameObject player;
    public Transform canvasPosition;
    public bool magnetInteract, recall, jump, wallJump, throwBox, boxJump;
    public bool actionCompleted = false;

    private bool withinRange;

    private float distanceToPlayer;

    public float distanceToActivate;

    private moveBoy moveScript;

    public ButtonSwitch buttonScript;
    // Start is called before the first frame update
    void Start()
    {
       
       player = GameObject.FindWithTag("Player");
       moveScript = FindObjectOfType<moveBoy>();
      
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= distanceToActivate)
        {
            withinRange = true;
        }
        else
        {
            withinRange = false;
        }

        if (withinRange&&!actionCompleted)
        {
           canvas.SetActive(true);
           
            imageToDisplay.SetActive(true);
            canvas.transform.position = canvasPosition.position;

            if (recall)
            {
                if (moveScript.pressedThrow)
                {
                    actionCompleted = true;
                }
            }

            if (jump)
            {
                if (moveScript.pressedJump)
                {
                    actionCompleted = true;
                }
            }

            if (wallJump)
            {
                if (!moveScript.inControl)
                {
                    actionCompleted = true;
                }
            }

            if (throwBox)
            {
                if (buttonScript.opening)
                {
                    actionCompleted=true;
                }
            }

            if (boxJump)
            {
                if (!moveScript.energyFull)
                {
                    actionCompleted = true;
                }
            }
        }
        else
        {
           
            imageToDisplay.SetActive(false);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(255,255,255,0.2f);
        Gizmos.DrawSphere(transform.position, distanceToActivate);
    }
    
}
