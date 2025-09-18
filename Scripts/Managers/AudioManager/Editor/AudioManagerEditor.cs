using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
#region Variables
    private AudioManager manager;
    private string[] snapshotOptions = { "Default", "Combat", "Stealth", "Underwater" };
    private int selectedSnapshotIndex = 0;

    private string[] soundNames;
    private int selectedSoundIndex = 0;
    private Vector3 soundPosition = Vector3.zero;

    private string[] musicNames;
    private int selectedMusicIndex = 0;

    private string[] ambientNames;
    private int selectedAmbientIndex = 0;

    private float fadeDuration = 2f;
    private float playDelay = 0f;
#endregion

#region OnEnable
    private void OnEnable()
    {
        manager = (AudioManager)target;

        if (manager.GetSoundLibrary() != null)
        {
            soundNames = manager.GetSoundLibrary().GetAllSoundNames();
            selectedSoundIndex = 0;
        }
        else
        {
            soundNames = new string[] { "No SFX Available" };
        }

        if (manager.TryGetMusicNames(out musicNames) && musicNames.Length > 0)
        {
            selectedMusicIndex = 0;
        }
        else
        {
            musicNames = new[] { "No music found" };
        }

        if (manager.TryGetAmbientNames(out ambientNames) && ambientNames.Length > 0)
        {
            selectedAmbientIndex = 0;
        }    
        else
        {
            ambientNames = new[] { "No ambient found" };
        }
    }
#endregion

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🔧 Runtime Tools", EditorStyles.boldLabel);
        
#region 2D Sound Test
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🧪 2D Sound Test", EditorStyles.boldLabel);
        selectedSoundIndex = EditorGUILayout.Popup("Test SFX", selectedSoundIndex, soundNames);
        if (GUILayout.Button("▶ Play Selected SFX"))
        {
            manager.PlaySound2D(soundNames[selectedSoundIndex]);
        }
#endregion

#region 3D Sound Test
        EditorGUILayout.LabelField("🧪 3D Sound Test", EditorStyles.boldLabel);
        selectedSoundIndex = EditorGUILayout.Popup("Sound", selectedSoundIndex, soundNames);
        soundPosition = EditorGUILayout.Vector3Field("Position", soundPosition);
        if (GUILayout.Button("📍 Play 3D Sound"))
        {
            manager.PlaySound3D(soundNames[selectedSoundIndex], soundPosition);
        }
#endregion

#region Music Test
    EditorGUILayout.Space();
    EditorGUILayout.LabelField("🎶 Music Test", EditorStyles.boldLabel);

    selectedMusicIndex = EditorGUILayout.Popup("Music Clip", selectedMusicIndex, musicNames);
    playDelay = EditorGUILayout.FloatField("Play Delay (sec)", playDelay);
    fadeDuration = EditorGUILayout.FloatField("Fade Duration (sec)", fadeDuration);

    if (GUILayout.Button("▶ Play Music"))
        manager.PlayMusic(musicNames[selectedMusicIndex], playDelay);

    if (GUILayout.Button("🌅 Fade In Music"))
        manager.StartCoroutine(manager.PlayMusicFade(musicNames[selectedMusicIndex], fadeDuration));

    if (GUILayout.Button("⏹ Stop Music"))
        manager.StopMusic();

    if (GUILayout.Button("🌄 Fade Out Music"))
        manager.StartCoroutine(manager.StopMusicFade(fadeDuration));
#endregion

#region Ambient Test
    EditorGUILayout.Space();
    EditorGUILayout.LabelField("🌲 Ambient Test", EditorStyles.boldLabel);

    selectedAmbientIndex = EditorGUILayout.Popup("Ambient Clip", selectedAmbientIndex, ambientNames);

    if (GUILayout.Button("▶ Play Ambient"))
        manager.PlayAmbient(ambientNames[selectedAmbientIndex], playDelay);

    if (GUILayout.Button("🌅 Fade In Ambient"))
        manager.StartCoroutine(manager.PlayAmbientFade(ambientNames[selectedAmbientIndex], fadeDuration));

    if (GUILayout.Button("⏹ Stop Ambient"))
        manager.StopAmbient();

    if (GUILayout.Button("🌄 Fade Out Ambient"))
        manager.StartCoroutine(manager.StopAmbientFade(fadeDuration));
#endregion

#region Snapshot Control
        selectedSnapshotIndex = EditorGUILayout.Popup("Snapshot", selectedSnapshotIndex, snapshotOptions);
        if (GUILayout.Button("🎚 Switch Snapshot"))
        {
            manager.TransitionToSnapshot(snapshotOptions[selectedSnapshotIndex], 1f);
        }
#endregion

#region Info Display
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🎵 Playback Info", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Music:", manager.MusicIsPlaying() ? manager.GetCurrentMusicName() : "None");
        EditorGUILayout.LabelField("Ambient:", manager.AmbientIsPlaying() ? manager.GetCurrentAmbientName() : "None");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📦 FX Pool Usage", EditorStyles.boldLabel);

        if (manager.fxPool != null)
        {
            EditorGUILayout.LabelField("Pool Size:", manager.fxPool.poolSize.ToString());
            EditorGUILayout.LabelField("Active Sources:", manager.fxPool.GetActiveSourceCount().ToString());
        }
        else
        {
            EditorGUILayout.HelpBox("FX Pool not initialized.", MessageType.Warning);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📊 Mixer Levels", EditorStyles.boldLabel);

        DrawMixerMeter("MasterVolume", "Master");
        DrawMixerMeter("MusicVolume", "Music");
        DrawMixerMeter("AmbientVolume", "Ambient");
        DrawMixerMeter("FXVolume", "FX");
#endregion
    }

    private void DrawMixerMeter(string parameterName, string label)
    {
        float db = manager.GetMixerVolumeDB(parameterName);
        float normalized = Mathf.InverseLerp(-80f, 0f, db); // Normalize dB to 0–1

        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, normalized, $"{label}: {db:F1} dB");
        GUILayout.Space(5);
    }
}
