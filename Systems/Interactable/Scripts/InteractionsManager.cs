using UnityEngine;
using AwesomeAttributes;

/// <summary>
/// This script is the implementation of the IInteractable interface for various interactable objects in the game.
/// </summary>

public class InteractionsManager : MonoBehaviour, IInteractable
{
    // Which button the user must press to initiate the Interaction;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private KeyCode alternateInteractionKey = KeyCode.Mouse0;
    [SerializeField, TagSelector, Tooltip("This is the tag this object in specific will have for the interaction to work")] private string interactibleTag;
    public void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(alternateInteractionKey))
        {
            switch (interactibleTag)
            {
                case "Radio":
                    // put a LookAt here to face the radio while the dialogue is playing
                    // will need to make sure that the player can't move while the dialogue is playing
                    // and that can click the answer buttons

                    switch (StoryManager.Instance.CurrentState)
                    {
                        case StoryState.afterGenerator:
                            Debug.Log("Radio interaction at afterGenerator state");
                            FindAnyObjectByType<BeforeTutorialDialogue>().StopPreTutorialDialogue();
                            // it should start the introduction dialogue here
                            // that sets up for the heater task
                            StoryManager.Instance.AdvanceToState(StoryState.waitingForHeater);
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

                    break;
                case "Generator":
                    Debug.Log("Generator interaction");
                    if (StoryManager.Instance.IsAtState(StoryState.beforeGenerator))
                        StoryManager.Instance.AdvanceToState(StoryState.afterGenerator);
                    break;
                case "Heater":
                    Debug.Log("Heater interaction");
                    break;
                case "SupplyCabinet":
                    Debug.Log("Supply Cabinet interaction");
                    break;
                case "FuseBox":
                    Debug.Log("Fuse Box interaction");
                    break;
                case "Flashlight":
                    Debug.Log("Flashlight interaction");
                    break;
                case "Door":
                    Debug.Log("Door interaction");
                    break;
                case "Note":
                    Debug.Log("Note interaction");
                    break;
                case "Bed":
                    Debug.Log("Bed interaction");
                    break;
                case "Locker":
                    Debug.Log("Locker interaction");
                    break;
                case "Toolbox":
                    Debug.Log("Toolbox interaction");
                    break;
                case "Map":
                    Debug.Log("Map interaction");
                    break;
                case "Switch":
                    Debug.Log("Switch interaction");
                    break;
                default:
                    Debug.Log("No interaction defined for this tag");
                    break;
            }
        }
    }

    // When our interaction begins, we set the UI text to prompt the user to
    // press a button to interact with the gameobject;
    public void OnInteractEnter()
    {
        switch (interactibleTag)
        {
            // TO USE
            case "Generator":
            case "Radio":
            case "Heater":
            case "Switch":
                InteractionText.instance.SetText("Press " + interactionKey + " to use");
                break;

            // TO OPEN
            case "SupplyCabinet":
            case "FuseBox":
            case "Door":
            case "Locker":
                InteractionText.instance.SetText("Press " + interactionKey + " to open");
                break;

            // TO VIEW
            case "Map":
                InteractionText.instance.SetText("Press " + interactionKey + " to view");
                break;

            // TO PICKUP
            case "Toolbox":
                InteractionText.instance.SetText("Press " + interactionKey + " to pick up");
                break;

            // TO EQUIP
            case "Flashlight":
                InteractionText.instance.SetText("Press " + interactionKey + " to equip");
                break;

            // TO READ
            case "Note":
                InteractionText.instance.SetText("Press " + interactionKey + " to read");
                break;

            // TO SLEEP
            case "Bed":
                InteractionText.instance.SetText("Press " + interactionKey + " to sleep");
                break;
        }
        InteractionText.instance.SetText("Press " + interactionKey + " to interact");
    }


    // We can debug a statement to let us know when the interaction ends;
    public void OnInteractExit() { }
}

