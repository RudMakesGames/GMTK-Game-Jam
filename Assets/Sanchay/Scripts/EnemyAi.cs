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

    bool isNotMoving = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition=false;
    }
    private void Start()
    {
        InvokeRepeating("UpdateMoveLocation", 1f, 2f);
        player = FindObjectOfType<NetworkPlayer>().gameObject;
        playerRayCastScript = player.GetComponent<PlayerRayCastScript>();

        InvokeRepeating("shootAtPlayer", 6f, 2f);
        Invoke("getReferences", 1f);
        //projectileSpawnPoint = transform.Find("Follow Target").Find("railgun").Find("FirePoint").transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(nextMovePoint, transform.position)<4f)
        {
            isNotMoving = true ;
        }
        else
        {
            isNotMoving = false ;
        }
        if (player == null || playerRayCastScript == null)
        {
            player = FindObjectOfType<NetworkPlayer>().gameObject;
            playerRayCastScript = player.GetComponent<PlayerRayCastScript>();
        }

        if(player!=null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 5f && isNotMoving)
            {
                Debug.Log("called2");
                UpdateMoveLocation();
            }

        }

        if(transform.position.y<-10f)
        {
            EnemySpawnner.EnemyCount--;
            Destroy(gameObject);
        }
    }

    void getReferences()
    {
        agent.updatePosition = true;
    }

    void shootAtPlayer()
    {
        if (player != null && projectilePrefab != null && player.transform.position.y<15f && !isNotMoving)
        {
            GameObject projectileInstance = GameObject.Instantiate(projectilePrefab, transform.Find("Follow Target").transform.Find("railgun").transform.Find("FirePoint").transform.position, Quaternion.identity);
            projectileInstance.GetComponent<Rigidbody>().velocity = (player.transform.position- transform.Find("Follow Target").transform.Find("railgun").transform.Find("FirePoint").transform.position).normalized*projectileSpeed;
        }
    }
    void UpdateMoveLocation()
    {
        if(playerRayCastScript.updatePos && player!=null && player.transform.position.y<20f && agent.enabled)
        {
            int randomPoint = Random.Range(0, playerRayCastScript.validGroundPoints.Count);
            //nextMovePoint = new Vector3(playerRayCastScript.validGroundPoints[randomPoint].x, transform.position.y, playerRayCastScript.validGroundPoints[randomPoint].z) ;
            nextMovePoint = playerRayCastScript.validGroundPoints[randomPoint];
            agent.SetDestination(nextMovePoint);
            Debug.Log(nextMovePoint);
        }
        

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(stopAgent());
        }
    }

    IEnumerator stopAgent()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(1.2f);
        agent.enabled = true;
    }

}
