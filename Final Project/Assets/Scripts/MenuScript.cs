using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MenuScript : MonoBehaviour
{
    public GameObject defaultButton, optionsDefault, options, pauseCanvas;
    //public PlayerControls controls;
    public bool mainMenu;
    public bool gamePaused = false;
    public Text continueText;
    public Button continueButton;
    // Start is called before the first frame update
    void Start()
    {
       // controls = new PlayerControls();
        if (!mainMenu)
        {
           // controls.Gameplay.Pause.performed += ctx => PauseTime();
        }
        if (!PlayerPrefs.HasKey("currentLevel"))
        {
            print("disablingContinue");
            Color newColour = continueText.color;
            continueButton.interactable = false;
            continueText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }
    }
    private void OnEnable()
    {
       // controls.Gameplay.Enable();
        //controls.Gameplay.Pause.Enable();
    }

    private void OnDisable()
    {
        //controls.Gameplay.Disable();
    }
    // Update is called once per frame
    void Update()
    {
       
    }

    public void OptionsSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsDefault);
    }
    public void OptionsDeselected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(options);
    }

    /*public void PauseTime()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0;
            gamePaused = true;
            pauseCanvas.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(defaultButton);
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            gamePaused = false;
            pauseCanvas.SetActive(false);
            Cursor.visible = false;
        }
    }
    */
}
