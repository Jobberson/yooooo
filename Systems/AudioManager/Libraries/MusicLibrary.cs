using UnityEngine;
using System.Collections.Generic;

public class MusicLibrary : MonoBehaviour 
{
    public MusicClipData[] musicClips;

    private Dictionary<string, AudioClip> musicDictionary = new();

    void Awake() 
	{
        foreach (var musicData in musicClips) 
		{
            if (!musicDictionary.ContainsKey(musicData.musicName)) 
			{
                musicDictionary.Add(musicData.musicName, musicData.clip);
            }
        }
    }

    public AudioClip GetClipFromName(string name) 
	{
        if (musicDictionary.TryGetValue(name, out var clip)) 
		{
            return clip;
        }
        return null;
    }
}
