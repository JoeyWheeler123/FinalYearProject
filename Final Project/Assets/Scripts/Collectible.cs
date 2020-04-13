using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int collectableIndex;
    public bool resetCollectable;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")|| other.gameObject.CompareTag("box"))
        {
            BoxProperties.orbsCollected++;
            BoxProperties.totalOrbs++;
            gameManager.displayCollectible = true;
            string storedIndex = collectableIndex.ToString();
            PlayerPrefs.SetInt(storedIndex, 1);
            PlayerPrefs.SetInt("collectables", BoxProperties.totalOrbs);
            Destroy(this.gameObject);
        }
    }
    IEnumerator SelfDestruct()
    {
        float fTime = Random.Range(0.1f, 0.2f);
        yield return new WaitForSeconds(fTime);
        string storedIndex = collectableIndex.ToString();
        if (resetCollectable)
        {
            PlayerPrefs.SetInt(storedIndex, 0);
        }
        if (PlayerPrefs.GetInt(storedIndex) == 1)
        {
            BoxProperties.orbsCollected++;
            Destroy(this.gameObject);
        }
        yield return null;
    }
}
