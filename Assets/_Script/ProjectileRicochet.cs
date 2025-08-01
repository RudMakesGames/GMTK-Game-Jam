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
        if (!hasCollided)
        {
            hasCollided = true;
            StartCoroutine(DestroyAfterDelay());
        }
    }

    void DestroyLifetime()
    {
        if (!hasCollided)
        {
            Runner.Despawn(Object);
        }
    }

    IEnumerator StartBoomerang()
    {
        yield return new WaitForSeconds(2f); 

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
        Runner.Despawn(Object);
    }
}
    
