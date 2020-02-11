using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour {

	//public Material battleTrans;

	private SimpleBlit blit;

	private float cutOffValue = 1;

	private void Awake () 
	{
		blit = GetComponent<SimpleBlit>();

		blit.TransitionMaterial.SetFloat("_Cutoff", 1f);
		blit.TransitionMaterial.SetFloat("_Fade", 1f);

		//StartSwipeOut(); //todo remove
	}

	public void ShowBars ()
	{
		StopAllCoroutines();
		StartCoroutine(ShowBarsCoroutine());
	}

	public void HideBars ()
	{
		StopAllCoroutines();
		StartCoroutine(HideBarsCoroutine());
	}

	public void StartSwipeIn()
	{
//		PokiUnitySDK.Instance.gameplayStop();
//		PokiUnitySDK.Instance.gameLoadingStart();
		
		GameManager.IsTransitioning = true;
		StopAllCoroutines();
		StartCoroutine(SwipeIn());
	}

	public void StartSwipeOut()
	{
//		PokiUnitySDK.Instance.gameplayStart();
//		PokiUnitySDK.Instance.gameLoadingFinished();
		
		StopAllCoroutines();
		StartCoroutine(SwipeOut());
	}
	
	public IEnumerator SwipeIn() // 0.02f is the speed which the cutoff val of material increases
	{
		for (float i = 0f; i <= 1f; i+=.02f)
		{
			cutOffValue += .02f;
			cutOffValue =  Mathf.Clamp01(cutOffValue);

			blit.TransitionMaterial.SetFloat("_Cutoff", cutOffValue);
			blit.TransitionMaterial.SetFloat("_Fade",   1f);

			yield return new WaitForSecondsRealtime(.003f);
			blit.TransitionMaterial.SetFloat("_Cutoff", 1f);
		}
		
//		PokiUnitySDK.Instance.gameLoadingProgress(null);
	}

	public IEnumerator SwipeOut() // 0.02f is the speed which the cutoff val of material decreases
	{
//		if (PokiUnitySDK.Instance.adsBlocked() == false && PlayerStats.IsCommercialBreakInitialized == false)
//		{
//			AudioListener.volume = 0f;
//			
//			PokiUnitySDK.Instance.commercialBreakCallBack = commercialBreakComplete;
//			PokiUnitySDK.Instance.commercialBreak();
//
//			PlayerStats.IsCommercialBreakInitialized = true;
//			
//			yield return new WaitForSecondsRealtime(2f);
//		}
		
		yield return new WaitForSecondsRealtime(.1f);

		for (float i = 1f; i >= 0f; i-=.02f)
		{
			cutOffValue -= .02f;
			cutOffValue =  Mathf.Clamp01(cutOffValue);

			blit.TransitionMaterial.SetFloat("_Cutoff", cutOffValue);
			blit.TransitionMaterial.SetFloat("_Fade",   1f);

			yield return new WaitForSecondsRealtime(.003f);
			blit.TransitionMaterial.SetFloat("_Cutoff", 0f);
		}

		GameManager.IsTransitioning = false;
	}
	
	public IEnumerator ShowBarsCoroutine() // 0.02f is the speed which the cutoff val of material increases
	{
		for (float i = 0f; i <= .4f; i+=.02f)
		{
			cutOffValue += .02f;
			cutOffValue =  Mathf.Clamp01(cutOffValue);

			blit.TransitionMaterial.SetFloat("_Cutoff", cutOffValue);
			blit.TransitionMaterial.SetFloat("_Fade",   1f);

			yield return new WaitForSecondsRealtime(.003f);
			blit.TransitionMaterial.SetFloat("_Cutoff", .4f);
		}
	}

	public IEnumerator HideBarsCoroutine() // 0.02f is the speed which the cutoff val of material decreases
	{
		yield return new WaitForSecondsRealtime(.1f);

		for (float i = .4f; i >= 0f; i-=.02f)
		{
			cutOffValue -= .02f;
			cutOffValue =  Mathf.Clamp01(cutOffValue);

			blit.TransitionMaterial.SetFloat("_Cutoff", cutOffValue);
			blit.TransitionMaterial.SetFloat("_Fade",   1f);

			yield return new WaitForSecondsRealtime(.003f);
			blit.TransitionMaterial.SetFloat("_Cutoff", 0f);
		}

		GameManager.IsTransitioning = false;
	}
	
//	//Poki Commercial Break
//	public void commercialBreakComplete() 
//	{
//		AudioListener.volume = 1f;
//	}
}
