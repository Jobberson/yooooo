using System.Collections.Generic;
using UnityEngine;

public class AchievementProgress
{
    public Achievement achievement;
    public int currentCount;

    public AchievementProgress(Achievement achievement)
    {
        this.achievement = achievement;
        currentCount = PlayerPrefs.GetInt(achievement.achievementID + "_Progress", 0);
    }

    public void Increment()
    {
        currentCount++;
        PlayerPrefs.SetInt(achievement.achievementID + "_Progress", currentCount);

        if (currentCount >= achievement.targetCount)
        {
            AchievementManager.Instance.UnlockAchievement(achievement);
        }
    }
}
