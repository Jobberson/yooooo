using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public StoryManager StoryManager { get; private set; }
    public CheckpointManager CheckpointManager { get; private set; }

    void Awake()
    {
        Singleton();
    }

    private void Start()
    {
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
        RouteStoryEvents(newState);
    }

    private void RouteStoryEvents(StoryState state) 
    {
        // AudioManager.Instance?.PlaySnapshotForState(state);
        // DialogueManager.Instance?.TriggerDialogueForState(state);
        // EnvironmentManager.Instance?.UnlockAreaForState(state);
    }
}
