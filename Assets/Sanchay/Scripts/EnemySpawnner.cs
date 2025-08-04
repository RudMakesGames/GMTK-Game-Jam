using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnner : MonoBehaviour
{
    public GameObject EnemyPrefab;

    public static int EnemyCount;

    public int maxEnemyCount = 12;

    public bool spawnStopped = false;

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 5f, 15f);
        //EnemyPrefab = FindObjectOfType<NetworkPlayer>().enemyPrefab;
    }


    private void Update()
    {
        /*if (EnemyPrefab==null)
        {
            EnemyPrefab = FindObjectOfType<NetworkPlayer>().enemyPrefab;
        }*/
        if (EnemyCount>= maxEnemyCount && !spawnStopped)
        {
            spawnStopped = true;
        }

        else if(EnemyCount < maxEnemyCount && spawnStopped)
        {
            spawnStopped=false;
        }
    }

    void SpawnEnemy()
    {
        if (spawnStopped || EnemyPrefab==null) return;


        GameObject.Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
        EnemyCount++;
    }
}
