using UnityEngine;

/*
 * SCRIPT: BrainEnums (The Brain-Computer Interface Dictionary)
 * PURPOSE: This file acts as the master dictionary for our BCI system. 
 * It doesn't execute any logic; rather, it populates the clean, designer-friendly 
 * dropdown menus in the Unity Inspector. 
 * * DESIGNER NOTE: If Emotiv releases a new mental command or facial expression in the future,
 * this is the first place you add it so the rest of your tools can see it.
 */

// --- MAIN CATEGORIES ---
// This dictates the primary data stream we want to listen to from the headset.
public enum BrainDataCategory
{
    MentalCommands,    // Conscious thoughts trained by the player (e.g., imagining pushing a block).
    FacialExpressions, // Physical muscle movements detected by the sensors. Great for instant reactions.
    PerformanceMetrics,// Subconscious emotional states. Perfect for dynamic difficulty or changing music/lighting.
    BandPower          // Raw brainwave frequencies. Usually for advanced or experimental mechanics.
}

// ----------------------------------------------------
// --- SUB-CATEGORIES (Context-Sensitive Menus) ---
// These menus automatically swap out in the Inspector based on the Main Category chosen above.
// ----------------------------------------------------

// 1. PERFORMANCE METRICS (Emotional / Subconscious)
public enum BrainMetrics
{
    Attention,  // Use this for focus-based mechanics (e.g., aiming a sniper, charging a spell).
    Relaxation, // Use this for calming interactions (e.g., healing, restoring stamina).
    Stress      // Use this to trigger dynamic horror elements or increase enemy aggression.
}

// 2. FACIAL EXPRESSIONS (Physical / Instant)
public enum BrainFacial
{
    Blink,
    Smile,
    Clench,   // Clenching teeth. Good for triggering physical attacks or bracing for impact.
    Surprise,
    Frown
}

// 3. MENTAL COMMANDS (Conscious / Trained)
// NOTE: The player MUST train these specific thoughts in the Emotiv App before they work in-game.
public enum BrainMental
{
    Push, Pull, Lift, Drop, Left, Right, Up, Down
}

// 4. RAW BAND POWER (Frequencies)
public enum BrainWaveType
{
    Theta, // Deep relaxation/meditation.
    Alpha, // Relaxed but awake (Closing eyes spikes this significantly).
    Beta,  // Active thinking / problem solving.
    Delta  // Deep sleep (rarely used in active gameplay).
}

// --- HARDWARE MAPPING ---
// The specific physical electrodes on the EPOC X headset.
/* * DESIGNER NOTE: You usually won't need to change this unless you are building 
 * a highly specific mechanic that relies on a particular brain hemisphere 
 * (e.g., isolating the left frontal lobe for specific frequency tests).
 */
public enum EpocX_Sensor
{
    AF3 = 0, F7 = 1, F3 = 2, FC5 = 3, T7 = 4, P7 = 5, O1 = 6,
    O2 = 7, P8 = 8, T8 = 9, FC6 = 10, F4 = 11, F8 = 12, AF4 = 13
}