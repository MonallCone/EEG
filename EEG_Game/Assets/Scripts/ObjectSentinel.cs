using UnityEngine;

/* * SCRIPT: ObjectSentinel
 * PURPOSE: This script acts as a sensor/trigger zone. 
 * It detects when objects enter its area and decides whether to reward or penalize the player.
 */

public class ObjectSentinel : MonoBehaviour
{
    [Header("Detection Logic")]
    [Tooltip("Check this ONLY for the sensor inside the Catch Vessel. Leave it unchecked for the Floor.")]
    public bool isGoal = false;

    /* * FUNCTION: OnTriggerEnter
     * This built-in Unity function runs automatically when something enters this object's 'Trigger' zone.
     */
    void OnTriggerEnter(Collider other)
    {
        // 1. FILTER: Check if the object entering is actually a falling box.
        // We do this by checking if the object has the "Player" tag assigned in the Inspector.
        if (other.CompareTag("Player"))
        {
            // 2. DECISION: Is this a 'Goal' (Catch) or a 'Miss' (Floor)?
            if (isGoal)
            {
                // SUCCESS: Add positive points
                // STUDENT: Change '10' to any value you want for a successful catch!
                NeuroScoreManager.Instance.AddScore(10);
                Debug.Log("CATCH! +10 Points");
            }
            else
            {
                // FAILURE: Deduct points for missing
                // STUDENT: Change '-5' to increase the penalty for missing.
                NeuroScoreManager.Instance.AddScore(-5);
                Debug.Log("MISS! -5 Points");
            }

            // 3. CLEANUP: Remove the object from the game after it's been counted.
            // This is CRITICAL to prevent 'lag' as hundreds of boxes spawn over time.
            Destroy(other.gameObject);
        }
    }
}