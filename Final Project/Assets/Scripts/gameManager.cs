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
    public Animator anim;
    public Text collectedText;
    private float textOnScreenTime;
    public static bool displayCollectible;
    private GameObject[] allCollectibles;
    private int totalCollectibles;
    public GameObject canvas;
    public String levelToSave;

    
    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(true);
        collectedText.CrossFadeAlpha(0, 0, false);
        loadingText.gameObject.SetActive(false);
        loadingText.text = "loading";
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
        allCollectibles = GameObject.FindGameObjectsWithTag("collectible");
        totalCollectibles = allCollectibles.Length;
        print(totalCollectibles);
      
    }

    // Update is called once per frame
    void Update()
    {
        if (displayCollectible)
        {
            textOnScreenTime = 3f;
            displayCollectible = false;
        }
        if (textOnScreenTime >= 0f)
        {
            textOnScreenTime -= Time.deltaTime;
            collectedText.CrossFadeAlpha(1, 0, false);
        }
        else
        {
            collectedText.CrossFadeAlpha(0, 0.5f, false);
        }
        collectedText.text = BoxProperties.orbsCollected.ToString()+"/"+totalCollectibles.ToString(); 
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
            PlayerPrefs.SetString("currentLevel", sceneName);
            StartCoroutine(AsyncLoad());
        }
    }
    public void LevelSelected(String levelString)
    {
        sceneName = levelString;
        StartCoroutine(AsyncLoad());
    }
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        StartCoroutine(BeginNewGame());
    }
    public void SetProgress()
    {
        PlayerPrefs.SetString("currentLevel", levelToSave);
        PlayerPrefs.SetInt("checkPointNumber", 0);
    }
    IEnumerator AsyncLoad()
    {
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
        }
        anim.SetTrigger("Start");
        loadingText.text = "loading";
        yield return new WaitForSeconds(1f);
       

        AsyncOperation async = Application.LoadLevelAsync(sceneName);
        async.allowSceneActivation = false;
        

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        float transitionTime = 0;
        loadingText.text = "loading..";
        while (!async.isDone)
        {
            transitionTime += Time.deltaTime;
            
            if (async.progress >= 0.9f&&transitionTime>=0.5f)
            {
              
                async.allowSceneActivation = true;
                loadingText.text = "loading...";
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator BeginNewGame()
    {
        PlayerPrefs.DeleteAll();
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        sceneName = defaultSceneName;
        PlayerPrefs.SetString("currentLevel", sceneName);
        StartCoroutine(AsyncLoad());
        yield return null;
    }
}
