using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour {

	[SerializeField] private Animator canvasAnimator;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			GameManager.IsMenuDisabled = true;
			canvasAnimator.SetBool("endGame", true);
			GameManager.player.isDeactivated = true;
			LavaRising.StopRising = true;
			AudioController.Instance.StopAllMusic();
			Invoke(nameof(TeleportBack), 25f);
		}
	}

	private void TeleportBack()
	{
		PlayerStats.IsCommercialBreakInitialized = false;
		
		GameManager.Instance.ReloadHouse();
		GameManager.IsMenuDisabled = false;
	}
	
}
