using System.Collections;
using UnityEngine;
using Snog.PlayerMovement;

// (connects the dialog system to radio UI + audio)
namespace cherrydev
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DialogBehaviour dialogBehaviour; // assign in inspector (the DialogBehaviour component)
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject radioUI; // UI canvas/panel that appears when radio is used
        [SerializeField] private Transform playerTransform;
        [SerializeField] private PlayerCharacter playerCharacter; // script you use to disable player input
        [SerializeField] private PlayerCamera playerCamera;

        [Header("Dialog Lines Audio Clips")]
        private Dictionary<string, AudioClip> clipDictionary = new();
 
        private void Awake()
        {
            if (dialogBehaviour == null || playerCharacter == null || playerCamera == null || radioUI == null)
                Debug.LogError("DialogController is missing required references.");

            if (!TryGetComponent(out audioSource))
                Debug.LogError("Missing AudioSource component on DialogController.");

            InitDialogueLines();

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
            playerCharacter.canMove = false;
            playerCharacter.canCrouch = false;
            playerCamera.canLook = false;

            yield break;
        }

        private IEnumerator UnlockPlayerCoroutine()
        {
            playerCharacter.canMove = true;
            playerCharacter.canCrouch = true;
            playerCamera.canLook = true;
            yield break;
        }

        private void OnDialogFinished(DialogVariablesHandler variables)
        {
            // clean up or close UI when dialog ends
            radioUI?.SetActive(false);
        }

        private void InitDialogueLines()
        {
            foreach (var clip in audioClips)
            {
                if (clip == null) continue;

                string key = "Play_" + clip.name;
                clipDictionary[key] = clip;

                dialogBehaviour.BindExternalFunction(key, () => PlayClipCoroutine(clip, loop: false));
            }
        }
    }
}