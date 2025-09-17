public enum AchievementType 
{ 
    Instant, 
    Progress 
}

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievements/Achievement")]
public class Achievement : ScriptableObject
{
    public string achievementID;
    public string title;
    public string description;
    public Sprite icon;
    public bool isSecret;
    public AchievementType type;

    [Header("Progress Settings")]
    public int targetCount; // e.g., 10 notes
}
