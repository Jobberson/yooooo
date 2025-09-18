using UnityEngine;
using System.Collections;
using AwesomeAttributes;
using UnityEngine.Audio;

[RequireComponent(typeof(SoundLibrary))]
[RequireComponent(typeof(MusicLibrary))]
[RequireComponent(typeof(AmbientLibrary))]
public class AudioManager : MonoBehaviour
{
#region Variables
	public static AudioManager Instance;
	public enum AudioChannel 
	{ 
		Master, 
		Music, 
		Ambient, 
		fx 
	};
	public enum SnapshotType 
	{ 
		Default, 
		Combat, 
		Stealth, 
		Underwater 
	}

	[Title("Volume", null, true, true)]
	[SerializeField, Range(0,1)] private float masterVolume = 1; // Overall volume
	[SerializeField, Range(0,1)] private float musicVolume = 1f; // Music volume
	[SerializeField, Range(0,1)] private float ambientVolume = 1; // Ambient volume
	[SerializeField, Range(0,1)] private float fxVolume = 1; // FX volume

	[Title(null, "isLooping", true, true)]
	[SerializeField] private bool MusicIsLooping = true;
	[SerializeField] private bool AmbientIsLooping = true;

	[Title("Mixers", "Objects", true, true)]
	[SerializeField, Required] private AudioMixer mainMixer;
	[SerializeField, Required] private AudioMixerGroup musicGroup;
	[SerializeField, Required] private AudioMixerGroup ambientGroup;
	[SerializeField, Required] private AudioMixerGroup fxGroup;

	[Title(null, "Snapshots", true, true)]
	[SerializeField] private AudioMixerSnapshot defaultSnapshot;
	[SerializeField] private AudioMixerSnapshot combatSnapshot;
	[SerializeField] private AudioMixerSnapshot stealthSnapshot;
	[SerializeField] private AudioMixerSnapshot underwaterSnapshot;

	[Title("SFX Pool", null, true, true)]
	[SerializeField, Required] private AudioSourcePool fxPool;
	[SerializeField] private int poolSize = 10;

	// Seperate audiosources
	private AudioSource musicSource;
	private AudioSource ambientSource;
	private AudioSource fxSource;

	// Sound libraries. All your audio clips
	private SoundLibrary soundLibrary;
	private MusicLibrary musicLibrary;
	private AmbientLibrary ambientLibrary;
#endregion

#region Unity Methods
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		// Get FX, Music and Ambient sound library
		soundLibrary = GetComponent<SoundLibrary>();
		musicLibrary = GetComponent<MusicLibrary>();
		ambientLibrary = GetComponent<AmbientLibrary>();

		// Create audio sources
		CreateAudioSources();

		// Get FX, Music and Ambient sound mixer groups
		fxSource.outputAudioMixerGroup = fxGroup;
		musicSource.outputAudioMixerGroup = musicGroup;
		ambientSource.outputAudioMixerGroup = ambientGroup;

		// Set volume on all the channels
		SetChannelVolumes();

		// Initialize 3D SFX pool
		InitFXPool();
	}
#endregion

#region Volume Controls
	private void SetChannelVolumes()
	{
		SetVolume(masterVolume, AudioChannel.Master);
		SetVolume(fxVolume, AudioChannel.fx);
		SetVolume(musicVolume, AudioChannel.Music);
		SetVolume(ambientVolume, AudioChannel.Ambient);
	}

	public void SetVolume(float volumePercent, AudioChannel channel) 
	{
		float volumeDB = Mathf.Log10(Mathf.Clamp(volumePercent, 0.0001f, 1f)) * 20;

		switch (channel) 
		{
			case AudioChannel.Master:
				mainMixer.SetFloat("MasterVolume", volumeDB);
				break;
			case AudioChannel.fx:
				mainMixer.SetFloat("FXVolume", volumeDB);
				break;
			case AudioChannel.Music:
				mainMixer.SetFloat("MusicVolume", volumeDB);
				break;
			case AudioChannel.Ambient:
				mainMixer.SetFloat("AmbientVolume", volumeDB);
				break;
		}
	}
