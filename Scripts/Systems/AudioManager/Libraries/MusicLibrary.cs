using UnityEngine;
using System.Collections.Generic;

public class MusicLibrary : MonoBehaviour 
{
    public MusicTrack[] musicClips;

    private Dictionary<string, AudioClip> musicDictionary = new();

    void Awake() 
	{
        foreach (var musicData in musicClips) 
		{
            if (!musicDictionary.ContainsKey(musicData.trackName)) 
			{
                musicDictionary.Add(musicData.trackName, musicData.clip);
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