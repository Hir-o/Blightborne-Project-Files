using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMushroomJump : MonoBehaviour {

	[SerializeField] private Vector2 mushroomThrowForce = new Vector2(0f, 20f);

	private Rigidbody2D rigidbody;
	private Animator animator;
	private Player player;
	private PlayerParticles playerParticles;

	private void Start() 
	{
		rigidbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		player = GetComponent<Player>();
		playerParticles = GetComponent<PlayerParticles>();
	}

	public void ThrowPlayer()
	{
		player.StopAllAnimations();
		playerParticles.PlayJumpParticles();
		animator.SetBool("isJumping", true);
		rigidbody.velocity = mushroomThrowForce;

		player.AllowRolling();
		
		AudioController.Instance.MushroomSFX();
	}
}
