using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteReaderUI : MonoBehaviour
{
    [SerializeField] private GameObject notePanel;
    [SerializeField] private TextMeshProGUI titleText;
    [SerializeField] private TextMeshProGUI contentText;
    [SerializeField] private Image noteImage;
    [SerializeField] private AudioSource audioSource;

    public void ShowNote(NoteData note)
    {
        titleText.text = note.noteTitle;
        contentText.text = note.noteContent;
        noteImage.sprite = note.noteImage;
        noteImage.gameObject.SetActive(note.noteImage != null);
        notePanel.SetActive(true);
    }

    public void HideNote()
    {
        notePanel.SetActive(false);
        audioSource.Stop();
    }
}
