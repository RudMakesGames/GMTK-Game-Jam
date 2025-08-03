using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalResultsController : MonoBehaviour
{
    public AudioClip Honk1, Honk2, Tada, Firework;
    public GameObject Button,Rockets;
    
    void Update()
    {
        
    }
    public void EnableButton()
    {
        Button.SetActive(true);
    }
    public void ThirdPlace()
    {
        AudioManager.instance.PlaySoundFXClip(Honk1, transform, 1, 1);
        AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f, 1.1f));
    }
    public void SecondPlace()
    {
        AudioManager.instance.PlaySoundFXClip(Honk2, transform, 1, 1);
        AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f, 1.1f));
    }

    public void FirstPlace()
    {
        AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f,1.1f));
    }
    public void PlayFireCrackers()
    {
        AudioManager.instance.PlaySoundFXClip(Firework, transform, 1, 1);
        Rockets.SetActive(true);
    }
}
