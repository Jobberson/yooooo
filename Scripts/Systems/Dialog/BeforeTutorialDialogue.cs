using UnityEngine;
using AwesomeAttributes;
using System.Collections.Generic;
using System.Collections;
using TMPro;

[System.Serializable]
public class DialogueLines
{
    public string dialogLine;
    public AudioClip dialogAudioLine;
}

public class BeforeTutorialDialogue : MonoBehaviour
{
    /// <summary>
    /// This script is here to play the dialogue when the player enters the station after turning the generator on
    /// </summary>

    [SerializeField] private List<DialogLines> dialogueLines = new();
    [SerializeField, MinMaxSlider(3f, 15f)] private Vector2 minMaxBetweenLines = new(6f, 13f);
    [SerializeField] private AudioSource radioAudioSource;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private RadioFlashingLight radioFlashingLight;

    private Coroutine dialogueRoutine;
    private float secBetweenLines;
    private bool hasPlayed = false;

    public void StartPreTutorialDialogue()
    {
        // Only start if not already running
        if (hasPlayed || !StoryManager.Instance.IsAtState(StoryState.afterGenerator)) return;
        dialogueRoutine ??= StartCoroutine(PlayPreTutorialDialogue());
        radioFlashingLight.radioLight.SetActive(true)
    }

    public void StopPreTutorialDialogue()
    {
        // if (dialogueRoutine != null)
        // {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
            radioFlashingLight.radioLight.SetActive(false)
            Debug.Log("Pre-tutorial dialogue stopped.");
            gameobject.SetActive(false);
        // }
    }

    private IEnumerator PlayPreTutorialDialogue()
    {
        hasPlayed = true;
        Debug.Log("Starting pre-tutorial dialogue...");

        for (int i = 0; i < dialogueLines.Count; i++)
        {
            // Play line
            radioAudioSource.clip = dialogueLines[i];
            subtitleText.text = dialogueLines[i].dialogLine;
            radioAudioSource.Play();

            // Waits audio length + half a second
            yield return new WaitForSeconds(radioAudioSource.clip.length + 0.5f);

            // clears subtitle
            subtitleText.text = "";

            // randomizes the amount of seconds to wait until next line
            secBetweenLines = Random.Range(minMaxBetweenLines.x, minMaxBetweenLines.y);
            yield return new WaitForSeconds(secBetweenLines);
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
