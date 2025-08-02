using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHitHandler : NetworkBehaviour
{
    Animator anim;
    NetworkRigidbody3D Nrb;
    Rigidbody rb;
    public float knockBackStrength;
    NetworkPlayer player;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        Nrb = GetComponent<NetworkRigidbody3D>();
        rb = GetComponent<Rigidbody>();
        player = GetComponent<NetworkPlayer>();
    }

    [Networked, OnChangedRender(nameof(HealthChanged))]
   
    public float NetworkedHealth { get; set; } = 100;

    void HealthChanged()
    {
        Debug.Log($"Health changed to: {NetworkedHealth}");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(Vector3 projectileVelocity)
    {
        player.isHit = true;
        rb.AddForce(projectileVelocity.normalized * knockBackStrength);
        // The code inside here will run on the client which owns this object (has state and input authority).
        Debug.Log("Received knockback on StateAuthority, Adding force to this object" + projectileVelocity.normalized*knockBackStrength);
        NetworkedHealth -= 0.5f;
        anim.SetTrigger("isHit");
        Invoke("resetHitBool", 0.15f);
    }

    void resetHitBool()
    {
        player.isHit = false;
    }
}
