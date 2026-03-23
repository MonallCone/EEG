using UnityEngine;

/* * SCRIPT: NeuroMapper
 * PURPOSE: This is the "Translator". It takes data values from NeuroDataManager
 * and converts them into physical actions like scaling, rotating, or spawning objects.
 */

public class NeuroMapper : MonoBehaviour
{
    [Header("Data Connection")]
    public NeuroDataManager manager; // Reference to our data source

    [Tooltip("Enter the EXACT column name from your CSV (e.g., Met_Relaxation, Fac_Blink, etc.)")]
    public string columnName;

    // STUDENT: You can add new Action Types here if you want to create new behaviors!
    public enum ActionType { ScaleX_Remapped, SpawnObject, RotateX_Positive, RotateX_Negative }

    [Header("Action Selection")]
    public ActionType action;

    [Header("General Settings")]
    [Tooltip("Increases or decreases the intensity of the effect.")]
    public float multiplier = 1.0f;

    /* STUDENT: SMOOTHING explained
     * Higher value = faster response, but more shaky/noisy.
     * Lower value = slower response, but very smooth motion.
     */
    public float smoothing = 8.0f;

    [Tooltip("The data value must be higher than this to trigger actions like 'SpawnObject'.")]
    public float threshold = 0.5f;

    [Header("Scaling Options (For ScaleX_Remapped)")]
    public float minScale = 1.0f; // Minimum width when data is 0
    public float maxScale = 3.0f; // Maximum width when data is 1

    [Header("References")]
    [Tooltip("The object (Prefab) to create when using 'SpawnObject'.")]
    public GameObject prefabToSpawn;

    // Private variables for internal calculations
    private float smoothedValue;
    private float spawnTimer = 0;
    private Vector3 initialScale;

    void Start()
    {
        // Store the original size so we can reset or modify it correctly
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Guard: Stop if data manager is missing or column name is empty
        if (manager == null || string.IsNullOrEmpty(columnName)) return;

        // 1. DATA FILTERING (Smoothing)
        // We use Lerp to prevent the object from "teleporting" between values.
        float rawData = manager.GetValue(columnName);
        smoothedValue = Mathf.Lerp(smoothedValue, rawData, Time.deltaTime * smoothing);

        // 2. ACTION EXECUTION
        switch (action)
        {
            /* CASE: ScaleX_Remapped
             * Maps the 0.0 to 1.0 data range into your custom Min/Max width.
             */
            case ActionType.ScaleX_Remapped:
                // Mathf.Lerp(A, B, t) -> if t is 0, returns A. If t is 1, returns B.
                float targetWidth = Mathf.Lerp(minScale, maxScale, smoothedValue * multiplier);

                // STUDENT: CHANGE THIS if you want to scale Y or Z axis instead!
                transform.localScale = new Vector3(targetWidth, initialScale.y, initialScale.z);
                break;

            /* CASE: SpawnObject
             * Creates a new object when a specific threshold is met (like a Blink).
             */
            case ActionType.SpawnObject:
                spawnTimer += Time.deltaTime;

                // If data is above threshold AND enough time has passed (cooldown)
                if (rawData > threshold && spawnTimer > 0.4f)
                {
                    if (prefabToSpawn)
                    {
                        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
                        spawnTimer = 0; // Reset timer for the next spawn
                    }
                }
                break;

            /* CASE: RotateX (Positive/Negative)
             * Rotates the object around its X-axis based on the data value.
             */
            case ActionType.RotateX_Positive:
                // 80f is the maximum angle. Change this to rotate more or less.
                transform.localRotation = Quaternion.Euler(smoothedValue * 80f * multiplier, 0, 0);
                break;

            case ActionType.RotateX_Negative:
                transform.localRotation = Quaternion.Euler(smoothedValue * -80f * multiplier, 0, 0);
                break;
        }
    }
}