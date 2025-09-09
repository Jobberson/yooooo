using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SoundLibraryEditorWindow : EditorWindow {
    private List<SoundClipData> soundAssets = new();
    private Vector2 scrollPos;

    [MenuItem("Tools/Audio/Sound Library Manager")]
    public static void ShowWindow() {
        GetWindow<SoundLibraryEditorWindow>("Sound Library Manager");
    }

    private void OnEnable() {
        LoadSoundAssets();
    }

    private void LoadSoundAssets() {
        soundAssets.Clear();
        string[] guids = AssetDatabase.FindAssets("t:SoundClipData");
        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SoundClipData asset = AssetDatabase.LoadAssetAtPath<SoundClipData>(path);
            if (asset != null) soundAssets.Add(asset);
        }
    }

    private void OnGUI() {
        GUILayout.Label("Sound Clip Assets", EditorStyles.boldLabel);
        if (GUILayout.Button("Refresh List")) {
            LoadSoundAssets();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var sound in soundAssets) {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Name:", sound.soundName);
            sound.priority = (SoundPriority)EditorGUILayout.EnumPopup("Priority:", sound.priority);

            if (sound.clips != null && sound.clips.Length > 0) {
                if (GUILayout.Button("Preview Random Clip")) {
                    AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];
                    EditorUtility.PlayClip(clip);
                }
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }
}
