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

    void Awake()
    {
        Singleton();
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

    public void AdvanceToState(StoryState newState)
    {
        Debug.Log($"Story advanced to: {newState}");
        CurrentState = newState;
    }

    public bool IsAtState(StoryState state)
    {
        return CurrentState == state;
    }
}
