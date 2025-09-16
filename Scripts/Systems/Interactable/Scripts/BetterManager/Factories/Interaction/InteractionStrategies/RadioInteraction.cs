
public class RadioInteraction : IInteractionBehavior
{
    public void Execute(GameObject interactable)
    {
        Debug.Log("Interacting with Radio");
        
        var dialogController = Object.FindAnyObjectByType<DialogController>();
        var storyManager = StoryManager.Instance;

        if (dialogController == null || storyManager == null)
        {
            Debug.LogWarning("Missing DialogController or StoryManager.");
            return;
        }

        var radioComponent = interactable.GetComponent<BetterInteractionManager>();
        if (radioComponent == null)
        {
            Debug.LogWarning("Missing BetterInteractionManager on Radio.");
            return;
        }

        var graph = GetDialogForState(radioComponent.radioDialogMappings, storyManager.CurrentState);
        if (graph != null)
        {
            dialogController.StartRadioDialog(graph);
        }
        else
        {
            Debug.LogWarning("No dialog graph found for current story state.");
        }

        // Handle state transitions
        switch (storyManager.CurrentState)
        {
            case StoryState.afterGenerator:
                Debug.Log("Radio interaction at afterGenerator state");
                Object.FindAnyObjectByType<BeforeTutorialDialogue>()?.StopPreTutorialDialogue();
                storyManager.AdvanceToState(StoryState.waitingForHeater);
                break;

            case StoryState.waitingForHeater:
                Debug.Log("Radio interaction at waitingForHeater state");
                break;

            case StoryState.doneWithHeater:
                Debug.Log("Radio interaction at doneWithHeater state");
                break;

            case StoryState.doneWithSupplyCabinet:
                Debug.Log("Radio interaction at doneWithSupplyCabinet state");
                break;

            case StoryState.doneWithFuseBox:
                Debug.Log("Radio interaction at doneWithFuseBox state");
                break;

            case StoryState.Act1Intro:
                Debug.Log("Radio interaction at Act1Intro state");
                break;

            default:
                Debug.Log("No dialogue for this state");
                break;
        }
    }

    private DialogNodeGraph GetDialogForState(List<DialogStateMapping> mappings, StoryState state)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.storyState == state)
                return mapping.dialogGraph;
        }
        return null;
    }
}
