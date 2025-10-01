using Unity.Netcode;
using UnityEngine;

public class PlayerRoleAssigner : NetworkBehaviour
{
    // OnNetworkSpawn is called when the player object is created in the network scene
    public override void OnNetworkSpawn()
    {
        // This check ensures this logic only runs on the player object you own
        if (!IsOwner) return;

        // The Host/Server always has a ClientId of 0
        if (OwnerClientId == 0)
        {
            // This is the Host (Architect)
            Debug.Log("I am the Architect!");
            GetComponent<ArchitectController>().enabled = true;
            GetComponent<PlayerMovement>().enabled = false;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            // This is a connecting Client (Jumper)
            Debug.Log("I am the Jumper!");
            GetComponent<ArchitectController>().enabled = false;
            GetComponent<PlayerMovement>().enabled = true;
        }
    }
}