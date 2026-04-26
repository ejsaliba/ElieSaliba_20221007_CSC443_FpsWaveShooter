using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Scores")]
    public int currentScore = 0;
    public int totalScore = 0;

    [Header("Kill Streak")]
    public float killWindow = 1f;
    public float streakDuration = 3f;

    private List<float> killTimes = new List<float>();
    private bool streakActive = false;
    private float streakTimer = 0f;

    public bool IsStreakActive => streakActive;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Handle streak countdown
        if (streakActive)
        {
            streakTimer -= Time.deltaTime;

            if (streakTimer <= 0f)
            {
                streakActive = false;
                killTimes.Clear(); // reset chain when streak ends
            }
        }
    }

    public void RegisterKill(Robot.EnemyType type, bool exploded)
    {
        int baseScore = 0;

        switch (type)
        {
            case Robot.EnemyType.Melee:
                baseScore = 10;
                break;

            case Robot.EnemyType.Ranged:
                baseScore = 30;
                break;

            case Robot.EnemyType.Exploder:
                if (!exploded)
                    baseScore = 50;
                break;
        }

        float currentTime = Time.time;

        // --- Track kill times ---
        killTimes.Add(currentTime);
        killTimes.RemoveAll(t => currentTime - t > killWindow);

        // --- Activate streak ---
        if (killTimes.Count >= 3)
        {
            streakActive = true;
            streakTimer = streakDuration;
        }

        // --- Reset timer if already active ---
        if (streakActive)
        {
            streakTimer = streakDuration;
            baseScore *= 2;
        }

        // --- Apply score ---
        currentScore += baseScore;
        totalScore += baseScore;
    }
}