#endregion

#region Music controls
	// Play music with delay. 0 = No delay
	public void PlayMusic(string musicName, float delay)
	{
		var clip = musicLibrary.GetClipFromName(musicName);
		if (clip == null) {
			Debug.LogWarning($"Music clip '{musicName}' not found.");
			return;
		}
		musicSource.clip = clip;
		musicSource.PlayDelayed(delay);
	}

	// Play music fade in
	public IEnumerator PlayMusicFade(string musicName, float duration)
	{
		float startVolume = 0;
		float targetVolume = musicSource.volume;
		float currentTime = 0;

		var clip = musicLibrary.GetClipFromName(musicName);
		if (clip == null) {
			Debug.LogWarning($"Music clip '{musicName}' not found.");
			return;
		}
		musicSource.clip = clip;
		musicSource.Play();

		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
			yield return null;
		}
		yield break;
	}

	// Stop music
	public void StopMusic()
	{
		musicSource.Stop();
	}

	// Stop music fading out
	public IEnumerator StopMusicFade(float duration)
	{
		float currentVolume = musicSource.volume;
		float startVolume = musicSource.volume;
		float targetVolume = 0;
		float currentTime = 0;

		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
			yield return null;
		}
		musicSource.Stop();
		musicSource.volume = currentVolume;
		yield break;
	}
#endregion

#region Ambient controls
	// Play ambient sound with delay 0 = No delay
	public void PlayAmbient(string ambientName, float delay)
	{
		var clip = ambientLibrary.GetClipFromName(ambientName);
		if (clip == null) {
			Debug.LogWarning($"ambient clip '{ambientName}' not found.");
			return;
		}
		ambientSource.clip = clip;
		ambientSource.PlayDelayed(delay);
	}

	public IEnumerator PlayAmbientFade(string ambientName, float duration)
	{
		float startVolume = 0;
		float targetVolume = ambientSource.volume;
		float currentTime = 0;

		var clip = ambientLibrary.GetClipFromName(ambientName);
		if (clip == null) {
			Debug.LogWarning($"ambient clip '{ambientName}' not found.");
			return;
		}
		ambientSource.clip = clip;
		ambientSource.Play();

		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
			yield return null;
		}
		yield break;
	}

	// Stop ambient sound fading out
	public IEnumerator StopAmbientFade(float duration)
	{
		float currentVolume = ambientSource.volume;
		float startVolume = ambientSource.volume;
		float targetVolume = 0;
		float currentTime = 0;

		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
			yield return null;
		}

		ambientSource.Stop();
		ambientSource.volume = currentVolume; // Reset volume for next playback
		yield break;
	}

	// Crossfade ambient sound
	public IEnumerator CrossfadeAmbient(string newClipName, float duration)
	{
		AudioClip newClip = ambientLibrary.GetClipFromName(newClipName);
		if (newClip == null) yield break;

		AudioSource tempSource = gameObject.AddComponent<AudioSource>();
		tempSource.clip = newClip;
		tempSource.outputAudioMixerGroup = ambientGroup;
		tempSource.loop = AmbientIsLooping;
		tempSource.volume = 0;
		tempSource.Play();

		float time = 0;
		float startVolume = ambientSource.volume;

		while (time < duration)
		{
			time += Time.deltaTime;
			float t = time / duration;
			ambientSource.volume = Mathf.Lerp(startVolume, 0, t);
			tempSource.volume = Mathf.Lerp(0, startVolume, t);
			yield return null;
		}

		ambientSource.Stop();
		Destroy(ambientSource);
		ambientSource = tempSource;
	}

	// Stop ambient sound
	public void StopAmbient()
	{
		ambientSource.Stop();
	}
#endregion

#region Sfx Controls
	// FX Audio
	public void PlaySound2D(string soundName)
	{
		var clip = soundLibrary.GetClipFromName(soundName);
		if (clip == null) {
			Debug.LogWarning($"Sound clip '{soundName}' not found.");
			return;
		}
		fxSource.PlayOneShot(clip, fxVolume * masterVolume);
	}

	public void PlaySound3D(string soundName, Vector3 soundPosition)
	{
		var clip = soundLibrary.GetClipFromName(soundName);
		if (clip == null) {
			Debug.LogWarning($"Sound clip '{soundName}' not found.");
			return;
		}
		fxPool.PlayClip(clip, soundPosition, fxVolume * masterVolume);
	}
