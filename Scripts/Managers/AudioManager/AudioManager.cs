using UnityEngine;
using System.Collections;
using AwesomeAttributes;
using UnityEngine.Audio;

[RequireComponent(typeof(SoundLibrary))]
[RequireComponent(typeof(MusicLibrary))]
[RequireComponent(typeof(AmbientLibrary))]
public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;
	public enum AudioChannel 
	{ 
		Master, 
		Music, 
		Ambient, 
		fx 
	};

	[Title("Volume", null, true, true)]
	[SerializeField, Range(0,1)] private float masterVolume = 1; // Overall volume
	[SerializeField, Range(0,1)] private float musicVolume = 1f; // Music volume
	[SerializeField, Range(0,1)] private float ambientVolume = 1; // Ambient volume
	[SerializeField, Range(0,1)] private float fxVolume = 1; // FX volume

	[Title(null, "isLooping", true, true)]
	[SerializeField] private bool MusicIsLooping = true;
	[SerializeField] private bool AmbientIsLooping = true;

	[Title("Mixers", "Objects", true, true)]
	[SerializeField] private AudioMixer mainMixer;
	[SerializeField] private AudioMixerGroup musicGroup;
	[SerializeField] private AudioMixerGroup ambientGroup;
	[SerializeField] private AudioMixerGroup fxGroup;

	[Title(null, "Snapshots", true, true)]
	[SerializeField] private AudioMixerSnapshot defaultSnapshot;
	[SerializeField] private AudioMixerSnapshot combatSnapshot;
	[SerializeField] private AudioMixerSnapshot stealthSnapshot;
	[SerializeField] private AudioMixerSnapshot underwaterSnapshot;

	[Title("SFX Pool", null, true, true)]
	[SerializeField] private AudioSourcePool fxPool;
	[SerializeField] private int poolSize = 10;

	// Seperate audiosources
	private AudioSource musicSource;
	private AudioSource ambientSource;
	private AudioSource fxSource;

	// Sound libraries. All your audio clips
	private SoundLibrary soundLibrary;
	private MusicLibrary musicLibrary;
	private AmbientLibrary ambientLibrary;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject); // Optional

		// Get FX, Music and Ambient sound library
		soundLibrary = GetComponent<SoundLibrary>();
		musicLibrary = GetComponent<MusicLibrary>();
		ambientLibrary = GetComponent<AmbientLibrary>();

		// Get FX, Music and Ambient sound mixer groups
		fxSource.outputAudioMixerGroup = fxGroup;
		musicSource.outputAudioMixerGroup = musicGroup;
		ambientSource.outputAudioMixerGroup = ambientGroup;

		// Create audio sources
		CreateAudioSources();

		// Set volume on all the channels
		SetChannelVolumes();

		// Initialize pool
		InitFXPool();
	}

	private void InitFXPool()
	{
		if (fxPool != null) return;
		
		GameObject poolObj = new("FX Pool");
		poolObj.transform.parent = transform;
		fxPool = poolObj.AddComponent<AudioSourcePool>();
		fxPool.fxGroup = fxGroup;
		fxPool.poolSize = poolSize;
	}

	private void SetChannelVolumes()
	{
		SetVolume(masterVolume, AudioChannel.Master);
		SetVolume(fxVolume, AudioChannel.fx);
		SetVolume(musicVolume, AudioChannel.Music);
		SetVolume(ambientVolume, AudioChannel.Ambient);
	}

	// Set volume on all the channels
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

	// Play music with delay. 0 = No delay
	public void PlayMusic(string musicName, float delay)
	{
		musicSource.clip = musicLibrary.GetClipFromName(musicName);
		musicSource.PlayDelayed(delay);
	}

	// Play music fade in
	public IEnumerator PlayMusicFade(string musicName, float duration)
	{
		float startVolume = 0;
		float targetVolume = musicSource.volume;
		float currentTime = 0;

		musicSource.clip = musicLibrary.GetClipFromName(musicName);
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

	// Stop music fade out
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

	// Play ambient sound with delay 0 = No delay
	public void PlayAmbient(string ambientName, float delay)
	{
		ambientSource.clip = ambientLibrary.GetClipFromName(ambientName);
		ambientSource.PlayDelayed(delay);
	}

	public IEnumerator PlayAmbientFade(string ambientName, float duration)
	{
		float startVolume = 0;
		float targetVolume = ambientSource.volume;
		float currentTime = 0;

		ambientSource.clip = ambientLibrary.GetClipFromName(ambientName);
		ambientSource.Play();

		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
			yield return null;
		}

		yield break;
	}

	// Stop ambient sound
	public void StopAmbient()
	{
		ambientSource.Stop();
	}

	// FX Audio
	public void PlaySound2D(string soundName)
	{
		fxSource.PlayOneShot(soundLibrary.GetClipFromName(soundName), fxVolume * masterVolume);
	}

	public void PlaySound3D(string soundName, Vector3 soundPosition)
	{
		AudioClip clip = soundLibrary.GetClipFromName(soundName);
		fxPool.PlayClip(clip, soundPosition, fxVolume * masterVolume);
	}

	// Snapshot Transitions
	public void TransitionToSnapshot(string snapshotName, float transitionTime) {
		switch (snapshotName) {
			case "Default":
				defaultSnapshot.TransitionTo(transitionTime);
				break;
			case "Combat":
				combatSnapshot.TransitionTo(transitionTime);
				break;
			case "Stealth":
				stealthSnapshot.TransitionTo(transitionTime);
				break;
			case "Underwater":
				underwaterSnapshot.TransitionTo(transitionTime);
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
}