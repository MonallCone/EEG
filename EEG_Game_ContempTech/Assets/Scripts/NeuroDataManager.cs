using UnityEngine;
using EmotivUnityPlugin;
using System.Collections;

/*
 * SCRIPT: NeuroDataManager (The Data Hub)
 * PURPOSE: This is the engine room. It safely extracts raw data from the headset 
 * and provides it to your game objects. 
 * DESIGNER NOTE: You do NOT need to edit this script. Just attach it to your NeuroManager object.
 */
public class NeuroDataManager : MonoBehaviour
{
    // Safe storage for our background data streams
    private float _latestRelaxation = 0f;
    private float _latestFocus = 0f;
    private float _latestStress = 0f;
    private float _latestTheta, _latestAlpha, _latestLowBeta, _latestHighBeta;

    void Start()
    {
        // Listen to the live data feeds as soon as the game starts
        DataStreamProcess.Instance.BandPowerDataReceived += OnRawBandPowerReceived;
        DataStreamProcess.Instance.PerfDataReceived += OnRawPerformanceDataReceived;
    }

    void OnDestroy()
    {
        // Clean up listeners when the game stops to prevent memory leaks
        DataStreamProcess.Instance.BandPowerDataReceived -= OnRawBandPowerReceived;
        DataStreamProcess.Instance.PerfDataReceived -= OnRawPerformanceDataReceived;
    }

    /*
     * MASTER FUNCTION: GetValueFromBypass
     * Other scripts (like NeuroObjectMapper) call this to get the current brain value.
     */
    public float GetValueFromBypass(BrainDataCategory category, string signalName, int sensorIndex = 0)
    {
        var itf = EmotivUnityItf.Instance;
        if (itf == null || !itf.IsSessionCreated) return 0f;

        string query = signalName.ToLower();

        switch (category)
        {
            case BrainDataCategory.PerformanceMetrics:
                // Emotional metrics (Usually returns a float between 0.0 and 1.0)
                if (query == "relaxation") return _latestRelaxation;
                if (query == "attention") return _latestFocus;
                if (query == "stress") return _latestStress;
                return (float)itf.GetPMData(query);

            case BrainDataCategory.FacialExpressions:
                // Facial movements (Returns 1 for instant actions like blinks, or 0.0 to 1.0 for smiles)
                if (query == "blink") return (itf.curEyeAct == "blink" ? 1f : 0f);
                if (query == "smile") return (itf.curLAct == "smile" ? (float)itf.curLPow : 0f);
                if (query == "clench") return (itf.curLAct == "clench" ? (float)itf.curLPow : 0f);
                if (query == "surprise") return (itf.curUAct == "surprise" ? (float)itf.curUPow : 0f);
                if (query == "frown") return (itf.curUAct == "frown" ? (float)itf.curUPow : 0f);
                break;

            case BrainDataCategory.MentalCommands:
                // Conscious thoughts trained by the player (Returns 0.0 to 1.0)
                string act = itf.LatestMentalCommand.act;
                float pow = (float)itf.LatestMentalCommand.pow;
                if (act == query) return pow;
                break;

            case BrainDataCategory.BandPower:
                // Raw brainwave frequencies (Returns higher dynamic numbers, often > 50 or 100)
                if (query == "theta") return _latestTheta;
                if (query == "alpha") return _latestAlpha;
                if (query == "beta") return _latestHighBeta;
                if (query == "delta") return _latestLowBeta;
                break;
        }

        return 0f; // Default fallback
    }

    // --- BACKGROUND WORKERS ---
    // These methods continuously catch the raw data arrays from the headset in the background.

    private void OnRawPerformanceDataReceived(object sender, ArrayList data)
    {
        // Extracting emotional data from specific array slots
        if (data != null && data.Count >= 14)
        {
            _latestStress = System.Convert.ToSingle(data[6]);
            _latestRelaxation = System.Convert.ToSingle(data[9]);
            _latestFocus = System.Convert.ToSingle(data[13]);
        }
    }

    private void OnRawBandPowerReceived(object sender, ArrayList data)
    {
        // Extracting raw frequency waves (Theta, Alpha, Beta)
        if (data != null && data.Count >= 5)
        {
            _latestTheta = System.Convert.ToSingle(data[1]);
            _latestAlpha = System.Convert.ToSingle(data[2]);
            _latestLowBeta = System.Convert.ToSingle(data[3]);
            _latestHighBeta = System.Convert.ToSingle(data[4]);
        }
    }
}