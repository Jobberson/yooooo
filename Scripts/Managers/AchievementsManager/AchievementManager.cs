using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private HashSet<string> unlockedAchievements = new HashSet<string>();
    private Dictionary<string, AchievementProgress> progressAchievements = new();

    public delegate void AchievementUnlocked(Achievement achievement);
    public event AchievementUnlocked OnAchievementUnlocked;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UnlockAchievement(Achievement achievement)
    {
        if (unlockedAchievements.Contains(achievement.achievementID)) return;

        unlockedAchievements.Add(achievement.achievementID);
        OnAchievementUnlocked?.Invoke(achievement);

        // Save progress here
        PlayerPrefs.SetInt(achievement.achievementID, 1);
    }

    public bool IsUnlocked(string achievementID)
    {
        return unlockedAchievements.Contains(achievementID) || PlayerPrefs.GetInt(achievementID, 0) == 1;
    }

    public void RegisterProgressAchievement(Achievement achievement)
    {
        if (achievement.type == AchievementType.Progress && !IsUnlocked(achievement.achievementID))
        {
            progressAchievements[achievement.achievementID] = new AchievementProgress(achievement);
        }
    }

    public void IncrementProgress(string achievementID)
    {
        if (progressAchievements.TryGetValue(achievementID, out var progress))
        {
            progress.Increment();
        }
    }
}
