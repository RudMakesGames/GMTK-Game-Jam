using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkPlayer networkPlayerPrefab;
    NetworkRunnerHandler networkRunnerHandlerScript;

    private void Awake()
    {
        networkRunnerHandlerScript = FindObjectOfType<NetworkRunnerHandler>();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
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
        if (runner.IsServer)
        {
            Debug.Log("This is host");
            Vector3 spawnPoint = networkRunnerHandlerScript.spawnPoint != null ? networkRunnerHandlerScript.spawnPoint.position : Vector3.zero;
            runner.Spawn(networkPlayerPrefab.gameObject, spawnPoint, Quaternion.identity, player);
        }
        else
        {
            Debug.Log("Now clients joining");

        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
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
