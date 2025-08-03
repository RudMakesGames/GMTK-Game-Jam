using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    bool playerLanded = false;

    public Vector3 nextMovePoint;

    GameObject player;
    PlayerRayCastScript playerRayCastScript;

    private void Start()
    {
        player = FindObjectOfType<NetworkPlayer>().gameObject;
        playerRayCastScript=player.GetComponent<PlayerRayCastScript>();
        InvokeRepeating("UpdateMoveLocation", 2f, 2f);
    }

    private void Update()
    {
        
    }


    void UpdateMoveLocation()
    {
        int randomPoint = Random.Range(0,playerRayCastScript.validGroundPoints.Count);

        nextMovePoint = playerRayCastScript.validGroundPoints[randomPoint];

    }

}
