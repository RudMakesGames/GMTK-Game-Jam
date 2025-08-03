using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelinePlayer : MonoBehaviour
{
    public float Delay;
    public string NextScene;
    void Start()
    {
        StartCoroutine(PlayDirector());
    }
    IEnumerator PlayDirector()
    {
        yield return new WaitForSeconds(Delay);
        SceneManager.LoadScene(NextScene);
    }
    
    void Update()
    {
        
    }
}
