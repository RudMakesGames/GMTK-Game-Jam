using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalResultsController : MonoBehaviour
{
    public AudioClip Honk1, Honk2, Tada, Firework;
    public GameObject Button,Rockets;

    MatchManager matchManager;

    public GameObject thirdWinner, secondWinner, Winner;


    private void Start()
    {
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
            thirdWinner.GetComponent<Renderer>().material = matchManager.MaterialList[2];
            thirdWinner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.NameList[2];

            secondWinner.GetComponent<Renderer>().material = matchManager.MaterialList[1];
            secondWinner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.NameList[1];

            Winner.GetComponent<Renderer>().material = matchManager.MaterialList[0];
            Winner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.NameList[0];
        }

        else if(matchManager.WinnerList.Count >= 2)
        {
            secondWinner.GetComponent<Renderer>().material = matchManager.MaterialList[1];
            secondWinner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.NameList[1];

            Winner.GetComponent<Renderer>().material = matchManager.MaterialList[0];
            Winner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.NameList[0];
        }
        
        else
        {
            Winner.GetComponent<Renderer>().material = matchManager.MaterialList[0];
            Winner.GetComponentInChildren<TextMeshProUGUI>().text = matchManager.NameList[0];
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
        }
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
