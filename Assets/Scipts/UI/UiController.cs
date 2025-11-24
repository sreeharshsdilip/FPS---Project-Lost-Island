using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject questMenu;

    FirstPersonController player;

    private void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (questMenu != null)
        {
            questMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            player.gameObject.GetComponent<MonoBehaviour>().enabled = false;
        }

        Time.timeScale = 1f;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverMenu.activeSelf && !questMenu.activeSelf)
        {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !gameOverMenu.activeSelf && !pauseMenu.activeSelf)
        {
            OnAccpetQuestButtonPressed();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            bool isActive = pauseMenu.activeSelf;
            pauseMenu.SetActive(!isActive);
            Time.timeScale = isActive ? 1f : 0f;

            Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;

            // Make sure that the camera doesn't move in pause menu
            player.gameObject.GetComponent<MonoBehaviour>().enabled = isActive ? true : false;

        }
    }

    public void ShowGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;

            player.gameObject.GetComponent<MonoBehaviour>().enabled = false;
            Time.timeScale = 0f;
        }
    }

    public void OnRetryButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButtonPressed()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnMainMenuButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void OnAccpetQuestButtonPressed()
    {
        questMenu.SetActive(false);
        player.gameObject.GetComponent<MonoBehaviour>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
