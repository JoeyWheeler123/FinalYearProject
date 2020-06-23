using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;


public class MenuScript : MonoBehaviour
{
    public GameObject defaultButton, optionsDefault, options, pauseCanvas, extras, extrasDefault;

    public GameObject newGameWarningCanvas, defaultCanvas, warningDefault, newGameDefault;

    public UnityEvent beginNewGame, crescentAnim;
    //public PlayerControls controls;
    public bool mainMenu;
    public bool gamePaused = false;
    public Text continueText;
    public Text bonusLevelsText;
    public Text level2Text, level3Text;
    public Button continueButton;
    public Button bonusLevelsButton;
    public Button level2Button, level3Button;
    public int minCollectables;
    public int totalCollectables;
    public Text collectableText;
    

    public Text totalOrbs;
    // Start is called before the first frame update
    void Start()
    {
        int coll = PlayerPrefs.GetInt("collectables");
        print(coll);
        crescentAnim.Invoke();
       // controls = new PlayerControls();
        if (!mainMenu)
        {
           // controls.Gameplay.Pause.performed += ctx => PauseTime();
        }
        /*if (!PlayerPrefs.HasKey("currentLevel"))
        {
            print("disablingContinue");
            Color newColour = continueText.color;
            continueButton.interactable = false;
            continueText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }
        int storedCollectables = PlayerPrefs.GetInt("collectables");
        if (storedCollectables < minCollectables)
        {
            Color newColour = bonusLevelsText.color;
            bonusLevelsButton.interactable = false;
            bonusLevelsText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }

        if (PlayerPrefs.GetInt("collectables") <= minCollectables)
        {

            string colString = PlayerPrefs.GetInt("collectables").ToString() + "/" + minCollectables.ToString();
            collectableText.text = colString;
        }
        else
        {
            string colString = minCollectables.ToString() + "/" + minCollectables.ToString();
            collectableText.text = colString;
        }

        String totalOrbsString = PlayerPrefs.GetInt("collectables").ToString() + "/" + totalCollectables.ToString();
        totalOrbs.text = totalOrbsString;
        //checking if levels have been started;
        if (!PlayerPrefs.HasKey("Level 2"))
        {
            Color newColour = level2Text.color;
            level2Button.interactable = false;
            level2Text.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }
        if (!PlayerPrefs.HasKey("Level 3"))
        {
            Color newColour = level3Text.color;
            level3Button.interactable = false;
            level3Text.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }
        */
        StartCoroutine(MenuBeginSelection());
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
    public void ExtrasSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(extrasDefault);
    }
    public void ExtrasDeselected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(extras);
    }

    public void NewGameSelected()
    {
        if (PlayerPrefs.HasKey("checkPointNumber"))
        {
            newGameWarningCanvas.SetActive(true);
            defaultCanvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(warningDefault);
        }
        else
        {
            beginNewGame.Invoke();
        }
    }

    public void NewGameDeselected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newGameDefault);   
    }

    public void QuitGame()
    {
        Application.Quit();
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
    IEnumerator MenuBeginSelection()
    {
        newGameWarningCanvas.SetActive(false);
        defaultCanvas.SetActive(false);
        yield return new WaitForSeconds(2f);
        Cursor.visible = true;
        defaultCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newGameDefault);
        print("coroutine time");
        
        if (!PlayerPrefs.HasKey("currentLevel"))
        {
            print("disablingContinue");
            Color newColour = continueText.color;
            continueButton.interactable = false;
            continueText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }
        int storedCollectables = PlayerPrefs.GetInt("collectables");
        if (storedCollectables < minCollectables)
        {
            Color newColour = bonusLevelsText.color;
            bonusLevelsButton.interactable = false;
            bonusLevelsText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }

        if (PlayerPrefs.GetInt("collectables") <= minCollectables)
        {

            string colString = PlayerPrefs.GetInt("collectables").ToString() + "/" + minCollectables.ToString();
            collectableText.text = colString;
        }
        else
        {
            string colString = minCollectables.ToString() + "/" + minCollectables.ToString();
            collectableText.text = colString;
        }

        String totalOrbsString = PlayerPrefs.GetInt("collectables").ToString() + "/" + totalCollectables.ToString();
        totalOrbs.text = totalOrbsString;
        //checking if levels have been started;
        if (!PlayerPrefs.HasKey("Level 2"))
        {
            Color newColour = level2Text.color;
            level2Button.interactable = false;
            level2Text.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }
        if (!PlayerPrefs.HasKey("Level 3"))
        {
            Color newColour = level3Text.color;
            level3Button.interactable = false;
            level3Text.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, 0.5f);
        }
        yield return null;
    }
}
