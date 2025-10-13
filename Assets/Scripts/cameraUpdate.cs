using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public string playerTag = "Player"; // Tag to identify player 2
    public Vector3 offset = new Vector3(0, 5, -10); // Camera offset from player
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement

    private Transform playerTransform;

    void Start()
    {
        // Find player 2 in the scene
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("Camera found player: " + player.name);
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure player has tag: " + playerTag);
        }
    }

    void LateUpdate()
    {
        // If player hasn't spawned yet, try to find it
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        // Calculate desired position
        Vector3 desiredPosition = playerTransform.position + offset;

        // Smoothly move camera to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Optional: Make camera look at player
        // transform.LookAt(playerTransform);
    }
}