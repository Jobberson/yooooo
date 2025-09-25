using UnityEngine;

public class MemoryCleaner : MonoBehaviour
{
    public bool autoClean = true;
    public float cleanInterval = 30f;

    private float timer;

    void Update()
    {
        if (!autoClean) return;

        timer += Time.deltaTime;
        if (timer >= cleanInterval)
        {
            CleanMemory();
            timer = 0f;
        }
    }

    public void CleanMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        Debug.Log("Memory cleaned.");
    }
}
