using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class MatchManager : NetworkBehaviour
{
    [Networked]
    public float MatchTime { get; set; }

    [Networked]
    public float BallonYRotPrime { get; set; }

    [SerializeField] float BalloonRotSpeed = 5f;

    GameObject primeBalloon;

    public List<NetworkPlayer> WinnerList;
    public List<Material> MaterialList;
    public List<string> NameList;

    public Material[] materials;

    bool fetchResults=false;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    //List<NetworkPlayer> playerList;

    public Dictionary<NetworkPlayer, int> playerKillCounter;

    public override void Spawned()
    {
        primeBalloon = GameObject.Find("CenterCyclinder");

        //playerList = new List<NetworkPlayer>(FindObjectsOfType<NetworkPlayer>());

        playerKillCounter = new Dictionary<NetworkPlayer, int>();

        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();

        foreach (NetworkPlayer player in players)
        {
            if (!playerKillCounter.ContainsKey(player))
            {
                playerKillCounter.Add(player, 0);
            }
        }

        foreach (var entry in playerKillCounter)
        {
            Debug.Log(entry.Key.name+" -> "+entry.Value);
        }

    }

    public override void Render()
    {
        //base.Render();
        primeBalloon.transform.Rotate(0, BalloonRotSpeed * Runner.DeltaTime, 0);
        BallonYRotPrime = primeBalloon.transform.eulerAngles.y;
    }



    public override void FixedUpdateNetwork()
    {
        if(Object.HasInputAuthority)
        {
            MatchTime += Runner.DeltaTime;
            //Mathf.CeilToInt(MatchTime);

            if (MatchTime > 348f && !fetchResults)
            {
                fetchResults = true;
                getWinners();
            }
        }
    }


    public void getWinners()
    {
        WinnerList = playerKillCounter
                        .OrderByDescending(entry => entry.Value)
                            .Take(3)
                                .Select(entry => entry.Key)
                                    .ToList();

        SetWinner();
    }

    /*[Rpc(RpcSources.All, RpcTargets.All)]
    public void TestRpc()
    {
        //stuff
    }*/
    public void SetWinner()
    {
        Debug.Log("setting winner values");
        /*if (rank == 1)
        {
            var firstPlace = WinnerList[0];
            return firstPlace;
        }

        else if (rank == 2)
        {
            if (WinnerList.Count > 1)
            {
                var secondPlace = WinnerList[1];  // 2nd highest
                return secondPlace;                              //Debug.Log($"2nd: {secondPlace.Key} with {secondPlace.Value} kills");
            }
            return null;
        }

        *//*if (WinnerList.Count > 1)
        {
            var secondPlace = WinnerList[1];  // 2nd highest
            //Debug.Log($"2nd: {secondPlace.Key} with {secondPlace.Value} kills");
        }*//*

        else if (rank == 3)
        {
            if (WinnerList.Count > 2)
            {
                var thirdPlace = WinnerList[2];   // 3rd highest
                return thirdPlace;                             //Debug.Log($"3rd: {thirdPlace.Key} with {thirdPlace.Value} kills");
            }
            return null;
        }
        *//*if (WinnerList.Count > 2)
        {
            var thirdPlace = WinnerList[2];   // 3rd highest
            //Debug.Log($"3rd: {thirdPlace.Key} with {thirdPlace.Value} kills");
        }*//*

        return null;*/


        MaterialList.Clear();
        NameList.Clear();

        for (int i = 0; i < WinnerList.Count; i++)
        {
            MaterialList.Add(materials[WinnerList[i].materialIndex]);
            NameList.Add(WinnerList[i].nickName.ToString());
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void GetPlayerKillCountRpc(NetworkPlayer playerObj, int killCount)
    {
        if(playerKillCounter.ContainsKey(playerObj))
            playerKillCounter[playerObj] = killCount;
        else
        {
            Debug.Log("player isnt added to the map Doing that rn");
            playerKillCounter.Add(playerObj, 0);
        }

        foreach (var entry in playerKillCounter)
        {
            Debug.Log(entry.Key.name + " -> " + entry.Value);
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void UpdatePlayerListRpc(NetworkPlayer playerObj)
    {
        if (!playerKillCounter.ContainsKey(playerObj))
            playerKillCounter.Add(playerObj, 0);

        foreach (var entry in playerKillCounter)
        {
            Debug.Log(entry.Key.name + " -> " + entry.Value);
        }
    }

    
}
