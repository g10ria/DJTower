using Unity.Netcode;
using UnityEngine;

public class GameState : NetworkBehaviour
{
    // A NetworkList is a list of values that is automatically synchronized
    // from the server to all clients. Perfect for our platform states!
    // We initialize it with a few platforms for testing.
    public NetworkList<bool> platformStates;

    private void Awake()
    {
        // Initialize the list. This only needs to be done once.
        platformStates = new NetworkList<bool>();
    }

    // We want the server to set the initial state when it starts.
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            platformStates.Add(true); // Platform 0 is OFF
            platformStates.Add(false); // Platform 1 is OFF
        }
    }
}