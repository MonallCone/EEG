using UnityEngine;
using EmotivUnityPlugin;
using System.Collections.Generic;

/*
 * SCRIPT: BrainAutoConnect (The Ignition Key)
 * PURPOSE: This script automates the complex handshake between Unity and the Emotiv Headset.
 * It handles authentication, finding the hardware, and subscribing to the data streams silently 
 * in the background, so you don't need a clunky login menu in your game.
 * * DESIGNER NOTE: The ONLY thing you need to touch here is the 'Profile Name' in the Inspector.
 */
public class BrainAutoConnect : MonoBehaviour
{
    [Header("--- Profile Settings ---")]
    [Tooltip("Type the EXACT name of the profile you trained in the Emotiv App. Essential for Mental Commands (like Push/Pull) to work.")]
    public string profileName = "123";

    private EmotivUnityItf _eItf;

    // The sequence of steps required to establish a stable connection.
    private enum ConnectState { WaitAuth, Querying, WaitSession, Subscribing, Done }
    private ConnectState currentState = ConnectState.WaitAuth;

    void Start()
    {
        _eItf = EmotivUnityItf.Instance;

        // 1. Boot up the Emotiv Engine using credentials from AppConfig
        _eItf.Init(AppConfig.ClientId, AppConfig.ClientSecret, AppConfig.AppName, AppConfig.AllowSaveLogToFile, AppConfig.IsDataBufferUsing, AppConfig.AppUrl);
        _eItf.Start();

        Debug.Log("⏳ [Brain] Starting Authentication automatically...");
        // Note: Background login is handled automatically by the Emotiv Cortex service on Windows.
    }

    void Update()
    {
        if (_eItf == null) return;

        // A State Machine that moves step-by-step to avoid freezing the game during the connection process
        switch (currentState)
        {
            case ConnectState.WaitAuth:
                // Step 1: Wait for the background app to authorize our game
                if (_eItf.IsAuthorizedOK)
                {
                    Debug.Log("🔍 [Brain] Authorized! Searching for Headset...");
                    _eItf.QueryHeadsets();
                    currentState = ConnectState.Querying;
                }
                break;

            case ConnectState.Querying:
                // Step 2: Grab the first available physical headset found nearby
                var headsets = _eItf.GetDetectedHeadsets();
                if (headsets != null && headsets.Count > 0)
                {
                    string foundId = headsets[0].HeadsetID;
                    Debug.Log($"🔗 [Brain] Connecting to: {foundId}");
                    _eItf.CreateSessionWithHeadset(foundId);
                    currentState = ConnectState.WaitSession;
                }
                break;

            case ConnectState.WaitSession:
                // Step 3: Session established. Now load the player's trained brain profile.
                if (_eItf.IsSessionCreated)
                {
                    if (!string.IsNullOrEmpty(profileName))
                    {
                        Debug.Log($"📂 [Brain] Loading Profile: {profileName}");
                        _eItf.LoadProfile(profileName);
                    }

                    // Step 4: Request the specific data packages we need for our game mechanics
                    // met = Metrics (Emotions), fac = Facial, com = Mental Commands, pow = Raw Frequencies
                    List<string> streams = new List<string> { "met", "fac", "com", "pow" };
                    _eItf.SubscribeData(streams);

                    Debug.Log("<color=green>✅ [Brain] ALL READY! Data is flowing!</color>");
                    currentState = ConnectState.Done;
                }
                break;

            case ConnectState.Done:
                // Step 5: Everything is connected. Disable this script's Update loop to save CPU performance.
                this.enabled = false;
                break;
        }
    }

    // Extremely important safety measure: Safely disconnects the headset when you hit 'Stop' in Unity.
    // Without this, Unity might crash or the headset might lock up for the next session.
    void OnApplicationQuit()
    {
        if (_eItf != null)
        {
            _eItf.Stop();
        }
    }
}