using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneChunkLoader : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunksPerAxis = 4;       // e.g., 4x4 grid
    public float chunkSize = 250f;      // size of each terrain chunk
    public float loadRadius = 300f;     // how far from player to load chunks

    [Header("Player Reference")]
    public Transform player;

    private HashSet<string> loadedScenes = new HashSet<string>();

    void Update()
    {
        if (player == null)
            return;

        Vector3 playerPos = player.position;

        for (int x = 0; x < chunksPerAxis; x++)
        {
            for (int z = 0; z < chunksPerAxis; z++)
            {
                Vector3 chunkCenter = new Vector3(
                    x * chunkSize + chunkSize / 2f,
                    0,
                    z * chunkSize + chunkSize / 2f
                );

                float distance = Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z), chunkCenter);
                string sceneName = $"TerrainChunk_{x}_{z}";

                if (distance <= loadRadius)
                {
                    if (!loadedScenes.Contains(sceneName))
                    {
                        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                        loadedScenes.Add(sceneName);
                        Debug.Log($"Loaded: {sceneName}");
                    }
                }
                else
                {
                    if (loadedScenes.Contains(sceneName))
                    {
                        SceneManager.UnloadSceneAsync(sceneName);
                        loadedScenes.Remove(sceneName);
                        Debug.Log($"Unloaded: {sceneName}");
                    }
                }
            }
        }
    }
}
