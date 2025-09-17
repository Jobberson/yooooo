using UnityEngine;

public enum StoryState
{
    beforeGenerator,
    afterGenerator,
    waitingForHeater,
    doneWithHeater,
    doneWithSupplyCabinet,
    doneWithFuseBox,
    Act1Intro
}

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }
    public StoryState CurrentState { get; private set; } = StoryState.beforeGenerator;
    public UnityEvent<StoryState> OnStoryStateChanged;

    void Awake()
    {
        Singleton();
        if (OnStoryStateChanged == null)
            OnStoryStateChanged = new UnityEvent<StoryState>();
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

#region State Management
    public void AdvanceToState(StoryState newState)
    {
        if (CurrentState != newState)
        {
            Debug.Log($"Story advanced to: {newState}");
            CurrentState = newState;
            OnStoryStateChanged.Invoke(newState);
            SaveManager.Instance.SaveStoryState(newState);
        }
    }

    public bool IsAtState(StoryState state)
    {
        return CurrentState == state;
    }

#if UNITY_EDITOR
    [ContextMenu("Advance to Next State")]
    public void DebugAdvanceState()
    {
        int next = ((int)CurrentState + 1) % System.Enum.GetValues(typeof(StoryState)).Length;
        AdvanceToState((StoryState)next);
    }
#endif
#endregion

#region Save/Load 
    public void SaveCurrentState()
    {
        SaveManager.Instance.SaveStoryState(CurrentState);
    }

    public void LoadSavedState()
    {
        StoryState loadedState = SaveManager.Instance.LoadStoryState();
        AdvanceToState(loadedState);
    }
#endregion
}
