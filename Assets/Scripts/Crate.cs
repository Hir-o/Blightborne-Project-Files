using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {

	private Vector2 spawnLocation;
	private Animator animator;
	private AnimationClip[] clips;
	private Player player;

	private float delayTime;

	[SerializeField] private float respawnRate = 1f;
	[SerializeField] private LayerMask playerMask;
	[SerializeField] private float rangeX = 1f;
	[SerializeField] private float rangeY = .5f;
	[SerializeField] private Collider2D[] playerColliders;

	[Header("Lava Checking Variables")]
	[SerializeField] private float lavaChangeRangeX = .5f;
	[SerializeField] private float lavaChangeRangeY = .5f;
	[SerializeField] private LayerMask lavaMask;
	[SerializeField] private Collider2D[] lavaColliders;

	private Collider2D crateCollider, playerCollider;

	private void Start() 
	{
		animator = GetComponent<Animator>();
		spawnLocation = new Vector2(transform.position.x, transform.position.y);	
		crateCollider = GetComponent<Collider2D>();
	}

	private void Update() 
	{
		CheckForLava();

		if (PlayerStats.IsRolling == false)
		{
			Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameManager.player.GetComponent<CapsuleCollider2D>(), false);
			//player = null;
		}
	}

	private void CheckForPlayer()
	{
		playerColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(rangeX, rangeY), 0f, playerMask);

		foreach (Collider2D collider in playerColliders)
        {
			if (collider.gameObject.CompareTag(Tag.PlayerTag))
            {
				player = collider.gameObject.GetComponent<Player>();
				playerCollider = player.GetComponent<Collider2D>();
				Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player.GetComponent<CapsuleCollider2D>(), true);
			}
		}
	}

	private void CheckForLava()
	{
		lavaColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(lavaChangeRangeX, lavaChangeRangeY), 0f, lavaMask);

		foreach (Collider2D collider in lavaColliders)
            if (collider.gameObject.CompareTag(Tag.LavaTag))
				DisableCrate();
	}

	public void DisableCrate()
	{
		animator.SetTrigger("hit");

		clips = animator.runtimeAnimatorController.animationClips;
		delayTime = 0f;

		foreach (AnimationClip clip in clips)
			if (clip.name.Equals("hit"))
				delayTime = clip.length;

		Invoke("DisableGameObject", delayTime);
	}

	private void DisableGameObject()
	{
		gameObject.active = false;

		Invoke("OnCrateDestroyed", respawnRate);
	}

	private void OnCrateDestroyed()
	{
		transform.position = spawnLocation;
		gameObject.active = true;
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag(Tag.SmartEnemyTag))	
			Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		else if (other.gameObject.CompareTag(Tag.CollectableTag))
			Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
	}

	private void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(rangeX, rangeY , 1f));
	}

	public void SetSpawnLocation(Vector2 newSpawnLocation)
	{
		spawnLocation = newSpawnLocation;
	}
	
}
