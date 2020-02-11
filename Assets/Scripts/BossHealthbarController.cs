using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbarController : MonoBehaviour {

	[SerializeField] private Image barImage;
	[SerializeField] private Image damagedBarImage;

	[Header("Fade Timer")]
	[SerializeField] private float damagedHealthFadeTimerMax = 1f; // visible timer before starts fading away
	[SerializeField] private float damagedHealthFadeTimer;

	[Header("Fade reduce amount")]
	[SerializeField] private float fadeAmount = 5f;
	
	private Color damagedColor;

	private void Start() 
	{
		barImage.fillAmount = 1f;	

		damagedColor = damagedBarImage.color;
		damagedColor.a = 0f;
		damagedBarImage.color = damagedColor;
	}

	private void Update() 
	{
		if (damagedColor.a > 0f)
		{
			damagedHealthFadeTimer -= Time.deltaTime;

			if (damagedHealthFadeTimer < 0f)
			{
				damagedColor.a -= fadeAmount * Time.deltaTime;
				damagedBarImage.color = damagedColor;
			}
		}
	}

	public void UpdateBossHealth(float healthNormalized)
	{
		if (damagedColor.a <= 0f)
		{
			damagedBarImage.fillAmount = barImage.fillAmount;
		}

		barImage.fillAmount = healthNormalized;

		damagedColor.a = 1f;
		damagedBarImage.color = damagedColor;
		damagedHealthFadeTimer = damagedHealthFadeTimerMax;
	}
}
