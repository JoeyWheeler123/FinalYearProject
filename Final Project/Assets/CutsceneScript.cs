using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneScript : MonoBehaviour
{
    public UnityEvent beginningEvent, middleEvent, outroEvent;
    public float interval1, interval2;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimedEvents());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator TimedEvents()
    {
        if (beginningEvent != null)
        {
            beginningEvent.Invoke();
        }
        yield return new WaitForSeconds(interval1);
        if (middleEvent != null)
        {
            middleEvent.Invoke();
        }
        yield return new WaitForSeconds(interval2);
        if (outroEvent != null)
        {
            outroEvent.Invoke();
        }
        yield return null;
    }
}
