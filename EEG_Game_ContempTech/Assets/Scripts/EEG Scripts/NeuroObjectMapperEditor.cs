using UnityEditor;
using UnityEngine;

/*
 * SCRIPT: NeuroObjectMapperEditor (Custom Tooling & UI)
 * PURPOSE: This is a Custom Editor script. It does NOT run during gameplay.
 * Its sole purpose is to customize how the NeuroObjectMapper looks inside the 
 * Unity Inspector, making it clean, intuitive, and foolproof for the design team.
 * * DESIGNER NOTE: You don't need to edit this. It exists purely to hide irrelevant 
 * variables so your workspace remains uncluttered while designing mechanics.
 */
[CustomEditor(typeof(NeuroObjectMapper))]
public class NeuroObjectMapperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Update the serialized object to ensure we are looking at the latest data
        serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        // Iterate through every variable exposed in the NeuroObjectMapper script
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            // Lock the script reference field at the top so it cannot be accidentally changed
            if (iterator.name == "m_Script")
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(iterator);
                GUI.enabled = true;
                continue;
            }

            // Check which Main Category the designer has currently selected
            BrainDataCategory currentCategory = (BrainDataCategory)serializedObject.FindProperty("dataCategory").enumValueIndex;

            // --- CONTEXT-SENSITIVE MENUS ---
            // Hide the dropdown menus that DO NOT match the currently selected Main Category.
            // This prevents logical errors (e.g., trying to use a "Smile" signal while in the "Mental Commands" category).
            if (iterator.name == "metricSignal" && currentCategory != BrainDataCategory.PerformanceMetrics) continue;
            if (iterator.name == "facialSignal" && currentCategory != BrainDataCategory.FacialExpressions) continue;
            if (iterator.name == "mentalSignal" && currentCategory != BrainDataCategory.MentalCommands) continue;
            if (iterator.name == "waveType" && currentCategory != BrainDataCategory.BandPower) continue;
            if (iterator.name == "bandPowerTarget" && currentCategory != BrainDataCategory.BandPower) continue;

            // --- PROGRESSIVE DISCLOSURE (Dynamic UI) ---
            // Keep the Inspector clean by hiding advanced settings until the designer explicitly enables them.

            // Hide the threshold value slider if the Threshold feature is disabled
            bool isThresholdUsed = serializedObject.FindProperty("useThreshold").boolValue;
            if (iterator.name == "threshold" && !isThresholdUsed) continue;

            // Hide the object prefab slot and cooldown timer if the Spawner feature is disabled
            bool isSpawningUsed = serializedObject.FindProperty("enableSpawning").boolValue;
            if ((iterator.name == "prefabToSpawn" || iterator.name == "spawnCooldown") && !isSpawningUsed) continue;

            // Draw the variable normally in the Inspector if it survived the filters above
            EditorGUILayout.PropertyField(iterator, true);
        }

        // Apply any changes the designer made back to the object
        serializedObject.ApplyModifiedProperties();
    }
}