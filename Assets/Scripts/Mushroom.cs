using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour {

	private Animator animator;

	private void Start() 
	{
		animator = GetComponent<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			if (other.GetType() == typeof(CapsuleCollider2D))
			{
				other.gameObject.GetComponent<PlayerMushroomJump>().ThrowPlayer();
				animator.SetTrigger("play");
			}
		}
	}

}
