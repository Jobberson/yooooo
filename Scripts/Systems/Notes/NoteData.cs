using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "Notes/NoteData")]
public class NoteData : ScriptableObject
{
    public string noteTitle;
    [TextArea(5, 20)] public string noteContent;
    public Sprite noteImage; // opcional, para notas manuscritas ou ilustradas
}
