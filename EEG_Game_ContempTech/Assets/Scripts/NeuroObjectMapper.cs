using UnityEngine;
using UnityEngine.Events;

// Creates a custom Unity Event that can pass a float value (the brain data) to other components.
[System.Serializable] public class NeuroFloatEvent : UnityEvent<float> { }

/*
 * SCRIPT: NeuroObjectMapper (The Ultimate Designer Bridge)
 * PURPOSE: This is your primary tool for creating Brain-Computer Interface (BCI) mechanics.
 * It takes raw data from the NeuroDataManager and translates it into physical gameplay 
 * (movement, spawning, or triggering other scripts) WITHOUT requiring any code.
 * * DESIGNER NOTE: Attach this script to any object you want to be controlled by the player's mind.
 */
public class NeuroObjectMapper : MonoBehaviour
{
    [Header("--- 1. DATA SOURCE ---")]
    [Tooltip("Drag the NeuroManager object from your scene here to establish the connection.")]
    public NeuroDataManager dataManager;

    [Header("--- 2. INPUT SIGNAL ---")]
    // Note: The custom Editor script (NeuroObjectMapperEditor) automatically hides 
    // the irrelevant dropdowns below based on your selected Data Category.
    public BrainDataCategory dataCategory = BrainDataCategory.PerformanceMetrics;
    public BrainMetrics metricSignal = BrainMetrics.Relaxation;
    public BrainFacial facialSignal = BrainFacial.Blink;
    public BrainMental mentalSignal = BrainMental.Push;
    public BrainWaveType waveType = BrainWaveType.Alpha;
    public EpocX_Sensor bandPowerTarget = EpocX_Sensor.AF3;

    [Header("--- 3. GENERAL SETTINGS (Game Feel) ---")]
    [Tooltip("Sensitivity: Increases or decreases the intensity of the brain signal.")]
    public float multiplier = 1.0f;

    [Tooltip("Base Value: Adds a flat amount to the final signal. Good for setting minimum limits.")]
    public float offset = 0.0f;

    [Tooltip("Flips the logic. E.g., The object shrinks when you focus, instead of growing.")]
    public bool invert = false;

    [Space(5)]
    [Tooltip("Converts smooth, continuous data (0.0 to 1.0) into a strict ON/OFF switch (0 or 1).")]
    public bool useThreshold = false;

    [Range(0f, 1f)]
    [Tooltip("The signal must pass this value to trigger. Higher = harder for the player to activate.")]
    public float threshold = 0.5f;

    [Header("--- 4. TRANSFORM MAPPING (Movement) ---")]
    [Tooltip("The object you want to move/scale/rotate. Usually this same GameObject.")]
    public Transform targetTransform;
    public enum MappingMode { None, Scale, Position, Rotation }
    public MappingMode mode = MappingMode.None;
    public bool x, y, z;

    [Range(0, 20f)]
    [Tooltip("Higher = snappy and robotic. Lower = floaty and smooth. Adjust for Game Feel.")]
    public float smoothSpeed = 10f;

    [Header("--- 5. SPAWN MAPPING (Instancing) ---")]
    [Tooltip("Enable to spawn an object. Perfect for instant triggers like 'Blinking' or 'Push'.")]
    public bool enableSpawning = false;
    public GameObject prefabToSpawn;

    [Tooltip("Time in seconds before the player can spawn another object. Prevents spamming/lag.")]
    public float spawnCooldown = 0.5f;
    private float spawnTimer = 0f;

    [Header("--- 6. UNITY EVENT (The Magic Port) ---")]
    [Tooltip("Link the brain signal to ANY component here (e.g., Light intensity, Audio volume, UI Sliders).")]
    public NeuroFloatEvent onUpdate;

    void Update()
    {
        // Guard: Do nothing if the manager isn't connected
        if (dataManager == null) return;

        string query = "";
        int sensorIdx = 0;

        // 1. Determine exactly what data the designer requested in the Inspector
        switch (dataCategory)
        {
            case BrainDataCategory.PerformanceMetrics: query = metricSignal.ToString(); break;
            case BrainDataCategory.FacialExpressions: query = facialSignal.ToString(); break;
            case BrainDataCategory.MentalCommands: query = mentalSignal.ToString(); break;
            case BrainDataCategory.BandPower:
                query = waveType.ToString();
                sensorIdx = (int)bandPowerTarget;
                break;
        }

        // 2. Fetch the raw data from the core engine
        float rawVal = dataManager.GetValueFromBypass(dataCategory, query, sensorIdx);

        // 3. Apply Designer Modifiers (Invert & Threshold)
        if (invert) rawVal = 1.0f - rawVal;

        if (useThreshold)
        {
            // Binary clamp: If it passes the threshold, it's 100% ON. Otherwise, completely OFF.
            rawVal = (rawVal >= threshold) ? 1.0f : 0.0f;
        }

        // Apply final sensitivity and base offset
        float finalVal = (rawVal * multiplier) + offset;

        // 4. BROADCAST TO UNITY EVENTS (Lights, Sounds, UI, etc.)
        onUpdate?.Invoke(finalVal);

        // 5. APPLY PHYSICAL MOVEMENT (Transform)
        if (targetTransform != null && mode != MappingMode.None)
        {
            ProcessTransformMapping(finalVal);
        }

        // 6. APPLY SPAWNING LOGIC
        if (enableSpawning && prefabToSpawn != null)
        {
            spawnTimer += Time.deltaTime;

            // Check if the signal passes the required threshold AND the cooldown timer is ready
            float activationTarget = useThreshold ? 0.5f : threshold;

            if (rawVal > activationTarget && spawnTimer >= spawnCooldown)
            {
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
                spawnTimer = 0f; // Reset cooldown
            }
        }
    }

    /*
     * FUNCTION: ProcessTransformMapping
     * Handles the physical manipulation of the object in the 3D world.
     */
    private void ProcessTransformMapping(float val)
    {
        // Get the current state of the object
        Vector3 current = (mode == MappingMode.Scale) ? targetTransform.localScale :
                         (mode == MappingMode.Position) ? targetTransform.localPosition :
                          targetTransform.localEulerAngles;

        // Interpolate (Lerp) towards the new value for smooth, organic visual feedback
        if (x) current.x = Mathf.Lerp(current.x, val, Time.deltaTime * smoothSpeed);
        if (y) current.y = Mathf.Lerp(current.y, val, Time.deltaTime * smoothSpeed);
        if (z) current.z = Mathf.Lerp(current.z, val, Time.deltaTime * smoothSpeed);

        // Apply the newly calculated Vector3 back to the object
        if (mode == MappingMode.Scale) targetTransform.localScale = current;
        else if (mode == MappingMode.Position) targetTransform.localPosition = current;
        else if (mode == MappingMode.Rotation) targetTransform.localEulerAngles = current;
    }
}