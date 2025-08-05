using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private CollectibleSpawner spawner;
    [SerializeField] float rotationSpeed=40f;
    public void SetSpawner(CollectibleSpawner Spawner)
    {
        this.spawner = Spawner;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Respawner>() != null)
        {
            collision.gameObject.GetComponent<Respawner>().AddRespawnPoint();
            collision.gameObject.GetComponent<NetworkPlayer>().playerAudio.PlayOneShot(collision.gameObject.GetComponent<NetworkPlayer>().sounds[6]);
            spawner?.OnCollectiblePickedUp();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
