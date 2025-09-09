using UnityEngine;

[CreateAssetMenu(fileName = "SoundClipData", menuName = "Audio/SoundClipData")]
public class SoundClipData : ScriptableObject 
{
    public string soundName;
    public AudioClip[] clips;
}
