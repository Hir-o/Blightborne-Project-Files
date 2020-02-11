using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpikes : MonoBehaviour {

	private Rigidbody2D rigidBody;
	private Animator animator;
	private BoxCollider2D boxCollider;
	private PolygonCollider2D polygonCollider;

	[SerializeField] private int damage = 50;
	[SerializeField] LayerMask desiredLayer;

	private bool canKillPlayer = true;

	private void Start() 
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		boxCollider = GetComponent<BoxCollider2D>();
		polygonCollider = GetComponent<PolygonCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (canKillPlayer == false) { return; }

		if (other.gameObject.CompareTag(Tag.PlayerTag) || other.gameObject.CompareTag(Tag.SmartEnemyTag))
		{
			rigidBody.bodyType = RigidbodyType2D.Dynamic;
			boxCollider.enabled = false;
		}	
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (canKillPlayer)
		{
			if (other.gameObject.CompareTag(Tag.PlayerTag))
			{
				rigidBody.bodyType = RigidbodyType2D.Static;
				other.gameObject.GetComponent<Player>().HitByHazards(damage);
				animator.SetTrigger("destroy");
			}

			if (other.gameObject.CompareTag(Tag.SmartEnemyTag))
			{
				rigidBody.bodyType = RigidbodyType2D.Static;
				other.gameObject.GetComponent<EnemyMele>().InstaKill();
				animator.SetTrigger("destroy");
				canKillPlayer = false;
			}

			if (other.gameObject.CompareTag(Tag.EnemyTag))
			{
				rigidBody.bodyType = RigidbodyType2D.Static;
				other.gameObject.GetComponent<Enemy>().InstaKill();
				animator.SetTrigger("destroy");
				canKillPlayer = false;
			}
		}

		if (other.gameObject.CompareTag(Tag.GroundTag) || other.gameObject.CompareTag(Tag.BridgeTag) || other.gameObject.CompareTag(Tag.CrateTag))
		{
			canKillPlayer = false;
			animator.SetTrigger("destroy");
		}
	}

	public void DestroySpikes() // called from animation event
	{
		Destroy(gameObject);
	}
}
