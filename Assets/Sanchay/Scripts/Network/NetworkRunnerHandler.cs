using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using Fusion.Sockets;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField]
    NetworkRunner networkRunnerPrefab;

    public
    Transform spawnPoint;

    NetworkRunner networkRunner;

    private void Awake()
    {
        networkRunner = FindObjectOfType<NetworkRunner>();
    }

    private void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network Runner";
        }

        GameMode mode2 = CharacterSelector.selectedMode == 1 ? GameMode.Shared : GameMode.Single;
        string lobbyName = CharacterSelector.selectedMode == 1 ? "RoomSessionTest" : UnityEngine.Random.Range(100, 999).ToString();

        var clientTask = InitializeNetworkRunner(networkRunner, mode2, lobbyName, NetAddress.Any(), SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), null);
        Debug.Log("Network Runner Initialized with mode as " + mode2 +"and lobby as "+lobbyName);
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        INetworkSceneManager sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        if(sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner networkRunner, GameMode gameMode, string sessionName, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        INetworkSceneManager sceneManager = GetSceneManager(networkRunner);
        networkRunner.ProvideInput = true;

        return networkRunner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "TestLobby",
            SceneManager = sceneManager
        });
    }
}
