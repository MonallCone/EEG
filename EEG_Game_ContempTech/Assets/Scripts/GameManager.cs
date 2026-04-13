using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Main Settings")]
    public GameObject spherePrefab; // Place the sphere prefab here
    public GameObject playerBox;    // Place the player box here

    [Header("Speed & Spawning Settings")]
    public float speedIncreasePercentX = 10f; // Speed increase percentage (X)
    public float spawnInterval = 1.5f; // How often a sphere spawns (in seconds)
    public float currentSphereSpeed = 5f; // Initial speed of the spheres

    [Header("Score")]
    public int score = 0;

    void Start()
    {
        // Call the IncreaseSpeed function every 60 seconds
        InvokeRepeating("IncreaseSpeed", 60f, 60f);

        // Start the infinite spawning of spheres
        StartCoroutine(SpawnSpheres());
    }

    IEnumerator SpawnSpheres()
    {
        while (true)
        {
            // Spawn the sphere at the top of the screen with a random X position
            Vector3 spawnPosition = new Vector3(Random.Range(-3f, 3f), 6f, 0f);
            GameObject newSphere = Instantiate(spherePrefab, spawnPosition, Quaternion.identity);

            // Pass the game manager reference, current speed, and player's Y position to the sphere
            newSphere.GetComponent<SphereBehavior>().Initialize(this, currentSphereSpeed, playerBox.transform.position.y);

            // Wait for the specified interval before spawning the next sphere
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void IncreaseSpeed()
    {
        // Increase the current speed by X percent
        currentSphereSpeed += currentSphereSpeed * (speedIncreasePercentX / 100f);
        Debug.Log("Speed Increased! New Speed: " + currentSphereSpeed);
    }

    public void AddScore()
    {
        score += 10;
        Debug.Log("Score: " + score);
    }

    public void SubtractScore()
    {
        score -= 5;
        Debug.Log("Score: " + score);
    }
}