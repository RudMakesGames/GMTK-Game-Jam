using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    [Networked]
    public float MatchTime { get; set; }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasInputAuthority)
        {
            MatchTime += Runner.DeltaTime;
            //Mathf.CeilToInt(MatchTime);
        }
    }
}
