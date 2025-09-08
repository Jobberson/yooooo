using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

// (connects the dialog system to radio UI + audio)
public class DialogController : MonoBehaviour
{
    [Header("References")]
    public DialogBehaviour dialogBehaviour; // assign in inspector (the DialogBehaviour component)
    public AudioSource audioSource;
    public GameObject radioUI; // UI canvas/panel that appears when radio is used
    public Transform playerTransform;
    public PlayerMovement playerMovement; // script you use to disable player input

    [Header("Audio Clips")]
    public AudioClips[] audioClips;

    private void Awake()
    {
        if (dialogBehaviour == null)
            Debug.LogError("DialogBehaviour reference required");

        audioSource ??= GetComponent<AudioSource>();

        for (int i = 0; i < audioClips.Length; i++)
        {
            var clip = audioClips[i]; 
            string key = "Play_" + clip.name; // dialog nodes will use this exact string

            dialogBehaviour.BindExternalFunction(key, () => PlayClipCoroutine(clip, loop:false));
        }

        // Bind simple external functions
        dialogBehaviour.BindExternalFunction("ToggleRadioUI", ToggleRadioUI);
        dialogBehaviour.BindExternalFunction("StopAllRadioAudio", StopAllAudio);

        // Bind coroutine external functions (these return IEnumerator)
        dialogBehaviour.BindExternalFunction("LockPlayer", () => LockPlayerCoroutine());
        dialogBehaviour.BindExternalFunction("UnlockPlayer", () => UnlockPlayerCoroutine());
    }

    // Called when player presses interact on radio
    public void StartRadioDialog(DialogNodeGraph dialogGraph)
    {
        // start the dialog (optionally provide finished callback)
        dialogBehaviour.StartDialog(dialogGraph, onVariablesHandlerInitialized: null, onDialogFinished: OnDialogFinished);
        // ensure UI opened (either internal to dialog nodes or call here)
    }

    private void ToggleRadioUI()
    {
        radioUI?.SetActive(!radioUI.activeSelf);
    }

    private void StopAllAudio()
    {
        audioSource.Stop();
    }

    // Coroutine helpers return IEnumerator so the dialog system will wait for them
    private IEnumerator PlayClipCoroutine(AudioClip clip, bool loop)
    {
        if (clip == null)
            yield break;

        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();

        if (loop)
        {
            // if looping, do not block progression (or block based on design)
            yield break;
        }
        else
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    private IEnumerator LockPlayerCoroutine()
    {
        playerMovement?.enabled = false;
        yield break;
    }

    private IEnumerator UnlockPlayerCoroutine()
    {
        playerMovement?.enabled = true;
        yield break;
    }

    private void OnDialogFinished(DialogVariablesHandler variables)
    {
        // clean up or close UI when dialog ends
        radioUI?.SetActive(false);
    }
}
