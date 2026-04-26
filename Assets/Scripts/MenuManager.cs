using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;



 
    private void Update()
    {
      
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            GoToStart();
        }
    }



    public void GoToStart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(1);
    }
}