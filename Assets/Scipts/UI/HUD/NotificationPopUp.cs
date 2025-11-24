using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationPrefab;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float spacing = 60f;
    [SerializeField] private int maxNotifications = 3;
    [SerializeField] private float slideDistance = 300f;
    [SerializeField] private float slideInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private List<(GameObject obj, Coroutine routine)> activeNotifications = new List<(GameObject obj, Coroutine routine)>();
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowNotification(string message)
    {
        while (activeNotifications.Count >= maxNotifications)
        {
            var oldest = activeNotifications[0];
            if (oldest.routine != null)
                StopCoroutine(oldest.routine);
            Destroy(oldest.obj);
            activeNotifications.RemoveAt(0);
        }

        // Create new notification
        GameObject newNotification = Instantiate(notificationPrefab.gameObject, transform);
        RectTransform newRect = newNotification.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = newNotification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = newNotification.AddComponent<CanvasGroup>();
        
        // Position it outside the visible area to the left
        float yOffset = -spacing * activeNotifications.Count;
        newRect.anchoredPosition = new Vector2(-slideDistance, yOffset);

        TextMeshProUGUI tmpText = newNotification.GetComponent<TextMeshProUGUI>();
        tmpText.text = message;

        // Start fade out routine
        Coroutine routine = StartCoroutine(AnimateNotification(newNotification, newRect, canvasGroup, yOffset));
        activeNotifications.Add((newNotification, routine));
    }

    private IEnumerator AnimateNotification(GameObject notification, RectTransform rect, CanvasGroup canvasGroup, float targetY)
    {
        float elapsedTime = 0f;
        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = new Vector2(0, targetY);

        // Slide in
        while (elapsedTime < slideInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / slideInDuration;
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsedTime / fadeOutDuration);
            yield return null;
        }

        // Remove and destroy the notification
        var index = activeNotifications.FindIndex(x => x.obj == notification);
        if (index != -1)
        {
            activeNotifications.RemoveAt(index);
            Destroy(notification);

            // Reposition remaining notifications
            for (int i = index; i < activeNotifications.Count; i++)
            {
                RectTransform remainingRect = activeNotifications[i].obj.GetComponent<RectTransform>();
                remainingRect.anchoredPosition = new Vector2(0, -spacing * i);
            }
        }
    }
}
