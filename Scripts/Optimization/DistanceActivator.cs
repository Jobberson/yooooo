using UnityEngine;

public class DistanceActivator : MonoBehaviour
{
    public Transform player;
    public float activationDistance = 150f;
    public GameObject[] objectsToToggle;

    private bool isActive = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < activationDistance && !isActive)
        {
            SetObjectsActive(true);
            isActive = true;
        }
        else if (distance >= activationDistance && isActive)
        {
            SetObjectsActive(false);
            isActive = false;
        }
    }

    void SetObjectsActive(bool state)
    {
        foreach (GameObject obj in objectsToToggle)
        {
            obj.SetActive(state);
        }
    }
}
