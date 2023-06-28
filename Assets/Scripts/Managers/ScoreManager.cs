using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public static event System.Action OnPelletCountUpdated;
    public static event System.Action OnPelletCountCompleted; // Event that gets called when pellet score has been reached
    public static bool StopScore;

    // Vars
    public int pelletCount;
    public int score;
    public float timeSpent;

    // Call this method to trigger the event and update the UI score
    public static void TriggerPelletCountUpdated()
    {
        ScoreManager.Instance.pelletCount += 1;
        OnPelletCountUpdated?.Invoke();
    }

    public static void PelletCountCompleted()
    {
        int totalNoOfPellets = PelletManager.Instance.pellets.Count;
        int noOfPelletsCollected = ScoreManager.Instance.pelletCount;

        if (noOfPelletsCollected == totalNoOfPellets)
        {
            Debug.Log("You won!");
            StopScore = true;

            OnPelletCountCompleted?.Invoke();
        }
    }

    private void Awake()
    {
        Instance = this;

        // initialize everything to 0
        StopScore = false;
        pelletCount = 0;
        score = 0;
        timeSpent = 0f;
    }

    private void Update()
    {
        if (StopScore)
            return;

        timeSpent += Time.unscaledDeltaTime;
    }
}
