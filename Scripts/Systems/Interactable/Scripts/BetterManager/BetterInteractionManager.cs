using UnityEngine;
using AwesomeAttributes;

/// <summary>
/// This script is the implementation of the IInteractable interface for various interactable objects in the game.
/// </summary>

public class BetterInteractionManager : MonoBehaviour, IInteractable
{
    // Which button the user must press to initiate the Interaction;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private KeyCode alternateInteractionKey = KeyCode.Mouse0;
    [SerializeField, TagSelector, Tooltip("This is the tag this object in specific will have for the interaction to work")] private string interactibleTag;

    private IInteractionBehavior interactionBehavior;
    private IInteractionPrompt prompt;
    private Outline outline;

    private void Awake()
    {
        interactionBehavior = InteractionBehaviorFactory.GetBehavior(interactibleTag);
        prompt = InteractionPromptFactory.GetPrompt(interactibleTag);
        
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(alternateInteractionKey))
        {
            interactionBehavior?.Execute(gameObject);
        }
    }

    public void OnInteractEnter()
    {
        outline.enabled = true;
        InteractionText.instance.SetText(prompt.GetPrompt(interactionKey));
    }

    public void OnInteractExit()
    {
        outline.enabled = false;
        InteractionText.instance.SetText("");
    }

}