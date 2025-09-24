using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TreeSpawner))]
public class TreeSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TreeSpawner spawner = (TreeSpawner)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Spawn Trees in Editor"))
        {
            spawner.SpawnTrees();
        }

        if (GUILayout.Button("Clear Spawned Trees"))
        {
            spawner.ClearTrees();
        }
    }
}
