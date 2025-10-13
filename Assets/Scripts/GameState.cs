using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode; 
using UnityEngine;
using Enums;

// A wrapper for PlatformType enum in order to make it network serializable


public class GameState : NetworkBehaviour
{
    // A NetworkList is a list of values that is automatically synchronized
    // from the server to all clients. Perfect for our platform states!
    // We initialize it with a few platforms for testing.
    

    /// <summary>
    /// Contains either:
    /// - If a `Toggle` type: 0.0 if the platform is disabled, 1.0 if enabled
    /// - If a `Trigger` type: the time remaining for the platform to be enabled
    /// </summary>
    public NetworkList<float> platformStates;
    /// <summary>
    /// A parallel list to `platformStates` that stores the type of platform
    /// </summary>
    public NetworkList<PlatformTypeNet> platformTypes;
    public NetworkList<int> platformGroups;

    
    public struct PlatformTypeNet : INetworkSerializable, IEquatable<PlatformTypeNet>
{
    public PlatformType Value;

    public PlatformTypeNet(PlatformType v) { Value = v; }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        byte tmp = (byte)Value;
        serializer.SerializeValue(ref tmp);
        if (serializer.IsReader) Value = (PlatformType)tmp;
    }

    public bool Equals(PlatformTypeNet other) => Value == other.Value;
    public override int GetHashCode() => (int)Value;
    public override bool Equals(object obj) => obj is PlatformTypeNet o && Equals(o);

    public static implicit operator PlatformType(PlatformTypeNet w) => w.Value;
    public static implicit operator PlatformTypeNet(PlatformType v) => new PlatformTypeNet(v);
}


    private void Awake()
    {
        // Initialize the list. This only needs to be done once.
        platformStates = new NetworkList<float>();
        platformTypes = new NetworkList<PlatformTypeNet>();
        platformGroups = new NetworkList<int>();
    }

    public void Update()
    {
        for (int i = 0; i < platformStates.Count; i++) {
            if (platformStates[i] > 0 && platformTypes[i] == PlatformType.Trigger)
            {
                platformStates[i] = math.max(0, platformStates[i] - Time.deltaTime);
            }
        }
    }

    // We want the server to set the initial state when it starts.
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlatformController[] platforms = FindObjectsByType<PlatformController>(FindObjectsSortMode.None);
            Array.Sort(platforms, (a, b) => a.platformId.CompareTo(b.platformId));

            foreach (PlatformController platform in platforms)
            {
                platformStates.Add(0f);
                platformTypes.Add(platform.type);
                platformGroups.Add(platform.platformGroup);
            } 
        }
    }

    public void AssignAudioIds()
    {
        // add platform ID to audio ID mappings
        PlayerAudio playerAudio = FindFirstObjectByType<PlayerAudio>();
        Dictionary<int, int> platToAudio = new Dictionary<int, int>();
        int audioId = 0;
        for (int platformId = 0; platformId < platformTypes.Count; platformId++)
        {
            if (platformTypes[platformId] == PlatformType.Toggle)
            {
                platToAudio[platformId] = audioId;
            }
        }
        playerAudio.MapToggleAudios(platToAudio);
    }
}