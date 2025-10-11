using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

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

    public enum PlatformType : byte
    {
        Toggle,
        Trigger,
    }
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
            for (int i = 0; i < 16; i++)
                platformStates.Add(0f);
            for (int i = 0; i < 16; i++) // need to update
                platformTypes.Add(PlatformType.Trigger);
            for (int i = 0; i < 16; i++)
                platformGroups.Add(0);
        }
    }
}