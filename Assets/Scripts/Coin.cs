using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour 
{
	private Rigidbody2D rigidBody;
	private SpriteRenderer spriteRenderer;
	[SerializeField] private float throwSpeed = 2000f;
	//private Vector3 force; old code
	private Vector2 direction;

	private bool isCollected = false;

	[Header("Direction Throwing Variables")]
	[SerializeField] private float minXDir = -.3f;
	[SerializeField] private float maxXDir = .3f;
	[SerializeField] private float minYDir = .6f;
	[SerializeField] private float maxYDir = 1f;

	[Header("Throwable / Always Positive Values")]
	[SerializeField] private bool isThrowable = false;
	[SerializeField] private float minThrowDirection = .3f;
	[SerializeField] private float maxThrowDirection = .6f;
	public float playerScaleZ = 1f;
	[SerializeField] private float collectableTimer;
	[SerializeField] private int collectableLayer;
	[SerializeField] private float destroyTimer = 20f;

	private bool isCollectable = false;

	private Color tempColor;

	private void Awake() 
	{
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		tempColor = spriteRenderer.color;
	}

	private void Start() 
	{
		Physics2D.IgnoreCollision(GameManager.player.GetComponent<CapsuleCollider2D>(), GetComponent<Collider2D>());
		
		collectableLayer = LayerMask.NameToLayer("Collectable");

		Throw();
		Invoke(nameof(MakeCollectable), collectableTimer);
		Invoke(nameof(DestroyAfterTimeExpires), destroyTimer);
	}
	
	private void Throw()
	{
		// force = UnityEngine.Random.insideUnitCircle * 1.2f; old code
		// rigidBody.AddForce(force * throwSpeed); old code
	
		if (isThrowable)
		{
			playerScaleZ *= -1f;
			direction = new Vector2(Random.Range(playerScaleZ * minThrowDirection, playerScaleZ * maxThrowDirection), Random.Range(minYDir, maxYDir));
		}
		else
		{
			direction = new Vector2(Random.Range(minXDir, maxXDir), Random.Range(minYDir, maxYDir));
		}

		rigidBody.AddForce(direction * throwSpeed);
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		Collect(other);
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		Collect(other);
	}

	private void Collect(Collider2D other)
	{
		if (isCollectable == false) { return; }
		if (isCollected) { return; }

		if (other.GetComponent<BoxCollider2D>() == null) { return; }

		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			AudioController.Instance.CoinSFX();
			isCollected = true;
			PlayerStats.Coins++;
			EssentialObjects.UpdateCoinsStatic();
			other.gameObject.GetComponent<CoinSack>().CheckForCoinCapacity();
			Destroy(gameObject);
		}
	}

	private void MakeCollectable()
	{
		isCollectable = true;
		gameObject.layer = collectableLayer;
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag(Tag.LavaTag) || other.gameObject.CompareTag(Tag.EctoplasmTag))
			Destroy(gameObject);
	}

	private void OnCollisionStay2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag(Tag.LavaTag) || other.gameObject.CompareTag(Tag.EctoplasmTag))
			Destroy(gameObject);
	}

	private void DestroyAfterTimeExpires()
	{
		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		for (int i = 0; i < 15; i++)
		{
			tempColor.a = 0;
			spriteRenderer.color = tempColor;

			if (i <= 5)
				yield return new WaitForSeconds(.2f);
			else
				yield return new WaitForSeconds(.08f);

			tempColor.a = 255;
			spriteRenderer.color = tempColor;

			if (i <= 5)
				yield return new WaitForSeconds(.2f);
			else
				yield return new WaitForSeconds(.08f);
		}

		Destroy(gameObject);
	}

}
