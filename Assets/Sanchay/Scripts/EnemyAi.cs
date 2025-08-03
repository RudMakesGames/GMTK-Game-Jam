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

    public GameObject projectilePrefab;

    NavMeshAgent agent;

    Vector3 projectileSpawnPoint;

    public float projectileSpeed=50f;


    private void Start()
    {
        InvokeRepeating("UpdateMoveLocation", 2f, 8f);
        InvokeRepeating("shootAtPlayer", 2f, 2f);
        Invoke("getReferences", 8f);
        agent = GetComponent<NavMeshAgent>();
        //projectileSpawnPoint = transform.Find("Follow Target").Find("railgun").Find("FirePoint").transform.position;
    }

    private void Update()
    {
        if (player == null || playerRayCastScript == null)
        {
            player = FindObjectOfType<NetworkPlayer>().gameObject;
            playerRayCastScript = player.GetComponent<PlayerRayCastScript>();
        }

        if(player!=null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 4f) UpdateMoveLocation();

        }
    }

    void getReferences()
    {
        player = FindObjectOfType<NetworkPlayer>().gameObject;
        playerRayCastScript = player.GetComponent<PlayerRayCastScript>();
    }

    void shootAtPlayer()
    {
        if (player != null && projectilePrefab != null && player.transform.position.y<15f)
        {
            GameObject projectileInstance = GameObject.Instantiate(projectilePrefab, transform.Find("Follow Target").transform.Find("railgun").transform.Find("FirePoint").transform.position, Quaternion.identity);
            projectileInstance.GetComponent<Rigidbody>().velocity = (player.transform.position- transform.Find("Follow Target").transform.Find("railgun").transform.Find("FirePoint").transform.position).normalized*projectileSpeed;
        }
    }
    void UpdateMoveLocation()
    {

        if(playerRayCastScript.updatePos && player!=null && player.transform.position.y<20f)
        {
            int randomPoint = Random.Range(0, playerRayCastScript.validGroundPoints.Count);
            nextMovePoint = playerRayCastScript.validGroundPoints[randomPoint];
            agent.SetDestination(nextMovePoint);
        }
        

    }

}
