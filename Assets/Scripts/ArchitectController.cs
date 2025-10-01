using Unity.Netcode;
using UnityEngine;

public class ArchitectController : NetworkBehaviour
{
    private GameState gameState;

    public override void OnNetworkSpawn()
    {
        // We only want the player who owns this object to be able to control it.
        if (!IsOwner) return;

        // Find the GameState once we're in the game.
        gameState = FindObjectOfType<GameState>();
    }

    void Update()
    {
        // Again, only the owner can give input.
        if (!IsOwner || gameState == null) return;

        // Example Input: Press '1' to toggle platform 0, '2' for platform 1, etc.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TogglePlatformServerRpc(0); // Request to toggle platform 0
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TogglePlatformServerRpc(1); // Request to toggle platform 1
        }

    }

    // An RPC (Remote Procedure Call) is a function a client can ask the server to run.
    [ServerRpc]
    private void TogglePlatformServerRpc(int platformId)
    {
        // This code ONLY runs on the server.
        if (platformId < gameState.platformStates.Count)
        {
            // The server changes the value in the synchronized list.
            gameState.platformStates[platformId] = !gameState.platformStates[platformId];
            // Netcode will automatically send this change to all clients!
        }
    }
}