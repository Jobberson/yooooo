using UnityEngine;

public class DemoTimer : MonoBehaviour
{
    private TimerManager timerManager;

    void Start()
    {
        timerManager = FindAnyObjectByType<TimerManager>();

        // Spawn enemy after 5 seconds
        timerManager.AddTimer(5f, () => SpawnEnemy("Goblin"));

        // Spawn another enemy after 10 seconds
        timerManager.AddTimer(10f, () => SpawnEnemy("Orc"));
    }

    void SpawnEnemy(string enemyType)
    {
        Debug.Log($"Spawned enemy: {enemyType}");
    }
}
