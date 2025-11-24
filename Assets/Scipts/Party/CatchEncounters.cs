using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class CatchEncounters : MonoBehaviour
{
    [SerializeField] private RawImage pulsingImage;
    [SerializeField] private GameObject captureButtonUI;
    [SerializeField] private GameObject captureProgressBarBG;
    [SerializeField] private Image captureProgressBar;
    [SerializeField] private float captureTime = 2f;
    [SerializeField] private float captureThreshold = 0.4f; // 40%
    [SerializeField] private NotificationPopUp notificationPopUp;
    [SerializeField] private Transform playerTransform;

    private EnemyHealth targetEnemy;
    private EnemyAttack enemyAttack;
    private float captureProgress;
    private bool isCapturing;

    private GameObject partyMember;

    private void Start()
    {
        if (captureButtonUI != null && pulsingImage != null)
        {
            captureButtonUI.SetActive(false);
            pulsingImage.gameObject.SetActive(false);
        }
        captureProgress = 0f;
    }

    private void Update()
    {
        if (targetEnemy != null)
        {
            float healthPercentage = (float)targetEnemy.GetCurrentHealth() / targetEnemy.GetMaxHealth();

            if (healthPercentage <= captureThreshold && !isCapturing)
            {
                captureButtonUI.SetActive(true);
                pulsingImage.gameObject.SetActive(true);
                captureProgressBarBG.gameObject.SetActive(true);
            }

            if (Input.GetKey(KeyCode.X) && healthPercentage <= captureThreshold)
            {
                StartCapture();
            }
            else if (Input.GetKeyUp(KeyCode.X))
            {
                StopCapture();
            }

            if (isCapturing)
            {
                UpdateCaptureProgress();
            }
        } 
        else
        {
            captureButtonUI.SetActive(false);
            pulsingImage.gameObject.SetActive(false); 
        }

            AttackEnemy();
    }

    private void AttackEnemy()
    {
        if (partyMember != null && Input.GetKeyDown(KeyCode.G))
        {
            EnemyHealth nearestEnemy = targetEnemy;
            if (nearestEnemy != null)
            {
                EnemyAttack partyAttack = partyMember.GetComponent<EnemyAttack>();
                if (partyAttack != null)
                {
                    partyAttack.enabled = true;
                    EnemyAI partyAI = partyMember.GetComponent<EnemyAI>();
                    if (partyAI != null)
                    {
                        Transform nearestEnemyTransform = nearestEnemy.GetComponent<Transform>();
                        partyAI.SetTarget(nearestEnemyTransform);
                    }
                    notificationPopUp?.ShowNotification("<color=yellow>Party member attacks!</color>");
                    enemyAttack.SetTarget(partyMember.transform);
                }
            }
        }
        else if (partyMember != null && targetEnemy == null)
        {
            EnemyAttack partyAttack = partyMember.GetComponent<EnemyAttack>();
            if (partyAttack != null)
            {
                partyAttack.enabled = false;
                EnemyAI partyAI = partyMember.GetComponent<EnemyAI>();
                if (partyAI != null)
                {
                    playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                    partyAI.SetTarget(playerTransform);
                }
            }
        }
    }

    private void StartCapture()
    {
        if (!isCapturing && targetEnemy != null)
        {
            isCapturing = true;
        }
    }

    private void StopCapture()
    {
        if (isCapturing)
        {
            isCapturing = false;
            captureProgress = 0f;
            if (captureProgressBar != null)
            {
                captureProgressBar.fillAmount = 0f;
            }
        }
    }

    private void UpdateCaptureProgress()
    {
        captureProgress += Time.deltaTime / captureTime;

        if (captureProgressBar != null)
        {
            captureProgressBar.fillAmount = captureProgress;
        }

        if (captureProgress >= 1f)
        {
            CaptureEnemy();
        }
    }

    private void CaptureEnemy()
    {
        if (targetEnemy != null)
        {
            notificationPopUp?.ShowNotification("<color=green>Enemy Captured!</color>");
            AddToParty(targetEnemy.gameObject);
            ResetCaptureState();
        }
    }

    private void AddToParty(GameObject enemy)
    {
        if (partyMember != null)
        {
            Destroy(partyMember);
        }
        enemy.GetComponent<EnemyAI>().SetChaseRange(Mathf.Infinity);       
        enemy.GetComponent<EnemyAttack>().enabled = false;
        enemy.GetComponent<EnemyHealth>().RestoreHealth();
        partyMember = enemy;
    }

    private void ResetCaptureState()
    {
        isCapturing = false;
        captureProgress = 0f;
        targetEnemy = null;
        enemyAttack = null;
        captureButtonUI.SetActive(false);
        pulsingImage.gameObject.SetActive(false);

        if (captureProgressBar != null)
        {
            captureProgressBar.fillAmount = 0f;
        }
    }

    public void SetTargetEnemy(EnemyHealth enemy)
    {
        targetEnemy = enemy;
        enemyAttack = enemy.GetComponent<EnemyAttack>();
    }
}
