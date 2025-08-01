using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject RespawnPoint;
    [SerializeField] private float respawnTime = 5f;

    private GameObject currentRespawnPoint;

    private void Start()
    {
        SpawnCollectible();
    }

    private void SpawnCollectible()
    {
        currentRespawnPoint = Instantiate(RespawnPoint,transform.position, Quaternion.identity);

        currentRespawnPoint.transform.SetParent(transform);

        RespawnPoint respawnPoint = currentRespawnPoint.GetComponent<RespawnPoint>();
        if(respawnPoint != null)
        {
            respawnPoint.SetSpawner(this);
        }
    }
    public void OnCollectiblePickedUp()
    {
        currentRespawnPoint = null;
        Invoke(nameof(SpawnCollectible),respawnTime);
    }
}
