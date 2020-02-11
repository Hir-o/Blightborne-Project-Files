using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardProjectile : MonoBehaviour {

	[SerializeField] private int damage = 10;
	public float destroyTimer = 5f;
	public float projectileSpeed = 2f;

	public bool isMissile, hasHit = false;
	public Transform target;
	private Animator animator;

	private void Start() 
	{
		animator = GetComponent<Animator>();
		Invoke("DestroyProjectile", destroyTimer);
	}

	private void FixedUpdate() 
	{
		if (hasHit == true) { return; }
		if (target == null)
			transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);	
		else
			transform.position = Vector2.MoveTowards(transform.position, target.position, projectileSpeed * Time.deltaTime);
	}

	private void DestroyProjectile()
	{
		hasHit = true;
		animator.SetTrigger("destroy");
	}
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.GetComponent<Player>() && other.GetComponent<Collider2D>().GetType() == typeof (CapsuleCollider2D))
			if (other.GetComponent<Animator>().GetBool("isRolling"))
				return;
				
		if (hasHit) { return; }

		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			if (other.GetType() == typeof(CapsuleCollider2D))
			{
				hasHit = true;
				other.gameObject.GetComponent<Player>().HitByMeleEnemy(damage);
				animator.SetTrigger("destroy");
			}
		}
	}

	public void DestroyParticleFromAnimation() // called from animation event
	{
		Destroy(gameObject);
	}
	
}
