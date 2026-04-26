using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] ActiveWeapon activeWeapon;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI weaponNameText;

    [Header("Health")]
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] TextMeshProUGUI healthText;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI totalScoreText;

    [Header("Kill Streak")]
    [SerializeField] TextMeshProUGUI killStreakText;

    [Header("Heal Ability UI")]
    [SerializeField] Image healAbilityImage;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color cooldownColor = Color.gray;

    [Header("Wave UI")]
    [SerializeField] TextMeshProUGUI waveText;

    private void Start()
    {
        if (killStreakText != null)
            killStreakText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // -------- WEAPON --------
        Weapon weapon = activeWeapon.CurrentWeapon;

        if (weapon == null)
        {
            ammoText.text = "";
            weaponNameText.text = "";
        }
        else
        {
            weaponNameText.text = weapon.Data.weaponName;
            ammoText.text = $"{weapon.CurrentAmmo} / {weapon.Data.maxAmmo}";
        }

        // -------- HEALTH --------
        if (playerHealth != null)
        {
            healthText.text = "HP: " + playerHealth.GetHPValue();
        }

        // -------- SCORE --------
        if (ScoreManager.Instance != null)
        {
            currentScoreText.text = "Score: " + ScoreManager.Instance.currentScore;
            totalScoreText.text = "Total: " + ScoreManager.Instance.totalScore;
        }

        // -------- KILL STREAK --------
        if (ScoreManager.Instance != null && killStreakText != null)
        {
            bool active = ScoreManager.Instance.IsStreakActive;

            if (killStreakText.gameObject.activeSelf != active)
            {
                killStreakText.gameObject.SetActive(active);
            }
        }

        // -------- HEAL ABILITY --------
        if (playerHealth != null && healAbilityImage != null)
        {
            if (playerHealth.IsHealOnCooldown)
                healAbilityImage.color = cooldownColor;
            else
                healAbilityImage.color = normalColor;
        }

        // -------- WAVE UI --------
        if (WaveManager.Instance != null && waveText != null)
        {
            if (WaveManager.Instance.IsWaveInProgress)
            {
                waveText.text = "Wave " + WaveManager.Instance.CurrentWave;
            }
            else
            {
                waveText.text = "Go to the next zone";
            }
        }
    }
}