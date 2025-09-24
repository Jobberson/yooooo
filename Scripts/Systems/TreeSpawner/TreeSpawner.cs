using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class TreeSpawner : MonoBehaviour
{
    [Header("Tree Settings")]
    public GameObject[] treePrefabs; // Array of tree prefabs
    public int treeCount = 1000;

    [Header("Terrain Settings")]
    public Terrain terrain;
    public float minHeight = 0f;
    public float maxHeight = 1000f;
    public float maxSlopeAngle = 30f;

    [Header("Randomization")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);

    public void SpawnTrees()
    {
        if (terrain == null || treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogWarning("Terrain or Tree Prefabs not assigned.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        for (int i = 0; i < treeCount; i++)
        {
            float x = Random.Range(0, terrainSize.x);
            float z = Random.Range(0, terrainSize.z);
            Vector3 position = new Vector3(x, 0, z);
            float y = terrain.SampleHeight(position);
            position.y = y;

            if (y < minHeight || y > maxHeight)
                continue;

            Vector3 normal = terrainData.GetInterpolatedNormal(x / terrainSize.x, z / terrainSize.z);
            float slopeAngle = Vector3.Angle(normal, Vector3.up);

            if (slopeAngle > maxSlopeAngle)
                continue;

            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            float scale = Random.Range(scaleRange.x, scaleRange.y);

            GameObject selectedPrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            GameObject tree = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;
            tree.transform.position = position;
            tree.transform.rotation = rotation;
            tree.transform.localScale = Vector3.one * scale;
            tree.transform.parent = this.transform;
        }

        Debug.Log($"{treeCount} trees attempted to spawn using {treePrefabs.Length} prefabs.");
    }

    public void ClearTrees()
    {
        int removed = 0;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
            removed++;
        }

        Debug.Log($"Cleared {removed} trees.");
    }
}
