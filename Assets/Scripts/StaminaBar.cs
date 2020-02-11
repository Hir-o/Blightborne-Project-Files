using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private static StaminaBar instance;

    private static Image staminaBarImage;
    private static Image staminaBarShadowImage;

    [Header("Stamina Regeneration Levers")]
    [SerializeField]
    float startRegenerationWaitTime = 2f;

    [SerializeField] float staminaToRegenerate       = .005f;
    [SerializeField] float regenerationSpeedFactor   = .0f;
    [SerializeField] float staminaRegenerateWaitTime = 2f;

    [Header("New Code for Stamina Regen")]
    [Range(0f, 1f)]
    [SerializeField]
    float staminaRegenFactor = .3f;

    [Header("Stamina Shadow Levers")]
    [SerializeField]
    float shadowReduceFactor = .005f;

    [SerializeField] float startShadowReduceTime   = 1.5f;
    [SerializeField] float shadowReduceSpeedFactor = .0f;

    private bool isRegenerating;
    private bool isReducing;

    private void Awake ()
    {
        if (instance == null) instance = this;
    }

    private void Start ()
    {
        staminaBarImage       = GameObject.Find("Stamina Bar").GetComponent<Image>();
        staminaBarShadowImage = GameObject.Find("Stamina Bar Shadow").GetComponent<Image>();

        staminaBarImage.fillAmount = PlayerStats.Stamina / PlayerStats.TotalStamina;
    }

    private void Update ()
    {
        if (PlayerStats.IsEnergyPyramidPurchased && startShadowReduceTime > 0f) startShadowReduceTime = 0f;

        if (PlayerStats.Stamina / PlayerStats.TotalStamina < .1f) PostProcessingController.Instance.SetSaturationToMinimum();
        else PostProcessingController.Instance.ResetSaturation();
        
        if (PlayerStats.Stamina < PlayerStats.TotalStamina && isRegenerating)
        {
            if (isReducing) { return; }

            PlayerStats.Stamina += staminaRegenFactor + ((PlayerStats.Dexterity / 2) / 10f);

            if (staminaBarImage.fillAmount < staminaBarShadowImage.fillAmount)
                ResetStaminaBarWithoutShadowsStatus();
            else
                ResetStaminaBarStatus();
        }
    }

    public static void ResetStaminaBarStatus ()
    {
        staminaBarImage.fillAmount       = PlayerStats.Stamina / PlayerStats.TotalStamina;
        staminaBarShadowImage.fillAmount = staminaBarImage.fillAmount;
    }

    //Called when stamina shadow is still being reduced
    private void ResetStaminaBarWithoutShadowsStatus ()
    {
        staminaBarImage.fillAmount = PlayerStats.Stamina / PlayerStats.TotalStamina;
    }

    public static void UpdateStaminaBarStatus ()
    {
        staminaBarImage.fillAmount = PlayerStats.Stamina / PlayerStats.TotalStamina;

        instance.StartCoroutine("StaminaShadow");
    }

    private IEnumerator StaminaShadow ()
    {
        isRegenerating = false;
        isReducing     = true;

        yield return new WaitForSeconds(startShadowReduceTime);

        while (staminaBarShadowImage.fillAmount > staminaBarImage.fillAmount)
        {
            staminaBarShadowImage.fillAmount -= shadowReduceFactor;

            yield return new WaitForSeconds(shadowReduceSpeedFactor);
        }
        
        yield return new WaitForSeconds(startShadowReduceTime);

        isRegenerating = true;
        isReducing     = false;
    }

    private IEnumerator RegenerateStamina ()
    {
        yield return new WaitForSeconds(startRegenerationWaitTime);

        while (PlayerStats.Stamina < PlayerStats.TotalStamina)
        {
            PlayerStats.Stamina              += staminaToRegenerate;
            staminaBarImage.fillAmount       =  PlayerStats.Stamina;
            staminaBarShadowImage.fillAmount =  PlayerStats.Stamina;

            yield return new WaitForSeconds(regenerationSpeedFactor);
        }
    }
}