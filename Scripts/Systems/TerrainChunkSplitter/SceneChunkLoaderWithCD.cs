using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneChunkLoader : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunksPerAxis = 4;
    public float chunkSize = 250f;

    [Header("Player Reference")]
    public Transform player;
    public float loadRadius = 300f;

    [Header("Cooldown Settings")]
    public float loadCooldown = 1f;
    public float unloadGracePeriod = 2f;

    private HashSet<string> loadedScenes = new HashSet<string>();
    private Dictionary<string, float> lastLoadTime = new Dictionary<string, float>();
    private Dictionary<string, float> unloadCandidates = new Dictionary<string, float>();

    void Update()
    {
        if (player == null)
            return;

        Vector3 playerPos = player.position;
        float currentTime = Time.time;

        for (int x = 0; x < chunksPerAxis; x++)
        {
            for (int z = 0; z < chunksPerAxis; z++)
            {
                string sceneName = $"TerrainChunk_{x}_{z}";
                Vector3 chunkCenter = new Vector3(
                    x * chunkSize + chunkSize / 2f,
                    0,
                    z * chunkSize + chunkSize / 2f
                );

                float distance = Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z), chunkCenter);

                if (distance <= loadRadius)
                {
                    // Load logic with cooldown
                    if (!loadedScenes.Contains(sceneName))
                    {
                        if (!lastLoadTime.ContainsKey(sceneName) || currentTime - lastLoadTime[sceneName] >= loadCooldown)
                        {
                            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                            loadedScenes.Add(sceneName);
                            lastLoadTime[sceneName] = currentTime;
                            Debug.Log($"[ChunkLoader] Loaded: {sceneName}");
                        }
                    }

                    // Cancel unload if player returned
                    if (unloadCandidates.ContainsKey(sceneName))
                    {
                        unloadCandidates.Remove(sceneName);
                        Debug.Log($"[ChunkLoader] Cancelled unload: {sceneName}");
                    }
                }
                else
                {
                    // Mark for unloading with grace period
                    if (loadedScenes.Contains(sceneName) && !unloadCandidates.ContainsKey(sceneName))
                    {
                        unloadCandidates[sceneName] = currentTime;
                        Debug.Log($"[ChunkLoader] Marked for unload: {sceneName}");
                    }
                }
            }
        }

        // Handle unloading after grace period
        List<string> toUnload = new List<string>();
        foreach (var kvp in unloadCandidates)
        {
            if (currentTime - kvp.Value >= unloadGracePeriod)
            {
                toUnload.Add(kvp.Key);
            }
        }

        foreach (string sceneName in toUnload)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            loadedScenes.Remove(sceneName);
            unloadCandidates.Remove(sceneName);
            Debug.Log($"[ChunkLoader] Unloaded: {sceneName}");
        }
    }
}
