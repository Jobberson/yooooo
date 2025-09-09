using UnityEngine;
using System.Collections.Generic;

public class AmbientLibrary : MonoBehaviour 
{
    public AmbientClipData[] ambientClips;

    private Dictionary<string, AudioClip> ambientDictionary = new();

    void Awake() 
	{
        foreach (var ambientData in ambientClips) 
		{
            if (!ambientDictionary.ContainsKey(ambientData.ambientName)) 
			{
                ambientDictionary.Add(ambientData.ambientName, ambientData.clip);
            }
        }
    }

    public AudioClip GetClipFromName(string name) 
	{
        if (ambientDictionary.TryGetValue(name, out var clip)) 
		{
            return clip;
        }
        return null;
    }
}
