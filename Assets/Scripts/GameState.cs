using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Settings;

public class GameState : NetworkBehaviour
{
    // A NetworkList is a list of values that is automatically synchronized
    // from the server to all clients. Perfect for our platform states!
    // We initialize it with a few platforms for testing.
    public NetworkList<float> platformStates;

    private void Awake()
    {
        // Initialize the list. This only needs to be done once.
        platformStates = new NetworkList<float>();
    }

    public void Update()
    {
        for (int i = 0; i < platformStates.Count; i++)
        {
            if (platformStates[i] > 0)
            {
                platformStates[i] = math.max(0, platformStates[i] - Time.deltaTime);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Config.TESTING = true;
            Debug.Log($"Testing mode: {Config.TESTING}");
            foreach (var netObj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
            {
                netObj.ChangeOwnership(0); // 0 = host’s client ID
            }

        }

    }

    // We want the server to set the initial state when it starts.
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int i = 0; i < 16; i++)
                platformStates.Add(0f);
                
        }
    }
}