using UnityEngine;
using UnityEngine.UI;

public class PulsingEffect : MonoBehaviour
{
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float pulseSpeed = 2f;
    
    private RawImage image;
    private float currentTime;
    private Vector3 originalScale;

    private void Start()
    {
        image = GetComponent<RawImage>();
        if (image == null)
        {
            Debug.LogWarning("No RawImage component found on GameObject with PulsingEffect script.");
        }
        originalScale = transform.localScale;
    }

    private void Update()
    {
        currentTime += Time.deltaTime * pulseSpeed;
        
        // Calculate the current scale using a sine wave
        float scaleModifier = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(currentTime) + 1f) * 0.5f);
        transform.localScale = originalScale * scaleModifier;
    }
}
