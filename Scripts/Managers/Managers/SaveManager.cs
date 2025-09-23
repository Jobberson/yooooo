using UnityEngine;
using AA.SaveSystem;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string StoryStateKey = "story_state";
    private const string PlayerPositionKey = "player_position";
    private const string HeldItemKey = "held_item_name";

    private const string KeyFolderName = "Bogus";
    private const string KeyFileName = "bogus.dat";

    private const string SavesFolderName = "Saves";
    private const string SavesFileExtension = ".save";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SaveSystem.EncryptionEnabled = true;
        SaveSystem.EncryptionKey = LoadOrGenerateEncryptionKey();
        
        // Set custom save path and extension for story data
        SaveSystem.SavePath = Path.Combine(Application.persistentDataPath, SavesFolderName);
        SaveSystem.FileExtension = SaveFileExtension;

        // Ensure folders exist
        Directory.CreateDirectory(SaveSystem.SavePath);
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, KeyFolderName));
    }

#region Encryption
    private string LoadOrGenerateEncryptionKey()
    {
        string keyFolderPath = Path.Combine(Application.persistentDataPath, KeyFolderName);
        string keyFilePath = Path.Combine(keyFolderPath, KeyFileName);

        if (File.Exists(keyFilePath))
        {
            string savedKey = File.ReadAllText(keyFilePath);
            Debug.Log("Loaded encryption key from file.");
            return savedKey;
        }
        else
        {
            string newKey = GenerateRuntimeKey();
            File.WriteAllText(keyFilePath, newKey);
            Debug.Log("Generated and saved new encryption key.");
            return newKey;
        }
    }

    private string GenerateRuntimeKey()
    {
        string baseKey = SystemInfo.deviceUniqueIdentifier + Application.companyName + Application.productName;
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(baseKey);
            byte[] hash = sha.ComputeHash(bytes);
            return System.Convert.ToBase64String(hash);
        }
    }
#endregion

#region StoryState Saving
    public void SaveStoryState(StoryState state)
    {
        UIManager.Instance.ShowSaveIndicator();
        SaveSystem.Save(StoryStateKey, (int)state);
        Debug.Log($"Encrypted story state saved: {state}");
    }

    public StoryState LoadStoryState()
    {
        if (SaveSystem.HasKey(StoryStateKey))
        {
            int stateIndex = SaveSystem.Load<int>(StoryStateKey);
            StoryState loadedState = (StoryState)stateIndex;
            Debug.Log($"Encrypted story state loaded: {loadedState}");
            return loadedState;
        }

        Debug.Log("No saved story state found. Using default.");
        return StoryState.beforeGenerator;
    }

    public void DeleteStoryState()
    {
        SaveSystem.Delete(StoryStateKey);
        Debug.Log("Deleted encrypted story state.");
    }
#endregion

#region PlayerPosition Saving
    public void SavePlayerPosition(Vector3 position)
    {
        SaveSystem.Save(PlayerPositionKey, position);
        Debug.Log($"Encrypted player position saved: {position}");
    }

    public Vector3 LoadPlayerPosition()
    {
        if (SaveSystem.HasKey(PlayerPositionKey))
        {
            Vector3 loadedPosition = SaveSystem.Load<Vector3>(PlayerPositionKey);
            Debug.Log($"Player position loaded: {loadedPosition}");
            return loadedPosition;
        }

        Debug.Log("No saved player position found. Using default (0,0,0).");
        return Vector3.zero;
    }

    public void DeletePlayerPosition()
    {
        SaveSystem.Delete(PlayerPositionKey);
        Debug.Log("Deleted encrypted player position.");
    }
#endregion

#region Inventory Saving
    public void SaveHeldItem(GameObject item)
    {
        if (item != null)
        {
            SaveSystem.Save(HeldItemKey, item.name);
            Debug.Log($"Saved held item: {item.name}");
        }
        else
        {
            SaveSystem.Delete(HeldItemKey);
            Debug.Log("No item held. Cleared held item save.");
        }
    }

    public GameObject LoadHeldItem()
    {
        if (SaveSystem.HasKey(HeldItemKey))
        {
            string itemName = SaveSystem.Load<string>(HeldItemKey);
            GameObject foundItem = GameObject.Find(itemName);
            if (foundItem != null)
            {
                Debug.Log($"Loaded held item: {itemName}");
                return foundItem;
            }
            else
            {
                Debug.LogWarning($"Held item '{itemName}' not found in scene.");
            }
        }

        return null;
    }
#endregion
}
