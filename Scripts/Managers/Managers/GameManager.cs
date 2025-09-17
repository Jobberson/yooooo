using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public StoryManager StoryManager { get; private set; }
    public CheckpointManager CheckpointManager { get; private set; }

    void Awake()
    {
        Singleton();
        InitializeManagers();
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeManagers()
    {
        // Find or create StoryManager
        if (StoryManager == null)
        {
            StoryManager = FindObjectOfType<StoryManager>();
            if (StoryManager == null)
            {
                GameObject storyManagerGO = new GameObject("StoryManager");
                StoryManager = storyManagerGO.AddComponent<StoryManager>();
            }
        }

        StoryManager.OnStoryStateChanged.AddListener(HandleStoryStateChange);

        // Find or create CheckpointManager
        if (CheckpointManager == null)
        {
            CheckpointManager = FindObjectOfType<CheckpointManager>();
            if (CheckpointManager == null)
            {
                GameObject checkpointManagerGO = new GameObject("CheckpointManager");
                CheckpointManager = checkpointManagerGO.AddComponent<CheckpointManager>();
            }
        }
    }

    private void HandleStoryStateChange(StoryState newState)
    {
        Debug.Log($"GameManager detected story state change: {newState}");

        CheckpointManager.Instance.SaveCheckpoint(newState);

        // Example: trigger audio or visual effects
        // AudioManager.Instance.PlaySnapshotForState(newState);
        // DialogueManager.Instance.TriggerDialogueForState(newState);
        // EnvironmentManager.Instance.UnlockAreaForState(newState);
        // UIManager.Instance.ShowObjectiveForState(newState);
        // EnemyAIManager.Instance.ActivateBehaviorForState(newState);
    }
}
