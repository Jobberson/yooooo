using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveCheckpoint()
    {
        SaveManager.Instance.SaveStoryState(StoryManager.Instance.CurrentState);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            SaveManager.Instance.SavePlayerPosition(player.transform.position);

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
                inventory.SaveInventoryState();
        }

        Debug.Log("Checkpoint saved.");
    }

    public void LoadCheckpoint()
    {
        StoryManager.Instance.LoadSavedState();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = SaveManager.Instance.LoadPlayerPosition();

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
                inventory.LoadInventoryState();
        }

        Debug.Log("Checkpoint loaded.");
    }
}
