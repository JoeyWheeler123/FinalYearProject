using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class gameManager : MonoBehaviour
{
    private Scene scene;

    private Scene sceneToLoad;

    private String sceneName;
    public String defaultSceneName;
    public bool loadOnStart, resetProgress;
    // Start is called before the first frame update
    void Start()
    {
        if (loadOnStart)
        {
            LoadProgress();
        }

        if (resetProgress)
        {
            ResetProgress();
        }
       scene = SceneManager.GetActiveScene();
       PlayerPrefs.GetString(scene.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }
    }

    public void StoreProgress()
    {
        PlayerPrefs.SetString("currentLevel",scene.name);
        PlayerPrefs.SetInt("checkPointNumber",SavePoint.currentCheckpoint);
    }

    public void LoadProgress()
    {
        if(PlayerPrefs.HasKey("currentLevel")){
            SavePoint.currentCheckpoint = PlayerPrefs.GetInt("checkPointNumber");
            sceneName =PlayerPrefs.GetString("currentLevel");
            SceneManager.LoadScene(sceneName);
        }

        else
        {
            SceneManager.LoadScene(defaultSceneName);
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}
