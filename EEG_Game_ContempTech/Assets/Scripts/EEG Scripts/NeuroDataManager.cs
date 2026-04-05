using UnityEngine;
using EmotivUnityPlugin;
using System.Collections;

/*
 * SCRIPT: NeuroDataManager (The Data Hub & Simulator)
 * PURPOSE: Extracts real data from the headset, OR generates fake data for testing.
 * DESIGNER NOTE: Turn on "Simulation Mode" to test your game at home without a headset!
 */
public class NeuroDataManager : MonoBehaviour
{
    [Header("--- SIMULATION MODE (No Headset Needed) ---")]
    [Tooltip("Check this box to disconnect from Emotiv and use the sliders below to fake brain data.")]
    public bool simulationMode = false;

    [Header("1. Fake Performance Metrics")]
    [Range(0f, 1f)] public float simAttention = 0f;
    [Range(0f, 1f)] public float simRelaxation = 0f;
    [Range(0f, 1f)] public float simStress = 0f;

    [Header("2. Fake Facial Expressions")]
    [Range(0f, 1f)] public float simBlink = 0f;
    [Range(0f, 1f)] public float simSmile = 0f;
    [Range(0f, 1f)] public float simClench = 0f;
    [Range(0f, 1f)] public float simSurprise = 0f;
    [Range(0f, 1f)] public float simFrown = 0f;

    [Header("3. Fake Mental Commands")]
    [Range(0f, 1f)] public float simPush = 0f;
    [Range(0f, 1f)] public float simPull = 0f;
    [Range(0f, 1f)] public float simLift = 0f;
    [Range(0f, 1f)] public float simDrop = 0f;
    [Range(0f, 1f)] public float simLeft = 0f;
    [Range(0f, 1f)] public float simRight = 0f;
    [Range(0f, 1f)] public float simUp = 0f;
    [Range(0f, 1f)] public float simDown = 0f;

    [Header("4. Fake Raw Band Power")]
    [Range(0f, 150f)] public float simAlpha = 20f;
    [Range(0f, 150f)] public float simTheta = 15f;
    [Range(0f, 150f)] public float simBeta = 10f;
    [Range(0f, 150f)] public float simDelta = 5f;

    // --- REAL DATA STORAGE ---
    private float _latestRelaxation = 0f;
    private float _latestFocus = 0f;
    private float _latestStress = 0f;
    private float _latestTheta, _latestAlpha, _latestLowBeta, _latestHighBeta;

    void Start()
    {
        DataStreamProcess.Instance.BandPowerDataReceived += OnRawBandPowerReceived;
        DataStreamProcess.Instance.PerfDataReceived += OnRawPerformanceDataReceived;
    }

    void OnDestroy()
    {
        DataStreamProcess.Instance.BandPowerDataReceived -= OnRawBandPowerReceived;
        DataStreamProcess.Instance.PerfDataReceived -= OnRawPerformanceDataReceived;
    }

    public float GetValueFromBypass(BrainDataCategory category, string signalName, int sensorIndex = 0)
    {
        string query = signalName.ToLower();

        // ==========================================
        //  SIMULATION MODE (OFFLINE TESTING)
        // ==========================================
        if (simulationMode)
        {
            switch (category)
            {
                case BrainDataCategory.PerformanceMetrics:
                    if (query == "attention") return simAttention;
                    if (query == "relaxation") return simRelaxation;
                    if (query == "stress") return simStress;
                    break;

                case BrainDataCategory.FacialExpressions:
                    if (query == "blink") return simBlink;
                    if (query == "smile") return simSmile;
                    if (query == "clench") return simClench;
                    if (query == "surprise") return simSurprise;
                    if (query == "frown") return simFrown;
                    break;

                case BrainDataCategory.MentalCommands:
                    if (query == "push") return simPush;
                    if (query == "pull") return simPull;
                    if (query == "lift") return simLift;
                    if (query == "drop") return simDrop;
                    if (query == "left") return simLeft;
                    if (query == "right") return simRight;
                    if (query == "up") return simUp;
                    if (query == "down") return simDown;
                    break;

                case BrainDataCategory.BandPower:
                    if (query == "alpha") return simAlpha;
                    if (query == "theta") return simTheta;
                    if (query == "beta") return simBeta;
                    if (query == "delta") return simDelta;
                    break;
            }
            return 0f;
        }

        // ==========================================
        //  REAL HEADSET MODE
        // ==========================================
        var itf = EmotivUnityItf.Instance;
        if (itf == null || !itf.IsSessionCreated) return 0f;

        switch (category)
        {
            case BrainDataCategory.PerformanceMetrics:
                if (query == "relaxation") return _latestRelaxation;
                if (query == "attention") return _latestFocus;
                if (query == "stress") return _latestStress;
                return (float)itf.GetPMData(query);

            case BrainDataCategory.FacialExpressions:
                if (query == "blink") return (itf.curEyeAct == "blink" ? 1f : 0f);
                if (query == "smile") return (itf.curLAct == "smile" ? (float)itf.curLPow : 0f);
                if (query == "clench") return (itf.curLAct == "clench" ? (float)itf.curLPow : 0f);
                if (query == "surprise") return (itf.curUAct == "surprise" ? (float)itf.curUPow : 0f);
                if (query == "frown") return (itf.curUAct == "frown" ? (float)itf.curUPow : 0f);
                break;

            case BrainDataCategory.MentalCommands:
                string act = itf.LatestMentalCommand.act;
                float pow = (float)itf.LatestMentalCommand.pow;
                if (act == query) return pow;
                break;

            case BrainDataCategory.BandPower:
                if (query == "theta") return _latestTheta;
                if (query == "alpha") return _latestAlpha;
                if (query == "beta") return _latestHighBeta;
                if (query == "delta") return _latestLowBeta;
                break;
        }

        return 0f;
    }

    private void OnRawPerformanceDataReceived(object sender, ArrayList data)
    {
        if (data != null && data.Count >= 14)
        {
            _latestStress = System.Convert.ToSingle(data[6]);
            _latestRelaxation = System.Convert.ToSingle(data[9]);
            _latestFocus = System.Convert.ToSingle(data[13]);
        }
    }

    private void OnRawBandPowerReceived(object sender, ArrayList data)
    {
        if (data != null && data.Count >= 5)
        {
            _latestTheta = System.Convert.ToSingle(data[1]);
            _latestAlpha = System.Convert.ToSingle(data[2]);
            _latestLowBeta = System.Convert.ToSingle(data[3]);
            _latestHighBeta = System.Convert.ToSingle(data[4]);
        }
    }
}