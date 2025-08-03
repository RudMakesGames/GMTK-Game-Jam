using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkPlayer networkPlayerPrefab;
    [SerializeField] MatchManager matchManagerPrefab;
    private MatchManager matchManagerInstance;
    NetworkRunnerHandler networkRunnerHandlerScript;

    //PlayerRef playerToSpawn;

    private void Awake()
    {
        networkRunnerHandlerScript = FindObjectOfType<NetworkRunnerHandler>();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        /*if (runner.IsServer)
        {
            Debug.Log("Connected to server spawnnign player");
            Vector3 spawnPoint = networkRunnerHandlerScript.spawnPoint != null ? networkRunnerHandlerScript.spawnPoint.position : Vector3.zero;
            runner.Spawn(networkPlayerPrefab.gameObject, spawnPoint, Quaternion.identity, playerToSpawn);
        }
        else
        {
            Debug.Log("Now clients joining");

        }*/
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host disconnected migrationg in progress");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if(NetworkPlayer.Local!=null)
        {
            //Debug.Log("assigned to " + NetworkPlayer.Local.name);
            input.Set(NetworkPlayer.Local.GetNetworkInput());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //playerToSpawn = player;
        /*if (runner.IsServer)
        {
            *//*Debug.Log("This is host");
            Vector3 spawnPoint = networkRunnerHandlerScript.spawnPoint != null ? networkRunnerHandlerScript.spawnPoint.position : Vector3.zero;
            runner.Spawn(networkPlayerPrefab.gameObject, spawnPoint, Quaternion.identity, player);*//*
        }
        else
        {
            Debug.Log("Now clients joining");
            Vector3 spawnPoint = networkRunnerHandlerScript.spawnPoint != null ? networkRunnerHandlerScript.spawnPoint.position : Vector3.zero;
            runner.Spawn(networkPlayerPrefab.gameObject, spawnPoint, Quaternion.identity, player);
        }*/

        if (player == Runner.LocalPlayer)
        {
            Vector3 spawnPoint = networkRunnerHandlerScript.spawnPoint != null ? networkRunnerHandlerScript.spawnPoint.position : Vector3.zero;
            runner.Spawn(networkPlayerPrefab.gameObject, spawnPoint, Quaternion.identity, player);

            if(CharacterSelector.selectedMode!=1)
            {
                runner.Spawn(matchManagerPrefab, Vector3.zero, Quaternion.identity, player);
            }
            //spawnnedPlayer.GetComponent<Renderer>().material = CharacterSelector.selectedMat;
        }

        matchManagerInstance = GameObject.FindObjectOfType<MatchManager>();

        if (runner.IsSharedModeMasterClient && matchManagerPrefab != null && matchManagerInstance==null)
        {
            runner.Spawn(matchManagerPrefab, Vector3.zero, Quaternion.identity, player);
        }
        else if (!runner.IsSharedModeMasterClient) Debug.Log("No timer for you lil boy");

        /*if(runner.IsSharedModeMasterClient)*/
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsSharedModeMasterClient)
        {
            Debug.Log("I am now the new Shared Mode Master Client");
            var matchManagerNewInstance = FindObjectOfType<MatchManager>();

            if (matchManagerNewInstance != null && matchManagerNewInstance.HasInputAuthority == false)
            {
                matchManagerInstance.GetComponent<NetworkObject>().AssignInputAuthority(runner.LocalPlayer);
            }
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
