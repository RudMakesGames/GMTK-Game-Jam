using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Respawner : NetworkBehaviour
{
    public int RespawnPoints;
    public int RequiredPoints = 4;
    public float respawnTimeLimit = 30f;
    public Transform SpawnPoint;
    public Transform TPpoint;

    private float timer = 0f;
    private bool timerActive = false;

    Slider timeLeftSlider;
    TextMeshProUGUI timeLeftText;

    NetworkPlayer player;
    NetworkHitHandler hitHandler;

    bool killCountUpdating = false;


    void Start()
    {
        //timer = respawnTimeLimit;

        /*GameObject sp = GameObject.Find("SpawnPoint");
        GameObject tp = GameObject.Find("Tp Point");*/

        /*if (sp != null) SpawnPoint = sp.transform;
        else Debug.LogWarning("SpawnPoint not found!");

        if (tp != null) TPpoint = tp.transform;
        else Debug.LogWarning("TP Point not found!");*/
    }

    public void setReferences()
    {
        timer = respawnTimeLimit;

        GameObject sp = GameObject.Find("SpawnPoint");
        GameObject tp = GameObject.Find("TpPoint");

        if (sp != null) SpawnPoint = sp.transform;
        else Debug.LogWarning("SpawnPoint not found!");

        if (tp != null) TPpoint = tp.transform;
        else Debug.LogWarning("TP Point not found!");

        player = GetComponent<NetworkPlayer>();


        if(Object.HasInputAuthority)
        {
            timeLeftSlider = GameObject.Find("TimeLeft").GetComponent<Slider>();
            timeLeftSlider.maxValue = respawnTimeLimit;
            timeLeftSlider.gameObject.SetActive(false);
            timeLeftText = GameObject.Find("RespawnTimer").GetComponent<TextMeshProUGUI>();
            timeLeftText.gameObject.SetActive(false);
            hitHandler = GetComponent<NetworkHitHandler>();
        }   
    }

    public void AddRespawnPoint()
    {
        RespawnPoints++;
        CheckForPoints();
    }

    private void CheckForPoints()
    {
        if (RespawnPoints == RequiredPoints)
        {
            Debug.Log("Respawned!");
            RespawnAtSpawnPoint();
        }
    }

    private void RespawnAtSpawnPoint()
    {
        if (SpawnPoint != null)
        {
            transform.position = SpawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No SpawnPoint set!");
        }

        if(Object.HasInputAuthority)
        {
            timeLeftSlider.gameObject.SetActive(false);
            timeLeftText.gameObject.SetActive(false);
        }
        
        ResetState2(false);
    }

    private void Die()
    {
        Debug.Log("Player ran out of time! Attempting to teleport to TP Point.");

        if (TPpoint != null)
        {
            //transform.position = TPpoint.position;
            player.shouldTp = true;
            Debug.Log("Teleported to TP Point: " + TPpoint.position);
        }
        else
        {
            Debug.LogWarning("TP Point is null!");
        }

        if (Object.HasInputAuthority)
        {
            timeLeftSlider.gameObject.SetActive(true);
            timeLeftText.gameObject.SetActive(true);
        }

        ResetState2(true);
    }

    private void ResetState2(bool dead)
    {
        RespawnPoints = 0;
        timer = respawnTimeLimit;

        if(dead)
        {
            timerActive = true;
            player.respawnQuip();
        }
        else
        {
            timerActive = false;
        }


    }


    public override void FixedUpdateNetwork()
    {
        if (timerActive)
        {
            timer -= Runner.DeltaTime;
            timeLeftText.text = Mathf.FloorToInt(timer).ToString();

            timeLeftSlider.value = timer;

            if (timer <= 0f)
            {
                Die();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bounds") && !timerActive)
        {
            if (Object.HasInputAuthority)
            {
                timeLeftSlider.gameObject.SetActive(true);
                timeLeftText.gameObject.SetActive(true);

                Debug.Log("Entered Bounds – starting timer.");
                timerActive = true;
                timer = respawnTimeLimit;

                if (!killCountUpdating)
                {
                    killCountUpdating = true;
                    StartCoroutine(updateKillCount());
                }
            }
            /*timeLeftSlider.gameObject.SetActive(true);
            Debug.Log("Entered Bounds – starting timer.");
            timerActive = true;
            timer = respawnTimeLimit;

            if(!killCountUpdating)
            {
                killCountUpdating = true;
                StartCoroutine(updateKillCount());
            }*/
        }
    }

    IEnumerator updateKillCount()
    {
        if(hitHandler.lastHitPlayer==null) yield return null;
        else
        {
            Debug.Log("Enterning coroutine for kill update");
            hitHandler.lastHitPlayer.updateKillForPlayerRpc();
            yield return new WaitForSeconds(0.8f);
        }
        killCountUpdating=false;
    }
}
