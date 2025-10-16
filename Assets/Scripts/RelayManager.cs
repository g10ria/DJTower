using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;


public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    string player1Username;
    string player2Username;

    // These fields are still fine.
    public GameObject Player;
    public GameObject Architect;

    public string[] levels = {"Snoc"};

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private UnityTransport transport;
    // This string will be "wss" for WebGL builds and "dtls" for all other platforms
    private string connectionType = "wss";
    async void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // This is a preprocessor directive that will only compile the code inside
        // if the build target is WebGL
#if UNITY_WEBGL
            connectionType = "wss";
        // Also enable WebSockets in the transport for WebGL
        transport.UseWebSockets = true;
#else
        transport.UseWebSockets = false;
#endif

        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in as player {AuthenticationService.Instance.PlayerId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Unity Services init failed: {e}");
        }

        Debug.Log("started relay manager");
    }

    // CreateRelay starts a relay session and returns the allocation ID (gamecode)
    public async Task<string> CreateRelay()
    {
        try
        {
            Debug.Log("Creating Relay Allocation...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            Debug.Log("Allocation created!");

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Got join code: {joinCode}");

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, connectionType);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Relay service error: {e.Message}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Unexpected error creating relay: {e}");
            return null;
        }
    }

    // JoinRelay attempts to join a session with the provided gamecode and returns
    // if it is successful or not
    // TODO: check for failures. for now, returning true all the time
    public async Task<bool> JoinRelay(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, connectionType);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

        return true;
    }

    public void SetPlayerUsernames(string player1Username, string player2Username)
    {
        this.player1Username = player1Username;
        this.player2Username = player2Username;
    }

    public void SpawnPlayer()
    {
        const ulong clientId = NetworkManager.ServerClientId;
        Debug.Log("spawning player; serverclient id is " + clientId);
        GameObject prefabToSpawn = (clientId == 0) ? Architect : Player;
        Debug.Log(prefabToSpawn);
        GameObject playerInstance = Instantiate(prefabToSpawn);
        // // Important: Make sure the player instance is spawned at a valid position.
        // playerInstance.transform.position = new Vector3(0, 2, 0); // Example spawn position
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Invalid level index");
            return;
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            Debug.Log("only hosts can start levels for now");
            return;
        }

        string levelName = levels[levelIndex];
        Debug.Log("Loading networked level: " + levelName);

        // Subscribe to the OnLoadComplete before starting scene load
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        Debug.Log($"Scene '{sceneName}' loaded for client {clientId}");

        if (!NetworkManager.Singleton.IsServer)
            return;

        // Spawn Architect for host, Player for client(s)
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
                continue;

            GameObject prefab = client.ClientId == NetworkManager.Singleton.LocalClientId
                ? Architect
                : Player;

            GameObject instance = Instantiate(prefab);
            instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);

            Debug.Log($"Spawned {prefab.name} for client {client.ClientId}");
        }

        // Only need this once per load
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }
}
