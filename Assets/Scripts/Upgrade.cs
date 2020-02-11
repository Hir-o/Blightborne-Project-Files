using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrade : MonoBehaviour
{
    private int maxUpgradeLevel   = 10;
    private int costDividerFactor = 6;

    private                  string maxedText;
    private                  int    tempSkillLevel;
    [SerializeField] private Color  maxedColor;
    [SerializeField] private Color  normalColor;
    [SerializeField] private Color  upgradedColor;

    [Header("Factors")]
    [SerializeField]
    private float incrementalFactor = .23f;

    [Header("Base Values")]
    public float baseHealth = 100f;

    public float baseStamina = 100f;
    public float baseDamage  = 5;

    [Header("Base Cost Prices")]
    private int startingStrengthCost, startingDexterityCost, startingVitalityCost, startingAtsCost, startingArmorCost;

    public int baseStrengthCost    = 12;
    public int baseDexterityCost   = 10;
    public int baseVitalityCost    = 10;
    public int baseAttackSpeedCost = 15;
    public int baseArmorCost       = 20;

    [Header("Upgrade Factors")]
    [SerializeField]
    private float healthFactor;

    [SerializeField] private float staminaFactor;

    [Header("TMPro Skill Points")]
    [SerializeField]
    private TextMeshProUGUI str_Points;

    [SerializeField] private TextMeshProUGUI dex_Points;
    [SerializeField] private TextMeshProUGUI vit_Points;
    [SerializeField] private TextMeshProUGUI ats_Points;
    [SerializeField] private TextMeshProUGUI armor_Points;

    [Header("TMPro Affected Values")]
    [SerializeField]
    private TextMeshProUGUI damageText;

    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI armorText;

    [Header("Upgrade Price Cost")]
    [SerializeField]
    private TextMeshProUGUI strengthCost;

    [SerializeField] private TextMeshProUGUI dexterityCost;
    [SerializeField] private TextMeshProUGUI vitalityCost;
    [SerializeField] private TextMeshProUGUI attackSpeedCost;
    [SerializeField] private TextMeshProUGUI armorCost;

    [Header("Upgrade Buttons")]
    [SerializeField]
    private Button strButton;

    [SerializeField] private Button dexButton;
    [SerializeField] private Button vitButton;
    [SerializeField] private Button atsButton;
    [SerializeField] private Button armorButton;

    [Header("Aquired Skill Icons")]
    [SerializeField]
    private Image imgCursedShot;

    [SerializeField] private Image imgFireShot;
    [SerializeField] private Image imgIceShot;
    [SerializeField] private Image imgPowerShot;

    private CoinSack coinSack;

    private void OnEnable ()
    {
        imgPowerShot.material.SetFloat("_SuperGrayScale_Fade_1", PlayerStats.UnlockPowerArrow ? 0f : 1f);
        imgIceShot.material.SetFloat("_SuperGrayScale_Fade_1", PlayerStats.UnlockIceArrow ? 0f : 1f);
        imgFireShot.material.SetFloat("_SuperGrayScale_Fade_1", PlayerStats.UnlockFireArrow ? 0f : 1f);
        imgCursedShot.material.SetFloat("_SuperGrayScale_Fade_1", PlayerStats.UnlockSpecialArrow ? 0f : 1f);

        Time.timeScale = 0f;
    }

    private void OnDisable () { Time.timeScale = 1f; }

    private void Start ()
    {
        maxedText = "MAX";

        coinSack = GameManager.player.GetComponent<CoinSack>();

        startingStrengthCost  = baseStrengthCost;
        startingDexterityCost = baseDexterityCost;
        startingVitalityCost  = baseVitalityCost;
        startingAtsCost       = baseAttackSpeedCost;
        startingArmorCost     = baseArmorCost;

        if (PlayerStats.Strength != 0)
        {
            baseStrengthCost =
                Mathf.RoundToInt(startingStrengthCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Strength + 1));
            strengthCost.text = baseStrengthCost + "";
            str_Points.text   = PlayerStats.Strength.ToString();
        }
        else
            strengthCost.text = baseStrengthCost + "";

        if (PlayerStats.Dexterity != 0)
        {
            baseDexterityCost =
                Mathf.RoundToInt(startingDexterityCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Dexterity + 1));
            dexterityCost.text = baseDexterityCost + "";
            dex_Points.text    = PlayerStats.Dexterity.ToString();
        }
        else
            dexterityCost.text = baseDexterityCost + "";

        if (PlayerStats.Vitality != 0)
        {
            baseVitalityCost =
                Mathf.RoundToInt(startingVitalityCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Vitality + 1));
            vitalityCost.text = baseVitalityCost + "";
            vit_Points.text   = PlayerStats.Vitality.ToString();
        }
        else
            vitalityCost.text = baseVitalityCost + "";

        if (PlayerStats.AttackSpeed != 0)
        {
            baseAttackSpeedCost =
                Mathf.RoundToInt(startingAtsCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.AttackSpeed + 1));
            attackSpeedCost.text = baseAttackSpeedCost + "";
            ats_Points.text      = PlayerStats.AttackSpeed.ToString();
        }
        else
            attackSpeedCost.text = baseAttackSpeedCost + "";

        if (PlayerStats.Armor != 0)
        {
            baseArmorCost =
                Mathf.RoundToInt(startingArmorCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Armor + 1));
            armorCost.text    = baseArmorCost + "";
            armor_Points.text = PlayerStats.Armor.ToString();
        }
        else
            armorCost.text = baseArmorCost + "";
    }

    public void UpgradeStrength ()
    {
        if (PlayerStats.Strength == maxUpgradeLevel) return;

        if (PlayerStats.Coins >= baseStrengthCost)
        {
            AudioController.Instance.UpgradeSFX();
            PlayerStats.Strength++;
            str_Points.text = PlayerStats.Strength.ToString();
            damageText.text = (baseDamage + PlayerStats.Strength * PlayerStats.StrengthMultiplier).ToString();

            str_Points.color = upgradedColor;
            damageText.color = upgradedColor;

            PlayerStats.Coins -= baseStrengthCost;

            baseStrengthCost =
                Mathf.RoundToInt(startingStrengthCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Strength + 1));
            strengthCost.text = baseStrengthCost.ToString();

            EssentialObjects.UpdateCoinsStatic();
            coinSack.CheckForCoinCapacity();

            if (PlayerStats.Strength == maxUpgradeLevel)
            {
                strengthCost.text  = maxedText;
                strengthCost.color = maxedColor;
            }
        }
        else { AudioController.Instance.ButtonClickSFX(); }
    }

    public void UpgradeDexterity ()
    {
        if (PlayerStats.Dexterity == maxUpgradeLevel) return;

        if (PlayerStats.Coins >= baseDexterityCost)
        {
            AudioController.Instance.UpgradeSFX();
            PlayerStats.Dexterity++;
            PlayerStats.TotalStamina = baseStamina + (PlayerStats.Dexterity * staminaFactor);
            PlayerStats.Stamina      = PlayerStats.TotalStamina;

            dex_Points.text  = PlayerStats.Dexterity.ToString();
            staminaText.text = PlayerStats.Stamina.ToString();

            StaminaBar.ResetStaminaBarStatus();

            PlayerStats.Coins -= baseDexterityCost;
            baseDexterityCost =
                Mathf.RoundToInt(startingDexterityCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Dexterity + 1));
            dexterityCost.text = baseDexterityCost.ToString();

            EssentialObjects.UpdateCoinsStatic();
            coinSack.CheckForCoinCapacity();

            if (PlayerStats.Dexterity == maxUpgradeLevel)
            {
                dexterityCost.text  = maxedText;
                dexterityCost.color = maxedColor;
            }
        }
        else { AudioController.Instance.ButtonClickSFX(); }
    }

    public void UpgradeVitality ()
    {
        if (PlayerStats.Vitality == maxUpgradeLevel) return;

        if (PlayerStats.Coins >= baseVitalityCost)
        {
            AudioController.Instance.UpgradeSFX();
            PlayerStats.Vitality++;
            PlayerStats.TotalHealth = baseHealth + (PlayerStats.Vitality * healthFactor);
            PlayerStats.Health      = PlayerStats.TotalHealth;

            vit_Points.text = PlayerStats.Vitality.ToString();
            healthText.text = PlayerStats.Health.ToString();

            HealthBar.ResetHealthBarStatus();

            PlayerStats.Coins -= baseVitalityCost;
            baseVitalityCost =
                Mathf.RoundToInt(startingVitalityCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Vitality + 1));
            vitalityCost.text = baseVitalityCost.ToString();

            EssentialObjects.UpdateCoinsStatic();
            coinSack.CheckForCoinCapacity();

            if (PlayerStats.Vitality == maxUpgradeLevel)
            {
                vitalityCost.text  = maxedText;
                vitalityCost.color = maxedColor;
            }
        }
        else { AudioController.Instance.ButtonClickSFX(); }
    }

    public void UpgradeAttackSpeed ()
    {
        if (PlayerStats.AttackSpeed == maxUpgradeLevel) return;

        if (PlayerStats.Coins >= baseAttackSpeedCost)
        {
            AudioController.Instance.UpgradeSFX();
            PlayerStats.AttackSpeed++;

            ats_Points.text = PlayerStats.AttackSpeed.ToString();
            reloadText.text = "+" + PlayerStats.AttackSpeed * PlayerStats.AttackSpeedMultiplier + "%";

            PlayerStats.Coins -= baseAttackSpeedCost;
            baseAttackSpeedCost =
                Mathf.RoundToInt(startingAtsCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.AttackSpeed + 1));
            attackSpeedCost.text = baseAttackSpeedCost.ToString();

            EssentialObjects.UpdateCoinsStatic();
            coinSack.CheckForCoinCapacity();

            if (PlayerStats.AttackSpeed == maxUpgradeLevel)
            {
                attackSpeedCost.text  = maxedText;
                attackSpeedCost.color = maxedColor;
            }
        }
        else { AudioController.Instance.ButtonClickSFX(); }
    }

    public void UpgradeArmor ()
    {
        if (PlayerStats.Armor == maxUpgradeLevel) return;

        if (PlayerStats.Coins >= baseArmorCost)
        {
            AudioController.Instance.UpgradeSFX();
            PlayerStats.Armor++;

            armor_Points.text = PlayerStats.Armor.ToString();
            armorText.text    = PlayerStats.Armor * PlayerStats.ArmorMultiplier + "%";

            PlayerStats.Coins -= baseArmorCost;
            baseArmorCost =
                Mathf.RoundToInt(startingArmorCost * Mathf.Pow((1 + incrementalFactor), PlayerStats.Armor + 1));
            armorCost.text = baseArmorCost.ToString();

            EssentialObjects.UpdateCoinsStatic();
            coinSack.CheckForCoinCapacity();

            if (PlayerStats.Armor == maxUpgradeLevel)
            {
                armorCost.text  = maxedText;
                armorCost.color = maxedColor;
            }
        }
        else { AudioController.Instance.ButtonClickSFX(); }
    }

    public void SelectStrength ()
    {
        if (PlayerStats.Strength == maxUpgradeLevel)
        {
            tempSkillLevel  = PlayerStats.Strength;
            str_Points.text = tempSkillLevel.ToString();
            damageText.text = (baseDamage + tempSkillLevel * PlayerStats.StrengthMultiplier).ToString();

            strengthCost.text  = maxedText;
            strengthCost.color = maxedColor;
            str_Points.color   = normalColor;
            damageText.color   = normalColor;
            return;
        }

        // tempSkillLevel = PlayerStats.Strength;
        // tempSkillLevel++;
        // str_Points.text = tempSkillLevel.ToString();
        // damageText.text = (baseDamage + tempSkillLevel * PlayerStats.StrengthMultiplier).ToString();

        str_Points.color = upgradedColor;
        damageText.color = upgradedColor;
    }

    public void DeselectStrength ()
    {
        tempSkillLevel  = PlayerStats.Strength;
        str_Points.text = tempSkillLevel.ToString();
        damageText.text = (baseDamage + tempSkillLevel * PlayerStats.StrengthMultiplier).ToString();

        str_Points.color = normalColor;
        damageText.color = normalColor;
    }

    public void SelectDexterity ()
    {
        if (PlayerStats.Dexterity == maxUpgradeLevel)
        {
            tempSkillLevel   = PlayerStats.Dexterity;
            dex_Points.text  = tempSkillLevel.ToString();
            staminaText.text = (baseStamina + (tempSkillLevel * staminaFactor)).ToString();

            dexterityCost.text  = maxedText;
            dexterityCost.color = maxedColor;
            dex_Points.color    = normalColor;
            staminaText.color   = normalColor;
            return;
        }

        // tempSkillLevel = PlayerStats.Dexterity;
        // tempSkillLevel++;
        // dex_Points.text = tempSkillLevel.ToString();
        // staminaText.text = (baseStamina + (tempSkillLevel * staminaFactor)).ToString();

        dex_Points.color  = upgradedColor;
        staminaText.color = upgradedColor;
    }

    public void DeselectDexterity ()
    {
        tempSkillLevel   = PlayerStats.Dexterity;
        dex_Points.text  = tempSkillLevel.ToString();
        staminaText.text = (baseStamina + (tempSkillLevel * staminaFactor)).ToString();

        dex_Points.color  = normalColor;
        staminaText.color = normalColor;
    }

    public void SelectVitality ()
    {
        if (PlayerStats.Vitality == maxUpgradeLevel)
        {
            tempSkillLevel  = PlayerStats.Vitality;
            vit_Points.text = tempSkillLevel.ToString();
            healthText.text = (baseHealth + (tempSkillLevel * healthFactor)).ToString();

            vitalityCost.text  = maxedText;
            vitalityCost.color = maxedColor;
            vit_Points.color   = normalColor;
            healthText.color   = normalColor;
            return;
        }

        // tempSkillLevel = PlayerStats.Vitality;
        // tempSkillLevel++;
        // vit_Points.text = tempSkillLevel.ToString();
        // healthText.text = (baseHealth + (tempSkillLevel * healthFactor)).ToString();

        vit_Points.color = upgradedColor;
        healthText.color = upgradedColor;
    }

    public void DeselectVitality ()
    {
        tempSkillLevel  = PlayerStats.Vitality;
        vit_Points.text = tempSkillLevel.ToString();
        healthText.text = (baseHealth + (tempSkillLevel * healthFactor)).ToString();

        vit_Points.color = normalColor;
        healthText.color = normalColor;
    }

    public void SelectAttackSpeed ()
    {
        if (PlayerStats.AttackSpeed == maxUpgradeLevel)
        {
            tempSkillLevel  = PlayerStats.AttackSpeed;
            ats_Points.text = tempSkillLevel.ToString();
            reloadText.text = "+" + tempSkillLevel * PlayerStats.AttackSpeedMultiplier + "%";

            attackSpeedCost.text  = maxedText;
            attackSpeedCost.color = maxedColor;
            ats_Points.color      = normalColor;
            reloadText.color      = normalColor;
            return;
        }

        // tempSkillLevel = PlayerStats.AttackSpeed;
        // tempSkillLevel++;
        // ats_Points.text = tempSkillLevel.ToString();
        // reloadText.text = "+" + tempSkillLevel * PlayerStats.AttackSpeedMultiplier + "%"; 

        ats_Points.color = upgradedColor;
        reloadText.color = upgradedColor;
    }

    public void DeselectAttackSpeed ()
    {
        tempSkillLevel  = PlayerStats.AttackSpeed;
        ats_Points.text = tempSkillLevel.ToString();
        reloadText.text = "+" + tempSkillLevel * PlayerStats.AttackSpeedMultiplier + "%";

        ats_Points.color = normalColor;
        reloadText.color = normalColor;
    }

    public void SelectArmor ()
    {
        if (PlayerStats.Armor == maxUpgradeLevel)
        {
            tempSkillLevel    = PlayerStats.Armor;
            armor_Points.text = tempSkillLevel.ToString();
            armorText.text    = tempSkillLevel * PlayerStats.ArmorMultiplier + "%";

            armorCost.text     = maxedText;
            armorCost.color    = maxedColor;
            armor_Points.color = normalColor;
            armorText.color    = normalColor;
            return;
        }

        // tempSkillLevel = PlayerStats.Armor;
        // tempSkillLevel++;
        // armor_Points.text = tempSkillLevel.ToString();
        // armorText.text = tempSkillLevel * PlayerStats.ArmorMultiplier + "%";

        armor_Points.color = upgradedColor;
        armorText.color    = upgradedColor;
    }

    public void DeselectArmor ()
    {
        tempSkillLevel    = PlayerStats.Armor;
        armor_Points.text = tempSkillLevel.ToString();
        armorText.text    = tempSkillLevel * PlayerStats.ArmorMultiplier + "%";

        armor_Points.color = normalColor;
        armorText.color    = normalColor;
    }
}