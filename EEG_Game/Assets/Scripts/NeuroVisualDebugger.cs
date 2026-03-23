using UnityEngine;
using UnityEngine.UI;

/* * SCRIPT: NeuroVisualDebugger
 * PURPOSE: This script creates a visual dashboard. 
 * It takes raw, noisy EEG data and smooths it out so we can monitor 
 * Brain Waves (Delta, Theta, Alpha, Beta) on the UI Sliders.
 */

public class NeuroVisualDebugger : MonoBehaviour
{
    [Header("Data Connection")]
    public NeuroDataManager dataManager; // Reference to our brain data streamer

    [Header("UI Slider References")]
    [Tooltip("Connect the UI Sliders from your Canvas to these slots.")]
    public Slider deltaSlider;
    public Slider thetaSlider;
    public Slider alphaSlider;
    public Slider betaSlider;

    [Header("Smoothing Speed")]
    [Tooltip("Controls how fast the sliders move. Higher = more responsive, Lower = smoother.")]
    public float uiSmoothing = 10.0f;

    void Update()
    {
        // Guard: If the data manager is missing, stop the script to prevent errors.
        if (!dataManager) return;

        // 1. DATA SMOOTHING (Filtering Jitter)
        // Raw neuro data at 250Hz is very "shaky". 
        // We use Mathf.Lerp to create a smooth transition between the current slider value 
        // and the new data point coming from the CSV.

        /* STUDENT: How Mathf.Lerp works here:
         * It calculates a value between 'current' and 'target' based on 'time'.
         * This prevents the slider from "flickering" or jumping instantly.
         */

        // Smoothing Delta Waves
        deltaSlider.value = Mathf.Lerp(deltaSlider.value, dataManager.GetValue("EEG_Delta"), Time.deltaTime * uiSmoothing);

        // Smoothing Theta Waves
        thetaSlider.value = Mathf.Lerp(thetaSlider.value, dataManager.GetValue("EEG_Theta"), Time.deltaTime * uiSmoothing);

        // Smoothing Alpha Waves
        alphaSlider.value = Mathf.Lerp(alphaSlider.value, dataManager.GetValue("EEG_Alpha"), Time.deltaTime * uiSmoothing);

        // Smoothing Beta Waves
        betaSlider.value = Mathf.Lerp(betaSlider.value, dataManager.GetValue("EEG_Beta"), Time.deltaTime * uiSmoothing);
    }
}