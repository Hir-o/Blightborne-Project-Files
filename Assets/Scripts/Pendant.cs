using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pendant : MonoBehaviour {

	[SerializeField] private Image clockImage;
	[SerializeField] private Animator animator;
	[SerializeField] private GameObject pendantImageMaterial;

	private float rechargeDelay;
	private float tempRechargeDelay;

	private void Start() 
	{
		pendantImageMaterial.GetComponent<Image>().material.SetFloat("_SuperGrayScale_Fade_1", 0f);
		pendantImageMaterial.GetComponent<Image>().material.SetFloat("_PremadeGradients_Fade_1", 0.095f);
	}

	public void InitializeReset(float pendantRechargeDelay)
	{
		rechargeDelay = pendantRechargeDelay;
		clockImage.fillAmount = 1f;
		StartCoroutine(ResetPendant());
	}
	
	private IEnumerator ResetPendant()
	{
		tempRechargeDelay = rechargeDelay;

		//animator.SetBool("isDisabled", true);
		DisableEffects();

		for (int i = 0; i < rechargeDelay; i++)
        {
			tempRechargeDelay--;

            clockImage.fillAmount = tempRechargeDelay / PlayerStats.PendantRechargeDelay;

            yield return new WaitForSeconds(1f);
        }

		PlayerStats.IsPendantReady = true;
		EnableEffects();
		//animator.SetBool("isDisabled", false);
	}

	private void DisableEffects()
	{
		pendantImageMaterial.GetComponent<Image>().material.SetFloat("_SuperGrayScale_Fade_1", 0.8f);
		pendantImageMaterial.GetComponent<Image>().material.SetFloat("_PremadeGradients_Fade_1", 0f);
	}

	private void EnableEffects()
	{
		pendantImageMaterial.GetComponent<Image>().material.SetFloat("_SuperGrayScale_Fade_1", 0f);
		pendantImageMaterial.GetComponent<Image>().material.SetFloat("_PremadeGradients_Fade_1", 0.095f);
	}
}
