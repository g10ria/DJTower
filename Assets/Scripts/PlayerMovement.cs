using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float jumpForce;

    private Rigidbody2D rb;
    private bool isGrounded;

    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private Vector3 respawnPosition = new Vector3(0, 2, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Move();

        if (transform.position.y < fallThreshold)
        {
            RespawnPlayer();
        }
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Flip player sprite if moving left/right
        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;
        
        // Check for checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            SetCheckpoint(other.transform.position);
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

    public void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
        Debug.Log("Checkpoint saved at: " + position);
    }
}
