using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAbilityUnlocker : MonoBehaviour {

	private HUD hud;
	private Collider2D collider;

	private bool isAbilityEnabled = false;

	private void Start() 
	{
		collider = GetComponent<Collider2D>();
		hud = GameManager.hud;	
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (isAbilityEnabled == true) { return; }

		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			if (other.GetComponent<Collider2D>().GetType() == typeof(CapsuleCollider2D))
			{
				isAbilityEnabled = true;
				//PlayerStats.EnableAbility();
				GameManager.hud.GetComponent<TutorialHUD>().EnableAbilityPanel();
				collider.enabled = false;
			}
		}	
	}

}
