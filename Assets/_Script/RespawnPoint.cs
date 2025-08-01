using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private CollectibleSpawner spawner;
    public void SetSpawner(CollectibleSpawner Spawner)
    {
        this.spawner = Spawner;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Respawner>() != null)
        {
            collision.gameObject.GetComponent<Respawner>().AddRespawnPoint();
            Destroy(gameObject);
        }
    }
}
