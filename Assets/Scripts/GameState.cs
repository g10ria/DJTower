using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public List<float> platformStates = new List<float>();

    private void Awake()
    {
        for (int i = 0; i < 16; i++)
                platformStates.Add(0f);
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
    }
    
    public void SetPlatformState(int platformId, float time)
    {
        if (platformId >= 0 && platformId < platformStates.Count)
            platformStates[platformId] = time;
    }
}