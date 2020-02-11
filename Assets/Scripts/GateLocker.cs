using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateLocker : MonoBehaviour {

	[SerializeField] private float sizeX, sizeY, checkCollisionTimer;
	[SerializeField] private Animator[] animators;
	[SerializeField] private Collider2D[] colliders;
	[SerializeField] private LayerMask playerMask;
	[SerializeField] private bool lockGates = false;

	private void Start() 
	{
		InvokeRepeating(nameof(CheckForPlayer), checkCollisionTimer, checkCollisionTimer);
	}

	private void CheckForPlayer()
	{
		colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(sizeX, sizeY), 0f, playerMask);

		foreach (Collider2D coll in colliders)
		{
			if (coll.GetType() == typeof(CapsuleCollider2D))
			{
				lockGates = true;
			}
		}

		if (lockGates == true)
		{
			LockAllGates();
		}
	}

	private void LockAllGates()
	{
		foreach (Animator anim in animators)
		{
			anim.SetBool("isLocked", true);
		}
		
		CancelInvoke();
		this.gameObject.SetActive(false);
		this.enabled = false;
	}

	void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeX, sizeY, 1));
    }

}
