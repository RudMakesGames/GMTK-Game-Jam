using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHitHandler : NetworkBehaviour
{
    Animator anim;
    NetworkMecanimAnimator networkAnim;
    NetworkRigidbody3D Nrb;
    Rigidbody rb;
    public float knockBackStrength, swatBackStrength;
    NetworkPlayer player;

    public NetworkPlayer lastHitPlayer;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        Nrb = GetComponent<NetworkRigidbody3D>();
        rb = GetComponent<Rigidbody>();
        player = GetComponent<NetworkPlayer>();
        networkAnim = GetComponent<NetworkMecanimAnimator>();
    }

    [Networked, OnChangedRender(nameof(HealthChanged))]
    public float NetworkedHealth { get; set; } = 100;

    void HealthChanged()
    {
        Debug.Log($"Health changed to: {NetworkedHealth}");

        if(lastHitPlayer!= null )   
        Debug.Log(lastHitPlayer.name);
        else
        {
            Debug.Log("wasnt shot by anyone");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DealDamageRpc(Vector3 projectileVelocity, NetworkPlayer whoShot)
    {
        player.isHit = true;
        lastHitPlayer = whoShot;
        //player.hitEffect.Play();
        player.playerAudio.PlayOneShot(player.sounds[4]);
        Debug.Log("was shot by "+whoShot);
        rb.AddForce(projectileVelocity.normalized * knockBackStrength);
        // The code inside here will run on the client which owns this object (has state and input authority).
        Debug.Log("Received knockback on StateAuthority, Adding force to this object" + projectileVelocity.normalized*knockBackStrength);
        NetworkedHealth -= 0.5f;
        //anim.SetTrigger("isHit");
        networkAnim.SetTrigger("isHit");
        Invoke("resetHitBool", 0.5f);

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void GetSwattedRPC(Vector3 hitPoint, NetworkPlayer whoHit)
    {
        player.isHit = true;
        lastHitPlayer = whoHit;
        //player.hitEffect.Play();
        player.playerAudio.PlayOneShot(player.sounds[5]);
        //Vector3 forceDirn = Vector3.Dot((hitPoint - player.transform.position).normalized, transform.right)>0 ? -transform.right : transform.right;
        Vector3 forceDirn = (player.transform.position-hitPoint).normalized;
        forceDirn.y = 0;

        rb.AddForce(forceDirn.normalized * swatBackStrength);
        // The code inside here will run on the client which owns this object (has state and input authority).
        Debug.Log("Received knockback on StateAuthority, Adding force to this object" + forceDirn.normalized * swatBackStrength);
        NetworkedHealth -= 0.5f;
        //anim.SetTrigger("isHit");
        networkAnim.SetTrigger("isHit");
        Invoke("resetHitBool", 0.5f);
    }

    void resetHitBool()
    {
        player.isHit = false;
    }
}
