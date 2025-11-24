using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip[] backgroundMusicClips;
    [SerializeField] private bool randomizeOnSwitch = true;
    [SerializeField] private float fadeDuration = 2f;

    private AudioSource audioSourceA;
    private AudioSource audioSourceB;
    private int currentTrackIndex = -1;
    private bool isFading = false;
    private AudioSource activeSource;
    private AudioSource nextSource;

    private void Start()
    {
        audioSourceA = gameObject.AddComponent<AudioSource>();
        audioSourceB = gameObject.AddComponent<AudioSource>();
        audioSourceA.loop = false;
        audioSourceB.loop = false;
        audioSourceA.volume = 1f;
        audioSourceB.volume = 0f;

        activeSource = audioSourceA;
        nextSource = audioSourceB;

        if (backgroundMusicClips != null && backgroundMusicClips.Length > 0)
        {
            PlayNextTrackImmediate();
        }
        else
        {
            Debug.LogWarning("No background music clips assigned!");
        }
    }

    private void Update()
    {
        if (!isFading && activeSource.isPlaying)
        {
            float timeRemaining = activeSource.clip.length - activeSource.time;
            if (timeRemaining <= fadeDuration)
            {
                StartCoroutine(CrossfadeToNextTrack());
            }
        }
        // If not playing and not fading, start next track immediately (edge case)
        else if (!isFading && !activeSource.isPlaying)
        {
            PlayNextTrackImmediate();
        }
    }

    private IEnumerator CrossfadeToNextTrack()
    {
        isFading = true;

        // Select next track
        int nextIndex = GetNextTrackIndex();
        nextSource.clip = backgroundMusicClips[nextIndex];
        nextSource.volume = 0f;
        nextSource.Play();

        float t = 0f;
        float startVolume = activeSource.volume;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeDuration;
            activeSource.volume = Mathf.Lerp(startVolume, 0f, progress);
            nextSource.volume = Mathf.Lerp(0f, 1f, progress);
            yield return null;
        }
        activeSource.volume = 0f;
        nextSource.volume = 1f;
        activeSource.Stop();

        // Swap sources
        var temp = activeSource;
        activeSource = nextSource;
        nextSource = temp;
        currentTrackIndex = nextIndex;

        isFading = false;
    }

    private void PlayNextTrackImmediate()
    {
        int nextIndex = GetNextTrackIndex();
        activeSource.clip = backgroundMusicClips[nextIndex];
        activeSource.volume = 1f;
        activeSource.Play();
        currentTrackIndex = nextIndex;
    }

    private int GetNextTrackIndex()
    {
        if (backgroundMusicClips.Length == 0) return 0;

        int newIndex;
        if (backgroundMusicClips.Length == 1)
        {
            newIndex = 0;
        }
        else if (randomizeOnSwitch)
        {
            do
            {
                newIndex = Random.Range(0, backgroundMusicClips.Length);
            } while (newIndex == currentTrackIndex);
        }
        else
        {
            newIndex = (currentTrackIndex + 1) % backgroundMusicClips.Length;
        }
        return newIndex;
    }

    private void OnDestroy()
    {
        if (audioSourceA != null)
        {
            audioSourceA.Stop();
            Destroy(audioSourceA);
        }
        if (audioSourceB != null)
        {
            audioSourceB.Stop();
            Destroy(audioSourceB);
        }
    }
}
