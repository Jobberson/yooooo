using UnityEngine;
using System.Collections.Generic;

public class SoundLibrary : MonoBehaviour 
{
    public SoundClipData[] soundData;

	private Dictionary<string, AudioClip[]> soundDict = new();

	void Awake() 
	{
		foreach (var data in soundData) 
		{
            soundDict[data.soundName] = data.clips;
        }
	}

	public AudioClip GetClipFromName(string name) 
	{
		if (soundDict.TryGetValue(name, out var clips)) 
		{
            return clips[Random.Range(0, clips.Length)];
        }

		return null;
	}

	public string[] GetAllClipNames()
	{
		return soundDict.Keys.ToArray(); // Assuming you're using a Dictionary<string, AudioClip>
	}

}