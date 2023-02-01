using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioSquenceManager : MonoBehaviour
{
    public AudioSource mainAudioSource;
    public AudioSource secondaryAudioSource;
    public AudioClip[] audioClips;
    public float fadeInSpeed = 0.02f;

    private void Start()
    {
        PlayRandomClip();
    }

    public void PlayRandomClip()
    {
        if (audioClips.Length == 0)
        {
            Debug.LogError("No audio clips assigned to array.");
            return;
        }

        int randomIndex = Random.Range(0, audioClips.Length);
        mainAudioSource.clip = audioClips[randomIndex];
        mainAudioSource.Play();
    }

    void Update()
    {
        if (mainAudioSource.time >= mainAudioSource.clip.length * 0.3f)
        {
            StartCoroutine(FadeIn(secondaryAudioSource, fadeInSpeed));
        }
        if (secondaryAudioSource.time >= secondaryAudioSource.clip.length * 0.95f)
        {
            StartCoroutine(FadeIn(mainAudioSource, fadeInSpeed));
        }
    }

    IEnumerator FadeIn(AudioSource audioSource, float speed)
    {
        audioSource.Play();
        float targetVolume = audioSource.volume;
        audioSource.volume = 0;
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += speed * Time.deltaTime;
            yield return null;
        }
    }
}