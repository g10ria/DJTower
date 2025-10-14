using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// This ensures the GameObject will have an AudioSource component.
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : NetworkBehaviour
{
    // Assign your sound effect in the Inspector.
    [SerializeField] private AudioClip[] triggerClips;
    private Dictionary<int, int> togglePlatIdtoClipId; // toggle platform ID to toggle audio clip ID
    [SerializeField] private AudioClip[] toggleClips;
    private float currentTime;

    private AudioSource triggerSource;
    private AudioSource[] toggleSources;

    private void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        triggerSource = GetComponent<AudioSource>();
        FindFirstObjectByType<GameState>().AssignAudioIds();
        toggleSources = new AudioSource[toggleClips.Length];
        for (int i = 0; i < toggleSources.Length; i++)
        {
            toggleSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    public void MapToggleAudios(Dictionary<int, int> map)
    {
        togglePlatIdtoClipId = map;
    }

    [ServerRpc]
    public void StartToggleAudiosServerRpc()
    {
        StartToggleAudiosClientRpc();
    }

    [ClientRpc]
    public void StartToggleAudiosClientRpc()
    {
        for (int i = 0; i < toggleClips.Length; i++)
        {
            toggleSources[i].clip = toggleClips[i];
            toggleSources[i].volume = 0f;
            toggleSources[i].Play();
        }
    }


    public float GetAudioLength(int platformId)
    {
        if (platformId >= 0 && platformId < triggerClips.Length)
            return triggerClips[platformId].length;
        return 0f;
    }

    // This is a Server Remote Procedure Call.
    // It is CALLED by a client, but EXECUTED on the server.
    [ServerRpc]
    public void PlaySoundServerRpc(int clipIndex, bool isTrigger)
    {
        // The server has received the request. Now, it tells all clients to play the sound.
        // We include the client who sent it so they hear the sound too.
        PlaySoundClientRpc(clipIndex, isTrigger);
    }

    // This is a Client Remote Procedure Call.
    // It is CALLED by the server, but EXECUTED on all connected clients.
    [ClientRpc]
    private void PlaySoundClientRpc(int clipIndex, bool isTrigger)
    {
        // A quick safety check to make sure the index is valid on this client.
        if (clipIndex < 0 || (clipIndex >= triggerClips.Length && isTrigger) || (clipIndex >= toggleClips.Length && !isTrigger))
        {
            Debug.LogWarning("Received an invalid sound clip index.");
            return;
        }
        
        if (isTrigger)
        {
        // Play the sound from the array at the specified index.
        triggerSource.PlayOneShot(triggerClips[clipIndex]);
        } else
        {
            toggleSources[clipIndex].volume = 1.0f - toggleSources[clipIndex].volume; // toggle volume between 0 and 1
        }
    }
}