using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip overworldMusic;
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private AudioClip victoryMusic;
    
    [Header("Audio Settings")]
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float musicVolume = 0.7f;
    
    private AudioSource[] audioSources;
    private int activeSource = 0;
    private bool isTransitioning = false;

    private void Start()
    {
        // Create two audio sources for cross-fading
        audioSources = new AudioSource[2];
        for (int i = 0; i < 2; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].loop = true;
            audioSources[i].playOnAwake = false;
            audioSources[i].volume = 0f;
        }

        PlayOverworldMusic();
    }

    public void PlayOverworldMusic()
    {
        if (!isTransitioning && audioSources[activeSource].clip != overworldMusic)
        {
            StartCoroutine(CrossFadeMusic(overworldMusic));
        }
    }

    public void PlayCombatMusic()
    {
        if (!isTransitioning && audioSources[activeSource].clip != combatMusic)
        {
            StartCoroutine(CrossFadeMusic(combatMusic));
        }
    }

    public void PlayVictoryMusic()
    {
        if (!isTransitioning)
        {
            StartCoroutine(PlayVictoryStinger());
        }
    }

    private IEnumerator CrossFadeMusic(AudioClip newClip)
    {
        isTransitioning = true;
        int newSource = 1 - activeSource;

        // Set up new clip
        audioSources[newSource].clip = newClip;
        audioSources[newSource].volume = 0f;
        audioSources[newSource].Play();

        // Cross fade
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            audioSources[newSource].volume = Mathf.Lerp(0f, musicVolume, t);
            audioSources[activeSource].volume = Mathf.Lerp(musicVolume, 0f, t);
            yield return null;
        }

        // Stop the old source
        audioSources[activeSource].Stop();
        activeSource = newSource;
        isTransitioning = false;
    }

    private IEnumerator PlayVictoryStinger()
    {
        isTransitioning = true;
        
        audioSources[activeSource].volume = musicVolume;
        audioSources[activeSource].clip = victoryMusic;
        audioSources[activeSource].Play();

        yield return new WaitForSeconds(victoryMusic.length);

        audioSources[activeSource].clip = overworldMusic;
        audioSources[activeSource].volume = musicVolume;
        audioSources[activeSource].Play();

        isTransitioning = false;
    }
}
