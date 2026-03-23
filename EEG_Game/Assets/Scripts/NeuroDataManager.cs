using UnityEngine;
using System.Collections.Generic;
using System.IO;

/* * SCRIPT: NeuroDataManager
 * PURPOSE: This script acts as the "Brain" of your project. It reads a CSV file containing
 * neural data (EEG, Mental Commands, etc.) and streams it row by row to the rest of the game.
 */

public class NeuroDataManager : MonoBehaviour
{
    [Header("File Settings")]
    [Tooltip("The name of your data file inside the 'StreamingAssets' folder.")]
    public string csvFileName = "GeneratedData.csv";

    // Internal data storage
    private List<string[]> dataRows = new List<string[]>();
    private Dictionary<string, int> columnMapping = new Dictionary<string, int>();
    private int currentRow = 0;
    private float timer = 0;

    void Awake()
    {
        // 1. Locate the file in the project's StreamingAssets folder
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(path))
        {
            Debug.LogError("FILE NOT FOUND! Make sure the filename is correct and it's in the StreamingAssets folder.");
            return;
        }

        // 2. Read all lines from the CSV
        string[] lines = File.ReadAllLines(path);

        if (lines.Length > 0)
        {
            // 3. Extract Column Names (Headers) and map their positions
            // Example: "EEG_Delta" is at index 1, "EEG_Theta" is at index 2, etc.
            string[] headers = lines[0].Trim().Split(',');
            for (int i = 0; i < headers.Length; i++)
            {
                columnMapping[headers[i].Trim()] = i;
            }

            // 4. Load the actual numeric data into memory
            for (int i = 1; i < lines.Length; i++)
            {
                if (!string.IsNullOrEmpty(lines[i]))
                    dataRows.Add(lines[i].Split(','));
            }

            Debug.Log("<color=green>Data Loaded Successfully!</color> Total Rows: " + dataRows.Count);
        }
    }

    void Update()
    {
        if (dataRows.Count == 0) return;

        // 5. DATA STREAMING FREQUENCY
        // This timer controls how fast we move to the next row of data.
        timer += Time.deltaTime;

        /* STUDENT: CHANGE THIS VALUE TO SPEED UP OR SLOW DOWN DATA PLAYBACK
         * 0.004f = 250 rows per second (Real-time speed for 250Hz data)
         * 0.01f  = 100 rows per second (Slower, easier to see changes)
         * 0.04f  = 25 rows per second (Very slow playback)
         */
        if (timer >= 0.004f)
        {
            timer = 0;
            // Move to the next row. If we reach the end, loop back to the start (%).
            currentRow = (currentRow + 1) % dataRows.Count;
        }
    }

    /* * FUNCTION: GetValue
     * Use this in other scripts to get the current value of a specific sensor or command.
     * Example: GetValue("Fac_Blink") will return a float value (0 or 1).
     */
    public float GetValue(string columnName)
    {
        // Check if the column exists in our data
        if (columnMapping.ContainsKey(columnName))
        {
            string val = dataRows[currentRow][columnMapping[columnName]];
            float result;

            // Convert the string value from CSV to a float number
            return float.TryParse(val, out result) ? result : 0;
        }

        // If column name is wrong or doesn't exist, return 0
        return 0;
    }
}