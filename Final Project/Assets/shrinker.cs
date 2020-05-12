using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shrinker : MonoBehaviour
{
    public int progressionCollectable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CollectableCheck()
    {
        if (PlayerPrefs.GetInt("totalOrbs") >= progressionCollectable)
        {
            StartCoroutine(ShrinkTime());
        }
    }

    IEnumerator ShrinkTime()
    {
        float timeElapsed = 0f;
        while (timeElapsed <= 1f)
        {
            timeElapsed += Time.deltaTime;
            transform.localScale = Vector3.MoveTowards(transform.localScale,
               Vector3.zero, Time.deltaTime);
            yield return null; 
        }
        yield return null;
    }
}
