using Unity.Netcode;
using UnityEngine;

// Change this line from NetworkBehaviour to MonoBehaviour.
// This allows it to live on the same GameObject as the NetworkManager.
public class PlayerSpawner : MonoBehaviour
{
    // These fields are still fine.
    public GameObject Player;
    public GameObject Architect;

    // Use Unity's regular Start() method.
    private void Start()
    {
        // We must wait for the NetworkManager to be initialized.
        // We'll hook our logic into its OnServerStarted event.
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    }

    private void HandleServerStarted()
    {
        // This logic now runs only after the server has successfully started.
        // Check if we are the host/server.
        if (NetworkManager.Singleton.IsServer)
        {
            // Hook into the connection event.
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            // Spawn the host player (client ID 0) immediately.
            SpawnPlayer(NetworkManager.ServerClientId);
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        // Don't spawn the host player again.
        if (clientId == NetworkManager.ServerClientId)
        {
            return;
        }
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        GameObject prefabToSpawn = (clientId == 0) ? Architect : Player;
        Debug.Log(prefabToSpawn);
        GameObject playerInstance = Instantiate(prefabToSpawn);
        // // Important: Make sure the player instance is spawned at a valid position.
        // playerInstance.transform.position = new Vector3(0, 2, 0); // Example spawn position
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    // It's good practice to un-subscribe from events when the object is destroyed.
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }
}