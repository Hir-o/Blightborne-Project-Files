using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private static Image healthBarImage;
    private static Image healthBarShadowImage;

    private static HealthBar instance;

    [Header("Health Shadow Levers")]
    [SerializeField] float shadowReduceFactor = .005f;
    [SerializeField] float startShadowReduceTime = 1.5f;
    [SerializeField] float shadowReduceSpeedFactor = .0f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        healthBarImage = GameObject.Find("Health Bar").GetComponent<Image>();
        healthBarShadowImage = GameObject.Find("Health Bar Shadow").GetComponent<Image>();

        healthBarImage.fillAmount = PlayerStats.Health / PlayerStats.TotalHealth;
    }

    public static void ResetHealthBarStatus()
    {
        healthBarImage.fillAmount = (PlayerStats.Health / PlayerStats.TotalHealth);
        healthBarShadowImage.fillAmount = healthBarImage.fillAmount;
    }

    public static void UpdateHealthBarStatus()
    {
        healthBarImage.fillAmount = (PlayerStats.Health / PlayerStats.TotalHealth);

        instance.StartCoroutine("HealthShadow");
    }

    IEnumerator HealthShadow()
    {
        yield return new WaitForSeconds(startShadowReduceTime);

        while (healthBarShadowImage.fillAmount > healthBarImage.fillAmount)
        {
            healthBarShadowImage.fillAmount -= shadowReduceFactor;

            yield return new WaitForSeconds(shadowReduceSpeedFactor);
        }
    }
}
