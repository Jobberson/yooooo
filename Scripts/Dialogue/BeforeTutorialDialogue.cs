using UnityEngine;
using AwesomeAttributes;
using System.Collections.Generic;
using System.Collections;

public class BeforeTutorialDialogue : MonoBehaviour
{
    /// <summary>
    /// This script is here to play the dialogue when the player enters the station after turning the generator on
    /// </summary>

    [SerializeField] private List<string> dialogueLines = new();
    [SerializeField, MinMaxSlider(3f, 15f)] private Vector2 minMaxBetweenLines = new(6f, 10f);
    private Coroutine dialogueRoutine;
    private float secBetweenLines;
    private bool hasPlayed = false;

    private void Start()
    {
        secBetweenLines = Random.Range(minMaxBetweenLines.x, minMaxBetweenLines.y);
    }

    public void StartPreTutorialDialogue()
    {
        // Only start if not already running
        if (hasPlayed || !StoryManager.Instance.IsAtState(StoryState.afterGenerator)) return;
        dialogueRoutine ??= StartCoroutine(PlayPreTutorialDialogue());
    }

    public void StopPreTutorialDialogue()
    {
        // if (dialogueRoutine != null)
        // {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
            Debug.Log("Pre-tutorial dialogue stopped.");
        // }
    }

    private IEnumerator PlayPreTutorialDialogue()
    {
        hasPlayed = true;
        Debug.Log("Starting pre-tutorial dialogue...");

        for (int i = 0; i < dialogueLines.Count; i++)
        {
            // Play line
            // audioSource.clip = dialogueLines[i];
            // audioSource.Play();

            // Wait until line finishes + delay
            // audioSource.clip.length + 
            yield return new WaitForSeconds(secBetweenLines);
            secBetweenLines = Random.Range(minMaxBetweenLines.x, minMaxBetweenLines.y);
        }

        dialogueRoutine = null; // Reset
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered dialogue trigger area.");
        if (other.CompareTag("Player"))
        {
            StartPreTutorialDialogue();
        }
    }
}
