using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swatter : NetworkBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject!=transform.parent.parent.gameObject)
            {
                Debug.Log("collison detected");
                if (collision.gameObject.TryGetComponent<NetworkHitHandler>(out var hitHandler))
                { 
                    hitHandler.GetSwattedRPC(collision.contacts[0].point, transform.parent.parent.GetComponent<NetworkPlayer>());
                    Debug.Log("RPC call sent out");
                }
            }
        
    }
}
