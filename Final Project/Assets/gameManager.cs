using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class gameManager : MonoBehaviour
{
    private Scene scene;

    private Scene sceneToLoad;

    private String sceneName;
    public String defaultSceneName;
    public bool loadOnStart, resetProgress;
    public Text loadingText;
    // Start is called before the first frame update
    void Start()
    {
        loadingText.gameObject.SetActive(false);
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

        if (loadingText != null)
        {
            //loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b,
                //Mathf.PingPong(Time.time, 1));
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
           
            StartCoroutine(AsyncLoad());
        }

        else
        {
            sceneName = defaultSceneName;
            StartCoroutine(AsyncLoad());
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }

    IEnumerator AsyncLoad()
    {
        yield return new WaitForSeconds(1f);
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
        }

        AsyncOperation async = Application.LoadLevelAsync(sceneName);
        async.allowSceneActivation = false;

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            loadingText.text = "loading: " + async.progress * 100f;
            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
        yield return null;
    }
}
