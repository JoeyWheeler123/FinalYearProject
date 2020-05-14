using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour
{
public CanvasGroup cGroup;
public GameObject title,credits;
public float timeToStart,titleTime,namesTime;
    // Start is called before the first frame update
    void Start()
    {
    StartCoroutine(BeginCredits());
        if(cGroup!=null){
        cGroup = GetComponent<CanvasGroup>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator BeginCredits(){
    yield return new WaitForSeconds(timeToStart);
    float timeElapsed = 0f;
            while(timeElapsed<=2f){
            cGroup.alpha +=Time.deltaTime/2f;
            timeElapsed += Time.deltaTime;
            yield return null;
            }
            title.SetActive(true);
   yield return new WaitForSeconds(titleTime);
   title.SetActive(false);
   credits.SetActive(true);
   yield return new WaitForSeconds(namesTime);
    }
}

