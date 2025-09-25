using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ZoneLoader : MonoBehaviour
{
    public Transform player;
    public string addressableSceneName;
    public float loadDistance = 150f;

    private bool isLoaded = false;
    private AsyncOperationHandle<SceneInstance> loadHandle;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < loadDistance && !isLoaded)
        {
            LoadZone();
        }
        else if (distance >= loadDistance && isLoaded)
        {
            UnloadZone();
        }
    }

    void LoadZone()
    {
        loadHandle = Addressables.LoadSceneAsync(addressableSceneName, LoadSceneMode.Additive);
        isLoaded = true;
    }

    void UnloadZone()
    {
        Addressables.UnloadSceneAsync(loadHandle);
        isLoaded = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Orange transparent
        Gizmos.DrawSphere(transform.position, loadDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loadDistance);
    }
}
