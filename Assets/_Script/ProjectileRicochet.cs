using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRicochet : MonoBehaviour
{
    public float destroyDelay = 5f; 
    private bool hasCollided = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided)
        {
            hasCollided = true;

            
   

           
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
