using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetPreloader : MonoBehaviour
{
    public AssetReference[] assetsToPreload;

    void Start()
    {
        foreach (var asset in assetsToPreload)
        {
            asset.LoadAssetAsync<GameObject>().Completed += handle =>
            {
                Debug.Log($"Preloaded: {handle.Result.name}");
            };
        }
    }
}
