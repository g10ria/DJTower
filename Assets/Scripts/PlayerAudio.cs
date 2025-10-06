using Unity.Netcode;
using UnityEngine;

// This ensures the GameObject will have an AudioSource component.
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : NetworkBehaviour
{
    // Assign your sound effect in the Inspector.
    [SerializeField] private AudioClip[] clips;

    private AudioSource audioSource;

    private void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        audioSource = GetComponent<AudioSource>();
    }
    // This is a Server Remote Procedure Call.
    // It is CALLED by a client, but EXECUTED on the server.
    [ServerRpc]
    public void PlaySoundServerRpc(int clipIndex)
    {
        // The server has received the request. Now, it tells all clients to play the sound.
        // We include the client who sent it so they hear the sound too.
        PlaySoundClientRpc(clipIndex);
    }

    // This is a Client Remote Procedure Call.
    // It is CALLED by the server, but EXECUTED on all connected clients.
    [ClientRpc]
    private void PlaySoundClientRpc(int clipIndex)
    {
        // A quick safety check to make sure the index is valid on this client.
        if (clipIndex < 0 || clipIndex >= clips.Length)
        {
            Debug.LogWarning("Received an invalid sound clip index.");
            return;
        }
        
        // Play the sound from the array at the specified index.
        audioSource.PlayOneShot(clips[clipIndex]);
    }
}