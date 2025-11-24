using UnityEngine;

public class WeaponShootingSFX : MonoBehaviour
{
    [SerializeField] private AudioClip shootingSFX;
    [SerializeField] private AudioClip SwitchingSFX;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the WeaponShootingSFX GameObject.");
        }
    }

    public void PlayShootingSFX()
    {
        audioSource.clip = shootingSFX;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopShootingSFX()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

    public void PlaySwitchingSFX()
    {
        if (audioSource != null && SwitchingSFX != null)
        {
            audioSource.PlayOneShot(SwitchingSFX);
        }
    }
}
