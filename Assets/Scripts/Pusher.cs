using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour {

	private Animator animator;
	[SerializeField] private float boxSizeX = 1f;
	[SerializeField] private float boxSizeY = 1f;
	[SerializeField] private LayerMask playerMask;
	[SerializeField] private Transform checkLocation;

	private Collider2D[] colliders;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void Update() 
	{
		CheckForPlayer();
	}

	private void CheckForPlayer()
	{
		colliders = Physics2D.OverlapBoxAll(checkLocation.position, new Vector2(boxSizeX, boxSizeY), playerMask);

		foreach(Collider2D collider in colliders)
        {
			if (collider.gameObject.CompareTag(Tag.PlayerTag))
			{
				animator.SetBool("isPush", true);
			}
		}
	}

	private void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(checkLocation.position, new Vector3(boxSizeX, boxSizeY, 1));
	}

}
