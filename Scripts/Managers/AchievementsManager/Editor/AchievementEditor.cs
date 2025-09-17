using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Achievement))]
public class AchievementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Achievement achievement = (Achievement)target;

        EditorGUILayout.LabelField("Basic Info", EditorStyles.boldLabel);
        achievement.achievementID = EditorGUILayout.TextField("ID", achievement.achievementID);
        achievement.title = EditorGUILayout.TextField("Title", achievement.title);
        achievement.description = EditorGUILayout.TextField("Description", achievement.description);
        achievement.icon = (Sprite)EditorGUILayout.ObjectField("Icon", achievement.icon, typeof(Sprite), false);
        achievement.isSecret = EditorGUILayout.Toggle("Is Secret?", achievement.isSecret);
        achievement.type = (AchievementType)EditorGUILayout.EnumPopup("Type", achievement.type);

        if (achievement.type == AchievementType.Progress)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Progress Settings", EditorStyles.boldLabel);
            achievement.targetCount = EditorGUILayout.IntField("Target Count", achievement.targetCount);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(achievement);
        }
    }
}
