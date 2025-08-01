using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRicochet : NetworkBehaviour
{
    public float destroyDelay = 5f; 
    private bool hasCollided = false;


    private void Start()
    {
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

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        //Destroy(gameObject);
        Runner.Despawn(Object);
    }
}
