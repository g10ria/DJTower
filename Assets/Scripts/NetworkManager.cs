using UnityEngine;
using Unity.Netcode;

public class PersistentNetworkManager : MonoBehaviour
{
    private void Awake()
    {
        var nm = GetComponent<NetworkManager>();
        if (nm == null)
        {
            Debug.LogError("NetworkManager component missing!");
            return;
        }

        // Prevent duplicates
        if (NetworkManager.Singleton != null && NetworkManager.Singleton != nm)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
