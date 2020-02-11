using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurviveLevelChanger : MonoBehaviour {

	[SerializeField] private bool isGameOverCollider = false;
	[Header("Applies for the collider attached to the moving platforms")]
	[SerializeField] private bool canIgnorePlayerCollison = false;

	[Header("Escape Level Code")]
	[SerializeField] private int damage = 50;
	[SerializeField] private float checkDuration = 0.1f;
	private bool canInflictDamage = true;

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (SceneManager.GetActiveScene().name == "Escape")
		{
			if (other.gameObject.CompareTag(Tag.PlayerTag))
			{
				if (other.GetComponent<Collider2D>().GetType() == typeof(CapsuleCollider2D))
				{
					if (canInflictDamage)
					{
						canInflictDamage = false;
						other.gameObject.GetComponent<Player>().HitByHazards(damage);
						StartCoroutine(ResetTimer());
					}
				}
			}
			else if (other.gameObject.CompareTag(Tag.SmartEnemyTag))
				Destroy(other.gameObject);
			}
		else
		{
			if (SceneManager.GetActiveScene().name == "Dungeon II-IV")
			{
				if (PlayerStats.CheckpointPriority == 3)
				{
					if (other.gameObject.CompareTag(Tag.PlayerTag))
					{
						if (other.GetType() == typeof(CapsuleCollider2D))
						{
							if (canIgnorePlayerCollison == true)
							{
								Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), false);
								return;
							}
						}
					}
				}
			}

			if (other.gameObject.CompareTag(Tag.PlayerTag))
			{
				if (other.GetType() == typeof(CapsuleCollider2D))
				{
					if (isGameOverCollider == true)
					{
						//GameManager.player.GetComponent<Animator>().SetTrigger("hasDied");
						GameManager.player.isDeactivated = true;
						GameManager.player.GetComponent<CapsuleCollider2D>().enabled = false;
						GameManager.player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
						GameManager.player.GetComponent<PlayerCheckpoint>().ResetPlayerToCheckpoint();
					}
				}
			}
			else if (other.gameObject.CompareTag(Tag.SmartEnemyTag))
			{
				if (SceneManager.GetActiveScene().name == "Dungeon II-IV") { return; }
				if (other.GetType() == typeof(CapsuleCollider2D))
					Destroy(other.gameObject);
			}
		}
	}

	private IEnumerator ResetTimer()
	{
		yield return new WaitForSeconds(checkDuration);
		
		canInflictDamage = true;
	}
}
