using UnityEngine;
using UnityEngine.UI;

/*
 * SCRIPT: NeuroVisualDebugger (The Diagnostics Dashboard)
 * PURPOSE: This is your primary calibration tool. It takes the chaotic, invisible 
 * raw brainwave frequencies and visualizes them as smooth UI sliders. 
 * * DESIGNER NOTE: Use this script to "see" what the player's brain is doing in real-time. 
 * The console logs generated here are crucial for figuring out exactly what numbers to type 
 * into the 'Threshold' and 'Multiplier' fields of your NeuroObjectMapper.
 */
public class NeuroVisualDebugger : MonoBehaviour
{
    [Header("--- 1. DATA CONNECTION ---")]
    [Tooltip("Drag the NeuroManager object here to supply the raw brain data.")]
    public NeuroDataManager dataManager;

    [Header("--- 2. UI SLIDER REFERENCES ---")]
    [Tooltip("Delta: Deep sleep / Unconscious mind.")]
    public Slider deltaSlider;

    [Tooltip("Theta: Deep relaxation / Meditation state.")]
    public Slider thetaSlider;

    [Tooltip("Alpha: Relaxed but awake. Spikes significantly when the player closes their eyes.")]
    public Slider alphaSlider;

    [Tooltip("Beta: Active thinking / Problem solving state.")]
    public Slider betaSlider;

    [Header("--- 3. VISUAL SETTINGS (Game Feel) ---")]
    [Range(1f, 30f)]
    [Tooltip("Controls UI responsiveness. Higher = snappy and accurate but jittery. Lower = smooth and organic.")]
    public float uiSmoothing = 10.0f;

    // Internal timer to prevent the Unity console from being flooded with messages every frame.
    private float logTimer = 0f;

    void Update()
    {
        // Guard clause: Do nothing if the data manager isn't connected.
        if (!dataManager) return;

        // 1. Fetch the absolute raw frequencies from the BCI bypass system.
        float delta = dataManager.GetValueFromBypass(BrainDataCategory.BandPower, "delta");
        float theta = dataManager.GetValueFromBypass(BrainDataCategory.BandPower, "theta");
        float alpha = dataManager.GetValueFromBypass(BrainDataCategory.BandPower, "alpha");
        float beta = dataManager.GetValueFromBypass(BrainDataCategory.BandPower, "beta");

        // 2. THE X-RAY SYSTEM (Designer Calibration Tool)
        // Prints the raw numbers to the Unity Console every 1 second.
        // Playtest your game and watch these numbers to determine your baseline thresholds!
        if (Time.time > logTimer && (alpha > 0 || theta > 0))
        {
            Debug.Log($"📊 [Dash X-Ray] Alpha: {alpha:F2} | Theta: {theta:F2} | Beta: {beta:F2}");
            logTimer = Time.time + 1f; // Reset timer for the next second
        }

        // 3. APPLY SMOOTHING TO THE UI
        // We use Lerp (Linear Interpolation) so the sliders glide gracefully to their new targets
        // instead of teleporting instantly, creating a highly polished, AAA UI feel.
        if (deltaSlider != null) deltaSlider.value = Mathf.Lerp(deltaSlider.value, delta, Time.deltaTime * uiSmoothing);
        if (thetaSlider != null) thetaSlider.value = Mathf.Lerp(thetaSlider.value, theta, Time.deltaTime * uiSmoothing);
        if (alphaSlider != null) alphaSlider.value = Mathf.Lerp(alphaSlider.value, alpha, Time.deltaTime * uiSmoothing);
        if (betaSlider != null) betaSlider.value = Mathf.Lerp(betaSlider.value, beta, Time.deltaTime * uiSmoothing);
    }
}