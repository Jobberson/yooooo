using UnityEngine;

public class AudioSourcePool : MonoBehaviour 
{
    public int poolSize = 10;
    public AudioMixerGroup fxGroup;
    private Queue<AudioSource> pool = new Queue<AudioSource>();

    void Awake() 
    {
        for (int i = 0; i < poolSize; i++) {
            GameObject go = new GameObject("PooledAudioSource");
            go.transform.parent = transform;
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = fxGroup;
            pool.Enqueue(source);
        }
    }

    public void PlayClip(AudioClip clip, Vector3 position, float volume) 
    {
        AudioSource source = pool.Dequeue();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f;
        source.Play();
        StartCoroutine(ReturnToPool(source, clip.length));
    }

    IEnumerator ReturnToPool(AudioSource source, float delay) 
    {
        yield return new WaitForSeconds(delay);
        pool.Enqueue(source);
    }
}
