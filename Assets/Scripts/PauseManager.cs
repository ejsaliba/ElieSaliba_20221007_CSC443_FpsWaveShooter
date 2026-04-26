using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using StarterAssets;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [SerializeField] private GameObject pauseUI;

    private bool isPaused = false;

    private FirstPersonController playerController;
    private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pauseUI.SetActive(false);

        playerController = FindObjectOfType<FirstPersonController>();
        playerInput = FindObjectOfType<PlayerInput>();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver())
                return;

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;

        pauseUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🔥 HARD STOP EVERYTHING
        if (playerController != null)
            playerController.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false; // ✅ THIS is the real fix
    }

    public void Resume()
    {
        isPaused = false;

        pauseUI.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 🔥 RESTORE INPUT
        if (playerController != null)
            playerController.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}