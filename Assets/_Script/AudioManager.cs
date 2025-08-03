using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource soundFXObject;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void PlaySoundFXClip(AudioClip audioclip, Transform spawnTransforn, float volume, float pitch)
    {
        AudioSource audiosource = Instantiate(soundFXObject, spawnTransforn.position, Quaternion.identity);
        audiosource.clip = audioclip;
        audiosource.volume = volume;
        audiosource.pitch = pitch;
        audiosource.Play();
        float clipLength = audiosource.clip.length;
        Destroy(audiosource.gameObject, clipLength);
    }
}
