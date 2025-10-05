using Unity.Netcode;
using UnityEngine;

public class PlayerRespawn : NetworkBehaviour
{
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private Vector3 respawnPosition = new Vector3(0, 2, 0);

    private void Update()
    {
        // Wait until the NetworkObject is spawned
        //if (!IsSpawned) return;

        // Only check on the server to avoid duplicate respawns
        //if (!IsServer) return;

        if (transform.position.y < fallThreshold)
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        transform.position = respawnPosition;
        Debug.Log($"Player {OwnerClientId} respawned");

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}