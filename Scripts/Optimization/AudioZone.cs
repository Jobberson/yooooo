using UnityEngine;

public class AudioZone : MonoBehaviour
{
    public Transform player;
    public AudioSource ambientAudio;
    public float activationDistance = 100f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        ambientAudio.enabled = distance < activationDistance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.4f); // Light blue, semi-transparent
        Gizmos.DrawSphere(transform.position, activationDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
