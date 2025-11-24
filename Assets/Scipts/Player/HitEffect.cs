using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject postProcessing;
    
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakeIntensity = 0.2f;
    
    [SerializeField] private float vignetteIntensity = 0.4f;
    [SerializeField] private float vignetteDuration = 0.5f;
    
    private Vector3 originalPosition;
    private float currentShakeTime;
    private bool isShaking;
    private Volume postProcessVolume;
    private Vignette vignette;

    private void OnEnable()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (postProcessVolume == null)
        {
            postProcessVolume = postProcessing.GetComponent<Volume>();
            if (postProcessVolume.profile.TryGet(out Vignette vig))
            {
                vignette = vig;
            }
        }
    }

    public void TriggerHitEffect()
    {
        StartCoroutine(ShakeCamera());
        StartCoroutine(VignetteEffect());
    }

    private IEnumerator ShakeCamera()
    {
        originalPosition = playerCamera.transform.localPosition;
        currentShakeTime = 0f;
        isShaking = true;

        while (currentShakeTime < shakeDuration)
        {
            if (playerCamera != null)
            {
                Vector3 randomPoint = originalPosition + Random.insideUnitSphere * shakeIntensity;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, randomPoint, Time.deltaTime * 5f);
            }

            currentShakeTime += Time.deltaTime;
            yield return null;
        }

        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = originalPosition;
        }
        isShaking = false;
    }

    private IEnumerator VignetteEffect()
    {
        if (vignette != null)
        {
            vignette.active = true;
            vignette.intensity.Override(vignetteIntensity);
            vignette.color.Override(Color.red);

            // Fade out of the vignette effect
            float elapsedTime = 0f;
            while (elapsedTime < vignetteDuration)
            {
                float normalizedTime = elapsedTime / vignetteDuration;
                vignette.intensity.Override(Mathf.Lerp(vignetteIntensity, 0f, normalizedTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset the vignette effect
            vignette.intensity.Override(0f);
            vignette.active = false;
        }
    }
}
