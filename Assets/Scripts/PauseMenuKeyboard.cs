using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using StarterAssets;

public class PauseMenuKeyboard : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    [Header("Menu Options")]
    [SerializeField] private TextMeshProUGUI[] options;

    private int currentIndex = 0;
    private bool isPaused = false;

    private FirstPersonController playerController;
    private PlayerInput playerInput;
    private ActiveWeapon activeWeapon;

    private void Start()
    {
        pauseUI.SetActive(false);
        UpdateSelection();

        playerController = FindObjectOfType<FirstPersonController>();
        playerInput = FindObjectOfType<PlayerInput>();
        activeWeapon = FindObjectOfType<ActiveWeapon>();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        if (!isPaused) return;

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = options.Length - 1;
            UpdateSelection();
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            currentIndex++;
            if (currentIndex >= options.Length) currentIndex = 0;
            UpdateSelection();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Select();
        }
    }

    void Pause()
    {
        isPaused = true;
        pauseUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🔥 disable gameplay
        if (playerController != null)
            playerController.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false;

        if (activeWeapon != null)
            activeWeapon.enabled = false;
    }

    void Resume()
    {
        isPaused = false;
        pauseUI.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 🔥 re-enable gameplay
        if (playerController != null)
            playerController.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;

        if (activeWeapon != null)
            activeWeapon.enabled = true;
    }

    void Select()
    {
        switch (currentIndex)
        {
            case 0:
                Resume();
                break;

            case 1:
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
                break;
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }
}