#endregion

#region Misc Methods
	private void InitFXPool()
	{
		if (fxPool != null) return;
		
		GameObject poolObj = new("FX Pool");
		poolObj.transform.parent = transform;
		fxPool = poolObj.AddComponent<AudioSourcePool>();
		fxPool.fxGroup = fxGroup;
		fxPool.poolSize = poolSize;
	}

	// Snapshot Transitions
	public void TransitionToSnapshot(SnapshotType snapshot, float transitionTime) 
	{
		switch (snapshot) 
		{
			case SnapshotType.Default:
				defaultSnapshot.TransitionTo(transitionTime);
				break;
			case SnapshotType.Combat:
				combatSnapshot.TransitionTo(transitionTime);
				break;
			case SnapshotType.Stealth:
				stealthSnapshot.TransitionTo(transitionTime);
				break;
			case SnapshotType.Underwater:
				underwaterSnapshot.TransitionTo(transitionTime);
				break;
			default:
				Debug.LogWarning($"Snapshot '{snapshotName}' not found.");
				break;
		}
	}

	private void CreateAudioSources()
	{
		GameObject newfxSource = new GameObject("2D fx source");
		fxSource = newfxSource.AddComponent<AudioSource>();
		newfxSource.transform.parent = transform;
		fxSource.playOnAwake = false;

		GameObject newMusicSource = new GameObject("Music source");
		musicSource = newMusicSource.AddComponent<AudioSource>();
		newMusicSource.transform.parent = transform;
		musicSource.loop = MusicIsLooping; // Music is looping
		musicSource.playOnAwake = false;

		GameObject newAmbientsource = new GameObject("Ambient source");
		ambientSource = newAmbientsource.AddComponent<AudioSource>();
		newAmbientsource.transform.parent = transform;
		ambientSource.loop = AmbientIsLooping; // Ambient sound is looping
		ambientSource.playOnAwake = false;
	}
#endregion

#region Helper Methods
	public bool MusicIsPlaying() => musicSource != null && musicSource.isPlaying;
	public string GetCurrentMusicName() => musicSource.clip != null ? musicSource.clip.name : "None";

	public bool AmbientIsPlaying() => ambientSource != null && ambientSource.isPlaying;
	public string GetCurrentAmbientName() => ambientSource.clip != null ? ambientSource.clip.name : "None";

	public SoundLibrary GetSoundLibrary() => soundLibrary;

	public bool TryGetSoundNames(out string[] names)
	{
		if (soundLibrary == null)
		{
			names = null;
			return false;
		}

		names = soundLibrary.GetAllClipNames();
		return names != null && names.Length > 0;
	}

	public bool TryGetMusicNames(out string[] names)
	{
		if (musicLibrary == null)
		{
			names = null;
			return false;
		}

		names = musicLibrary.GetAllClipNames();
		return names != null && names.Length > 0;
	}

	public bool TryGetAmbientNames(out string[] names)
	{
		if (ambientLibrary == null)
		{
			names = null;
			return false;
		}

		names = ambientLibrary.GetAllClipNames();
		return names != null && names.Length > 0;
	}

	public float GetMixerVolumeDB(string parameter)
	{
		if (mainMixer.GetFloat(parameter, out float value))
			return value;
		return -80f; // Silence
	}

	public void SetMixerParameter(string parameterName, float value)
	{
		if (!mainMixer.SetFloat(parameterName, value))
		{
			Debug.LogWarning($"Mixer parameter '{parameterName}' not found.");
		}
	}

	public float GetMixerParameter(string parameterName)
	{
		if (mainMixer.GetFloat(parameterName, out float value))
			return value;

		Debug.LogWarning($"Mixer parameter '{parameterName}' not found.");
		return -1f;
	}
#endregion
}