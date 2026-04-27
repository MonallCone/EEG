using UnityEngine;

public class SphereBehavior : MonoBehaviour
{
    private float speed;
    private GameManager gameManager;
    private float boxYPosition;
    private bool hasPassedBox = false;

    // This function is called by the GameManager when the sphere is spawned
    public void Initialize(GameManager manager, float moveSpeed, float playerYPos)
    {
        gameManager = manager;
        speed = moveSpeed;
        boxYPosition = playerYPos;
    }

    void Update()
    {
        // Move the sphere downwards
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Check if the sphere has passed the box (sphere's Y position is lower than the box)
        if (!hasPassedBox && transform.position.y < (boxYPosition - 2f))
        {
            hasPassedBox = true;
            gameManager.SubtractScore(); // Deduct 5 points
            Destroy(gameObject, 1f); // Destroy the sphere shortly after it passes
        }
    }

    // Detect collision between the sphere and the player box
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.AddScore(); // Add 10 points
            Destroy(gameObject); // Destroy the sphere immediately after collision
        }
    }
}