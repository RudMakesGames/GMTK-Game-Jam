using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalResultsController : MonoBehaviour
{
    public AudioClip Honk1, Honk2, Tada, Firework;
    public GameObject Button,Rockets;

    public MatchManager matchManager;

    public GameObject thirdWinner, secondWinner, Winner;

    public List<Material> materials;

    public List<Light> lights;


    private void Start()
    {
        foreach(Light light in lights)
        {
            light.gameObject.SetActive(false);
        }
        matchManager = FindObjectOfType<MatchManager>();
        GetWinners();
    }

    void Update()
    {
        
    }
    public void EnableButton()
    {
        Button.SetActive(true);
    }

    public void GoToMainMenu()
    {
        if(matchManager != null)
        Destroy(matchManager.gameObject);

        SceneManager.LoadSceneAsync("Menu");
    }

    public void GetWinners()
    {
        /*thirdWinner = matchManager.SetWinner(3);
        secondWinner = matchManager.SetWinner(2);
        Winner = matchManager.SetWinner(1);*/

        if(matchManager.WinnerList.Count >= 3)
        {
            thirdWinner.GetComponent<Renderer>().material = materials[matchManager.thirdPlaceMatIndex];
            thirdWinner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.thirdPlace;

            secondWinner.GetComponent<Renderer>().material = materials[matchManager.secondPlaceMatIndex];
            secondWinner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.secondPlace;

            Winner.GetComponent<Renderer>().material = materials[matchManager.WinnerMatIndex];
            Winner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.Winner;

        }

        else if(matchManager.WinnerList.Count >= 2)
        {
            secondWinner.GetComponent<Renderer>().material = materials[matchManager.secondPlaceMatIndex];
            secondWinner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.secondPlace;

            Winner.GetComponent<Renderer>().material = materials[matchManager.WinnerMatIndex];
            Winner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.Winner;
        }

        else
        {
            Winner.GetComponent<Renderer>().material = materials[matchManager.WinnerMatIndex];
            Winner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.Winner;
        }

    }
    public void ThirdPlace()
    {
        /*NetworkPlayer thirdWinner = matchManager.SetWinner(3);

        if (thirdWinner == null) return;*/
        if (matchManager.WinnerList.Count>=3)
        {
            AudioManager.instance.PlaySoundFXClip(Honk1, transform, 1, 1);
            AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f, 1.1f));
            lights[0].gameObject.SetActive(true);
        }

    }
    public void SecondPlace()
    {
        /*NetworkPlayer secondWinner = matchManager.SetWinner(2);

        if (secondWinner == null) return;*/

        if (matchManager.WinnerList.Count>=2)
        {
            AudioManager.instance.PlaySoundFXClip(Honk2, transform, 1, 1);
            AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f, 1.1f));
            lights[1].gameObject.SetActive(true);
        }
    }

    public void FirstPlace()
    {
        AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f,1.1f));
        lights[2].gameObject.SetActive(true);
    }
    public void PlayFireCrackers()
    {
        AudioManager.instance.PlaySoundFXClip(Firework, transform, 1, 1);
        Rockets.SetActive(true);
    }


    /*IEnumerator PlaySounds(int i)
    {
        yield return new WaitForSeconds(0.85f);

        if(i==3)
        {
            AudioManager.instance.PlaySoundFXClip(Honk1, transform, 1, 1);
            AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f, 1.1f));
        }
        else if(i==2) 
        {
            AudioManager.instance.PlaySoundFXClip(Honk2, transform, 1, 1);
            AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f, 1.1f));
        }
        else
        {
            AudioManager.instance.PlaySoundFXClip(Tada, transform, 0.5f, Random.Range(0.9f, 1.1f));

        }
    }*/
}
