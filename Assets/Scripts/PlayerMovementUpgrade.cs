using UnityEngine;

public class PlayerMovementUpgrade : MonoBehaviour
{
    public static PlayerMovementUpgrade Instance { get; private set; }

    private bool doubleJumpEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EnableDoubleJump()
    {
        doubleJumpEnabled = true;
        Debug.Log("Double Jump Enabled");
    }

    public bool HasDoubleJump()
    {
        return doubleJumpEnabled;
    }
}