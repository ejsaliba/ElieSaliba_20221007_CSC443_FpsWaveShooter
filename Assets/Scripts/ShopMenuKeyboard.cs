using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using StarterAssets;

public class ShopMenuKeyboard : MonoBehaviour
{
    public static ShopMenuKeyboard Instance;

    [Header("UI")]
    [SerializeField] private GameObject shopUI;
    [SerializeField] private TextMeshProUGUI[] options;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI pointsText; // 🔥 NEW

    [Header("Weapon Data")]
    [SerializeField] private WeaponData machineGunData;
    [SerializeField] private WeaponData sniperData;
    [SerializeField] private WeaponData rocketData;

    private int currentIndex = 0;
    private bool isOpen = false;
    public bool IsOpen => isOpen;

    private WeaponSwitcher weaponSwitcher;
    private FirstPersonController playerController;
    private PlayerInput playerInput;
    private ActiveWeapon activeWeapon;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        shopUI.SetActive(false);
        UpdateSelection();

        weaponSwitcher = FindObjectOfType<WeaponSwitcher>();
        playerController = FindObjectOfType<FirstPersonController>();
        playerInput = FindObjectOfType<PlayerInput>();
        activeWeapon = FindObjectOfType<ActiveWeapon>();
    }

    private void Update()
    {
        if (!isOpen) return;

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

        RefreshPoints(); // 🔥 live update while open
    }

    // ---------------- OPEN / CLOSE ----------------

    public void OpenShop()
    {
        isOpen = true;

        shopUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        if (playerController != null) playerController.enabled = false;
        if (playerInput != null) playerInput.enabled = false;
        if (activeWeapon != null) activeWeapon.enabled = false;

        ClearMessage();
        RefreshPoints(); // 🔥 update immediately on open
    }

    void CloseShop()
    {
        isOpen = false;

        shopUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        if (playerController != null) playerController.enabled = true;
        if (playerInput != null) playerInput.enabled = true;
        if (activeWeapon != null) activeWeapon.enabled = true;

    }

    // ---------------- INPUT ----------------

    void Select()
    {
        switch (currentIndex)
        {
            case 0: BuyDoubleJump(); break;
            case 1: BuyMachineGun(); break;
            case 2: BuySniper(); break;
            case 3: BuyRocket(); break;
            case 4: BuyAmmo(); break;
            case 5: CloseShop(); break;
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }

    // ---------------- SHOP LOGIC ----------------

    bool Spend(int cost)
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager missing!");
            return false;
        }

        if (ScoreManager.Instance.currentScore < cost)
        {
            ShowMessage("Not enough points!");
            return false;
        }

        ScoreManager.Instance.currentScore -= cost;
        RefreshPoints(); // 🔥 update UI immediately
        return true;
    }

   void BuyDoubleJump()
{
    if (!Spend(100)) return;

    if (PlayerMovementUpgrade.Instance == null)
    {
        ShowMessage("Double Jump system not found");
        Debug.LogError("PlayerMovementUpgrade.Instance is NULL");
        return;
    }

    PlayerMovementUpgrade.Instance.EnableDoubleJump();
    ShowMessage("Double Jump Unlocked");
}
    void BuyMachineGun()
    {
        if (!Spend(200)) return;
        UnlockWeapon(machineGunData);
    }

    void BuySniper()
    {
        if (!Spend(300)) return;
        UnlockWeapon(sniperData);
    }

    void BuyRocket()
    {
        if (!Spend(500)) return;
        UnlockWeapon(rocketData);
    }

   void BuyAmmo()
{
    if (!Spend(100)) return;

    Weapon[] weapons = weaponSwitcher.GetComponentsInChildren<Weapon>(true);

    foreach (Weapon w in weapons)
    {
        if (w == null) continue;

        w.RefillAmmo();
    }

    ShowMessage("All weapons reloaded");
}

    void UnlockWeapon(WeaponData data)
    {
        Weapon[] weapons = weaponSwitcher.GetComponentsInChildren<Weapon>(true);

        foreach (Weapon w in weapons)
        {
            if (w.Data == data)
            {
                weaponSwitcher.UnlockWeapon(w);
                ShowMessage("Weapon Unlocked");
                return;
            }
        }

        ShowMessage("Weapon not found");
    }

    // ---------------- UI ----------------

    void RefreshPoints()
    {
        if (pointsText == null) return;
        if (ScoreManager.Instance == null)
        {
            pointsText.text = "Points: 0";
            return;
        }

        pointsText.text = "Points: " + ScoreManager.Instance.currentScore;
    }

    void ShowMessage(string msg)
    {
        if (messageText == null) return;

        messageText.text = msg;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), 2f);
    }

    void ClearMessage()
    {
        if (messageText != null)
            messageText.text = "";
    }
}