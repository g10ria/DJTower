using Unity.Netcode;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    // Assign this in the Inspector for each platform (0, 1, 2, etc.)
    public int platformId;

    private GameState gameState;
    private SpriteRenderer spriteRenderer; // Example: change color
    private Collider2D platformCollider; // Example: disable collision

    void Start()
    {
        // Find the one and only GameState object in the scene.
        gameState = FindObjectOfType<GameState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        if (gameState == null)
        {
            Debug.LogError("GameState object not found in the scene!");
        }
    }

    void Update()
    {
        // If the GameState is ready and we have a valid ID...
        if (gameState != null && platformId < gameState.platformStates.Count)
        {
            // Read the boolean value for this platform from the synchronized list.
            bool isEnabled = gameState.platformStates[platformId];
            
            if (spriteRenderer != null) spriteRenderer.enabled = isEnabled;
            if (platformCollider != null) platformCollider.enabled = isEnabled;
        }
    }
}