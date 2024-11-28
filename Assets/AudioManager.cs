using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }
    
    public int maxSimultaneousSounds = 5;
    public float globalVolume = 1f;  // Global volume multiplier
    private Queue<AudioSource> audioSourcePool;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
            
        InitializeAudioPool();
    }

    void InitializeAudioPool()
    {
        audioSourcePool = new Queue<AudioSource>();
        for (int i = 0; i < maxSimultaneousSounds; i++)
        {
            GameObject audioObj = new GameObject("AudioSource_" + i);
            audioObj.transform.parent = transform;
            AudioSource source = audioObj.AddComponent<AudioSource>();
            audioSourcePool.Enqueue(source);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume * globalVolume;  // Combine with global volume
            source.Play();
            StartCoroutine(ReturnToPool(source, clip.length));
        }
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSourcePool.Enqueue(source);
    }
}