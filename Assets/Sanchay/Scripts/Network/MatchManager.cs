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

    [Networked]
    public string thirdPlace {  get; set; }

    [Networked]
    public string secondPlace { get; set; }

    [Networked]
    public string Winner { get; set; }

    [Networked]
    public int thirdPlaceMatIndex { get; set; }

    [Networked]
    public int secondPlaceMatIndex { get; set; }

    [Networked]
    public int WinnerMatIndex { get; set; }





    [SerializeField] float BalloonRotSpeed = 0.5f;

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

            if (MatchTime > 347f && !fetchResults) //348 tha iska
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

        Invoke("GettingWinnerListDelayed", 2f);

    }


    void GettingWinnerListDelayed()
    {
        this.SetWinnerRPC();
    }

    /*[Rpc(RpcSources.All, RpcTargets.All)]
    public void TestRpc()
    {
        //stuff
    }*/

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetWinnerRPC()
    {
        //if (!Object.HasInputAuthority) return;


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


        /*MaterialList.Clear();
        NameList.Clear();*/

        Debug.Log(WinnerList.Count);

        for (int i = 0; i < WinnerList.Count; i++)
        {
            Debug.Log("going for the" + i + " value");
            MaterialList.Add(materials[WinnerList[i].materialIndex]);
            NameList.Add(WinnerList[i].nickName.ToString());
        }

        if (WinnerList.Count >= 3)
        {
            thirdPlace = NameList[2]; secondPlace = NameList[1]; Winner = NameList[0];
            thirdPlaceMatIndex = WinnerList[2].materialIndex; secondPlaceMatIndex = WinnerList[1].materialIndex; WinnerMatIndex = WinnerList[0].materialIndex;
        }
        else if(WinnerList.Count >= 2)
        {
            secondPlace = NameList[1]; Winner = NameList[0];
            secondPlaceMatIndex = WinnerList[1].materialIndex; WinnerMatIndex = WinnerList[0].materialIndex;
        }
        else
        {
            Winner = NameList[0];
            WinnerMatIndex = WinnerList[0].materialIndex;
        }
        
        

        Debug.Log("thirdPlace-> "+thirdPlace+" and mat->" +thirdPlaceMatIndex);
        Debug.Log("secondPlace-> " + secondPlace + " and mat->" + secondPlaceMatIndex);
        Debug.Log("Winner-> " + Winner + " and mat->" + WinnerMatIndex);

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
