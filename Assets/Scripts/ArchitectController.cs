using Unity.Netcode;
using UnityEngine;
using Enums;

public class ArchitectController : NetworkBehaviour
{
    public float bufferTime; // Small buffer to ensure platform stays active for the duration of the sound
    private GameState gameState;
    private PlayerAudio playerAudio;

    public override void OnNetworkSpawn()
    {
        // We only want the player who owns this object to be able to control it.
        if (!IsOwner) return;

        // Find the GameState once we're in the game.
        gameState = FindFirstObjectByType<GameState>();
        playerAudio = GetComponent<PlayerAudio>();

        playerAudio.StartToggleAudiosServerRpc();
    }

    void Update()
    {
        // Again, only the owner can give input.
        if (!IsOwner || gameState == null) return;

        KeyCode[] keyCodes = {KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3, KeyCode.Alpha4,
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V };

        for (int groupId = 0; groupId < keyCodes.Length; groupId++)
        {
            if (Input.GetKeyDown(keyCodes[groupId]))
            {
                if (groupId >= gameState.platformGroups.Count) continue;
                for (int platformId = 0; platformId < gameState.platformGroups.Count; platformId++)
                {
                    if (gameState.platformGroups[platformId] != groupId) continue;
                    if (gameState.platformTypes[groupId] == PlatformType.Toggle 
                    || gameState.platformTypes[groupId] == PlatformType.Trigger && gameState.platformStates[groupId] <= 0)
                    {
                        TogglePlatformServerRpc(platformId);
                    }   
                }
            }
        }
    }

    // An RPC (Remote Procedure Call) is a function a client can ask the server to run.
    [ServerRpc]
    private void TogglePlatformServerRpc(int platformId)
    {

        // This code ONLY runs on the server.
        if (platformId >= gameState.platformStates.Count) return;
        playerAudio.PlaySoundServerRpc(platformId, gameState.platformTypes[platformId] == PlatformType.Trigger);
        if (gameState.platformTypes[platformId] == PlatformType.Trigger)
        {
            float audioLength = playerAudio.GetAudioLength(platformId);
            // The server changes the value in the synchronized list.
            gameState.platformStates[platformId] = audioLength+bufferTime;
            // Netcode will automatically send this change to all clients!
        } else
        {
            gameState.platformStates[platformId] = gameState.platformStates[platformId] == 0f? 1f : 0f;
        }
    }
}