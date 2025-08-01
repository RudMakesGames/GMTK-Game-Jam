using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    public int RespawnPoints;
    public int RequiredPoints = 4;
    public float respawnTimeLimit = 30f;
    public Transform SpawnPoint;
    public Transform TPpoint;

    private float timer = 0f;
    private bool timerActive = false;


    void Start()
    {
        timer = respawnTimeLimit;

        GameObject sp = GameObject.Find("SpawnPoint");
        GameObject tp = GameObject.Find("Tp Point");

        if (sp != null) SpawnPoint = sp.transform;
        else Debug.LogWarning("SpawnPoint not found!");

        if (tp != null) TPpoint = tp.transform;
        else Debug.LogWarning("TP Point not found!");
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

        ResetState();
    }

    private void Die()
    {
        Debug.Log("Player ran out of time! Attempting to teleport to TP Point.");

        if (TPpoint != null)
        {
            transform.position = TPpoint.position;
            Debug.Log("Teleported to TP Point: " + TPpoint.position);
        }
        else
        {
            Debug.LogWarning("TP Point is null!");
        }

        ResetState();
    }

    private void ResetState()
    {
        RespawnPoints = 0;
        timer = respawnTimeLimit;
        timerActive = false;
    }

    void Update()
    {
        if (timerActive)
        {
            timer -= Time.deltaTime;

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
            Debug.Log("Entered Bounds – starting timer.");
            timerActive = true;
            timer = respawnTimeLimit;
        }
    }
}
