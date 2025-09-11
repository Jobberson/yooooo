using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/State Mapping")]
public class DialogStateMapping : ScriptableObject
{
    public StoryState storyState;
    public DialogNodeGraph dialogGraph;
}
