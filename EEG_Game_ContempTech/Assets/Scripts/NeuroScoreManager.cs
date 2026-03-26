using UnityEngine;
using UnityEngine.UI;

/* * SCRIPT: NeuroScoreManager
 * PURPOSE: This script manages the game's scoring system. 
 * It keeps track of the total points and updates the score text on the screen.
 */

public class NeuroScoreManager : MonoBehaviour
{
    // 1. THE SINGLETON PATTERN
    // This allows other scripts to easily find and use the ScoreManager 
    // by calling "NeuroScoreManager.Instance" without needing a direct reference.
    public static NeuroScoreManager Instance;

    [Header("UI Settings")]
    [Tooltip("Drag your UI Text object here to display the score.")]
    public Text scoreText;

    // The actual score value stored in memory
    private int totalScore = 0;

    void Awake()
    {
        // Initialize the Singleton instance
        Instance = this;
    }

    /* * FUNCTION: AddScore
     * Call this from other scripts whenever the player earns or loses points.
     * Example: NeuroScoreManager.Instance.AddScore(10);
     */
    public void AddScore(int points)
    {
        totalScore += points;

        /*  EXTRA IDEAS:
         * - You can add a sound effect here: audioSource.PlayOneShot(scoreSound);
         * - You can check for a 'Win Condition' here: if (totalScore >= 100) WinGame();
         */

        UpdateUI();
    }

    /* * FUNCTION: UpdateUI
     * Updates the visual text on the player's screen.
     */
    void UpdateUI()
    {
        if (scoreText != null)
        {
            // STUDENT: You can change the word "Score" to something else like "Points" or "Focus level"
            scoreText.text = "Score: " + totalScore;
        }
    }
}