using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class LevelDoorMaze : MonoBehaviour {

	[SerializeField] private Transform locationToTeleport;
	[SerializeField] private GameObject buttonSprite;
	private bool canActivate = false;
	
	private Player player;
	private EnemyFollowMele[] enemies;

	private Animator _animator;

	private void Start () { _animator = buttonSprite.GetComponent<Animator>(); }

	private void Update() 
    {
        if (canActivate == true)
        {
            if (InputControl.GetButtonDown("Interact"))
            {
				player.transform.position = locationToTeleport.position;
				ResetAllEnemies();
            }
        }
    }

	  private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			player = other.gameObject.GetComponent<Player>();
			buttonSprite.SetActive(true);
			canActivate = true;
		}
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		if (other.GetComponent<SurviveLevelChanger>()) { return; }
		if (player == null) { return; }
		
		if (_animator != null)
		{
			switch (KeyBindings.CurrentProfile)
			{
				case KeyBindings.Profile.Profile1:
					_animator.SetBool("E",    false);
					_animator.SetBool("Down", true);
					break;
				case KeyBindings.Profile.Profile2:
					_animator.SetBool("Down", false);
					_animator.SetBool("E",    true);
					break;
				case KeyBindings.Profile.Profile3:
					_animator.SetBool("E",    false);
					_animator.SetBool("Down", true);
					break;
			}
		}

		if (player.isDeactivated) { return; }

        if (other.CompareTag(Tag.PlayerTag))
        {
            if(player.GetComponent<Animator>().GetBool("isRolling")) { return; }
            if(player.GetComponent<Animator>().GetBool("isClimbing")) { return; }
            if(player.GetComponent<Animator>().GetBool("isSliding")) { return; }
            if(player.GetComponent<Animator>().GetBool("isJumping")) { return; }
            
            GameManager.Instance.peekDisabled = true;
        }
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag(Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
			canActivate = false;
			
			GameManager.Instance.peekDisabled = false;
        }
	}

	private void ResetAllEnemies()
	{
		enemies = FindObjectsOfType<EnemyFollowMele>();

		foreach (EnemyFollowMele enemy in enemies)
			enemy.Reset();
	}

}
