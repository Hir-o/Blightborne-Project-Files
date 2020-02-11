using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWard : MonoBehaviour {

	[SerializeField] LayerMask playerMask;
	[SerializeField] private float attackRangeX = 5f;
	[SerializeField] private float attackRangeY = 5f;
	[SerializeField] private Transform attackPosition;
	
	private Collider2D[] colliders;
	private Player player;
	private Animator animator;

	[Header("Ward Type")]
	[SerializeField] private float shootReloadTimer = 4f;
	[SerializeField] private GameObject projectile;
	[SerializeField] private enum WardType { missile, directional, multiple}
	[SerializeField] private WardType wardType = WardType.multiple;
	
	[Header("If wardtype is multiple")]
	[SerializeField] private int projectileAmount = 3;

	private Vector3 playerFlatPosition;

	private GameObject projectile1;
	private GameObject projectile2;
	private GameObject projectile3;

	private void Start() 
	{
		animator = GetComponent<Animator>();
		InvokeRepeating("CheckForPlayer", .5f, .5f);
	}

	private void CheckForPlayer()
	{
		colliders = Physics2D.OverlapBoxAll(attackPosition.position, new Vector2(attackRangeX, attackRangeY), 0f, playerMask);

		if (colliders.Length == 0)
		{
			if (player != null)
			{
				DeselectTarget();
			}
			else
			{
				return;
			}
		}

		foreach (Collider2D coll in colliders)
		{
			if (coll.GetType() == typeof(CapsuleCollider2D))
			{
				if (player == null)
				SetTarget(coll);
			}
		}
	}

	private void SetTarget(Collider2D coll)
	{
		player = coll.gameObject.GetComponent<Player>();

		InvokeRepeating("Attack", 0f, shootReloadTimer);
	}

	private void DeselectTarget()
	{
		player = null;
		CancelInvoke("Attack");
	}

	private void Attack()
	{
		if (gameObject.activeSelf == false) { return; }

		animator.SetTrigger("attack");
	}

	public void InitializeAttack() // called from animation event
	{
		if (player == null) { return; }

		playerFlatPosition = new Vector3(player.transform.position.x, player.transform.position.y, 0f);

		if (wardType == WardType.multiple)
		{
			projectile1 = Instantiate(projectile, transform.position, Quaternion.identity);
			projectile1.transform.LookAt(playerFlatPosition);

			projectile2 = Instantiate(projectile, transform.position, Quaternion.identity);
			projectile2.transform.LookAt(playerFlatPosition);
			projectile2.transform.Rotate(45f, 0f, 0f);

			projectile3 = Instantiate(projectile, transform.position, Quaternion.identity);
			projectile3.transform.LookAt(playerFlatPosition);
			projectile3.transform.Rotate(-45f, 0f, 0f);
		}
		else if (wardType == WardType.directional)
		{
			projectile1 = Instantiate(projectile, transform.position, Quaternion.identity);
			projectile1.transform.LookAt(playerFlatPosition);
		}
		else if (wardType == WardType.missile)
		{
			projectile1 = Instantiate(projectile, transform.position, Quaternion.identity);
			projectile1.transform.LookAt(playerFlatPosition);
			projectile1.GetComponent<WardProjectile>().target = player.transform;
		}
	}

	private void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(attackPosition.position, new Vector3(attackRangeX, attackRangeY, 1f));
	}
	
}
