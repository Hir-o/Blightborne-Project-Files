using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesUnlocker : MonoBehaviour
{
    [Header("Trader Dialogue Box")]
    [SerializeField]
    private TraderStaticDialogue traderDialogue;

    [Header("Buttons")]
    [SerializeField]
    private Button powerButton;

    [SerializeField] private Button iceButton;
    [SerializeField] private Button fireButton;
    [SerializeField] private Button specialButton;

    [Header("Ability Costs")]
    [SerializeField]
    private int powerShotCost;

    [SerializeField] private int iceShotCost;
    [SerializeField] private int fireShotCost;
    [SerializeField] private int specialCost;

    [Header("Ability Images")]
    [SerializeField]
    private Image powerShotImage;

    [SerializeField] private Image iceShotImage;
    [SerializeField] private Image fireShotImage;
    [SerializeField] private Image specialImage;

    [Header("Ability Image Colors")]
    [SerializeField]
    private Color unlockedColor;

    [SerializeField] private Color lockedColor;

    private AbilityChooser abilityChooser;

    private void OnEnable ()
    {
        UpdateImages();
    }

    private void Start ()
    {
        powerButton.GetComponent<Animator>().SetBool("isClickable", true);
        abilityChooser = FindObjectOfType<AbilityChooser>();

        UpdateButtons();
    }

    private void UpdateImages ()
    {
        if (PlayerStats.Strength     >= 2
            && PlayerStats.Dexterity >= 2
            && PlayerStats.Vitality  >= 1
            && PlayerStats.Coins     >= powerShotCost) { powerShotImage.color = unlockedColor; }
        else if (PlayerStats.UnlockPowerArrow)
            powerShotImage.color = unlockedColor;
        else
            powerShotImage.color = lockedColor;

        if (PlayerStats.UnlockPowerArrow
            && PlayerStats.Strength    >= 4
            && PlayerStats.Dexterity   >= 5
            && PlayerStats.AttackSpeed >= 2
            && PlayerStats.Armor       >= 2
            && PlayerStats.Coins       >= iceShotCost) { iceShotImage.color = unlockedColor; }
        else if (PlayerStats.UnlockIceArrow)
            iceShotImage.color = unlockedColor;
        else
            iceShotImage.color = lockedColor;

        if (PlayerStats.UnlockIceArrow
            && PlayerStats.Strength  >= 8
            && PlayerStats.Dexterity >= 7
            && PlayerStats.Vitality  >= 5
            && PlayerStats.Coins     >= fireShotCost) { fireShotImage.color = unlockedColor; }
        else if (PlayerStats.UnlockFireArrow)
            fireShotImage.color = unlockedColor;
        else
            fireShotImage.color = lockedColor;

        if (PlayerStats.UnlockFireArrow
            && PlayerStats.Strength    >= 10
            && PlayerStats.Dexterity   >= 10
            && PlayerStats.Vitality    >= 8
            && PlayerStats.AttackSpeed >= 4
            && PlayerStats.Armor       >= 4
            && PlayerStats.Coins       >= specialCost) { specialImage.color = unlockedColor; }
        else if (PlayerStats.UnlockSpecialArrow)
            specialImage.color = unlockedColor;
        else
            specialImage.color = lockedColor;
    }

    private void UpdateButtons ()
    {
        abilityChooser.UpdateTextures();

        if (PlayerStats.UnlockPowerArrow)
        {
            powerButton.GetComponent<Button>().interactable = false;
            powerButton.GetComponent<Animator>().SetTrigger("Disabled");

            iceButton.GetComponent<Animator>().SetBool("isClickable", true);
        }

        if (PlayerStats.UnlockIceArrow)
        {
            iceButton.GetComponent<Button>().interactable = false;
            iceButton.GetComponent<Animator>().SetTrigger("Disabled");

            fireButton.GetComponent<Animator>().SetBool("isClickable", true);
        }

        if (PlayerStats.UnlockFireArrow)
        {
            fireButton.GetComponent<Button>().interactable = false;
            fireButton.GetComponent<Animator>().SetTrigger("Disabled");

            specialButton.GetComponent<Animator>().SetBool("isClickable", true);
        }

        if (PlayerStats.UnlockSpecialArrow)
        {
            specialButton.GetComponent<Button>().interactable = false;
            specialButton.GetComponent<Animator>().SetTrigger("Disabled");
        }

        UpdateImages();
    }

    public void TrainPowerShot ()
    {
        if (PlayerStats.Strength     < 2
            || PlayerStats.Dexterity < 2
            || PlayerStats.Vitality  < 1) { traderDialogue.NotEnoughSkillPointsDialogue(); }
        else if (PlayerStats.Coins < powerShotCost) { traderDialogue.NotEnoughCashDialogue(); }

        if (PlayerStats.Strength     >= 2
            && PlayerStats.Dexterity >= 2
            && PlayerStats.Vitality  >= 1
            && PlayerStats.Coins     >= powerShotCost)
        {
            AudioController.Instance.PurchaseSFX();
            PlayerStats.UnlockPowerArrow = true;

            PlayerStats.Coins -= powerShotCost;
            EssentialObjects.UpdateCoinsStatic();
            
//            PokiUnitySDK.Instance.happyTime(0.8f);
        }
        else if (powerButton.GetComponent<Button>().interactable) { AudioController.Instance.ButtonClickSFX(); }

        UpdateButtons();
    }

    public void TrainIceShot ()
    {
        if (PlayerStats.UnlockPowerArrow == false
            || PlayerStats.Strength      < 4
            || PlayerStats.Dexterity     < 5
            || PlayerStats.AttackSpeed   < 2
            || PlayerStats.Armor         < 2) { traderDialogue.NotEnoughSkillPointsDialogue(); }
        else if (PlayerStats.Coins < iceShotCost) { traderDialogue.NotEnoughCashDialogue(); }

        if (PlayerStats.UnlockPowerArrow
            && PlayerStats.Strength    >= 4
            && PlayerStats.Dexterity   >= 5
            && PlayerStats.AttackSpeed >= 2
            && PlayerStats.Armor       >= 2
            && PlayerStats.Coins       >= iceShotCost)
        {
            AudioController.Instance.PurchaseSFX();
            PlayerStats.UnlockIceArrow = true;
            PlayerStats.MaxAbilityIndex++;

            PlayerStats.Coins -= iceShotCost;
            EssentialObjects.UpdateCoinsStatic();
            
//            PokiUnitySDK.Instance.happyTime(0.8f);
        }
        else if (PlayerStats.UnlockPowerArrow) { AudioController.Instance.ButtonClickSFX(); }

        UpdateButtons();
    }

    public void TrainFireShot ()
    {
        if (PlayerStats.UnlockIceArrow == false
            || PlayerStats.Strength    < 8
            || PlayerStats.Dexterity   < 7
            || PlayerStats.Vitality    < 5) { traderDialogue.NotEnoughSkillPointsDialogue(); }
        else if (PlayerStats.Coins < fireShotCost) { traderDialogue.NotEnoughCashDialogue(); }

        if (PlayerStats.UnlockIceArrow
            && PlayerStats.Strength  >= 8
            && PlayerStats.Dexterity >= 7
            && PlayerStats.Vitality  >= 5
            && PlayerStats.Coins     >= fireShotCost)
        {
            AudioController.Instance.PurchaseSFX();
            PlayerStats.UnlockFireArrow = true;
            PlayerStats.MaxAbilityIndex++;

            PlayerStats.Coins -= fireShotCost;
            EssentialObjects.UpdateCoinsStatic();
            
//            PokiUnitySDK.Instance.happyTime(0.8f);
        }
        else if (PlayerStats.UnlockIceArrow) { AudioController.Instance.ButtonClickSFX(); }

        UpdateButtons();
    }

    public void TrainSpecialShot ()
    {
        if (PlayerStats.UnlockFireArrow == false
            || PlayerStats.Strength     < 10
            || PlayerStats.Dexterity    < 10
            || PlayerStats.Vitality     < 8
            || PlayerStats.AttackSpeed  < 4
            || PlayerStats.Armor        < 4) { traderDialogue.NotEnoughSkillPointsDialogue(); }
        else if (PlayerStats.Coins < specialCost) { traderDialogue.NotEnoughCashDialogue(); }

        if (PlayerStats.UnlockFireArrow
            && PlayerStats.Strength    >= 10
            && PlayerStats.Dexterity   >= 10
            && PlayerStats.Vitality    >= 8
            && PlayerStats.AttackSpeed >= 4
            && PlayerStats.Armor       >= 4
            && PlayerStats.Coins       >= specialCost)
        {
            AudioController.Instance.PurchaseSFX();
            PlayerStats.UnlockSpecialArrow = true;
            PlayerStats.MaxAbilityIndex++;

            PlayerStats.Coins -= specialCost;
            EssentialObjects.UpdateCoinsStatic();
            
//            PokiUnitySDK.Instance.happyTime(0.8f);
        }
        else if (PlayerStats.UnlockFireArrow) { AudioController.Instance.ButtonClickSFX(); }

        UpdateButtons();
    }
}