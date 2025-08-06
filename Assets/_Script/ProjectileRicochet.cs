using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRicochet : NetworkBehaviour
{
    public float destroyDelay = 5f;
    private bool hasCollided = false;
    public Transform SpawnedPoint;
    public bool isBoomerang = false;
    private Rigidbody rb;

    public Vector3 projectileVelocity;

    public NetworkPlayer player;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (isBoomerang)
        {
            StartCoroutine(StartBoomerang());
        }

        Invoke("DestroyLifetime", 15f);
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (!hasCollided)
        {
            if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("collison detected");
                if(collision.gameObject.TryGetComponent<NetworkHitHandler>(out var hitHandler))
                {
                    //if(hitHandler!=null && transform.parent.GetComponent<NetworkPlayer>()!=null) 
                    if(CharacterSelector.selectedMode!=1)
                    {
                        hitHandler.DealDamageRpc(projectileVelocity, null);
                        Debug.Log("RPC call sent out with no hit player as single player mode");

                    }
                    else
                    {
                        hitHandler.DealDamageRpc(projectileVelocity, player);
                        Debug.Log("RPC call sent out with hit player as" + player.name);
                    }
                }
            }
            hasCollided = true;
            StartCoroutine(DestroyAfterDelay());
        }
    }

    void DestroyLifetime()
    {
        if(CharacterSelector.selectedMode==1)
        {
            if (!hasCollided)
            {
                Runner.Despawn(Object);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public override void FixedUpdateNetwork()
    {
        projectileVelocity = rb.velocity;
    }

    IEnumerator StartBoomerang()
    {
        yield return new WaitForSeconds(0.75f); 

        if (rb != null && rb.velocity != Vector3.zero)
        {
          
            Vector3 returnDirection = -rb.velocity.normalized;

            float angleOffset = Random.Range(-5f, 5f);
            returnDirection = Quaternion.Euler(0f, angleOffset, 0f) * returnDirection;

            float speed = rb.velocity.magnitude;
            rb.velocity = returnDirection * speed;
        }
    }
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        
        if(CharacterSelector.selectedMode!=1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Runner.Despawn(Object);
        }
    }
}
    
