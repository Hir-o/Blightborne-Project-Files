using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeJointsCollisionChecker : MonoBehaviour {

	private BreakJoint joint;
	private Player player;

	private Collider2D[] colliders;

	[SerializeField] private LayerMask hazardLayer;
	[SerializeField] private float checkTimer = 1f;
	[SerializeField] private float cancelCheckTimer = 10f;
	[SerializeField] private float rangeX = 1f;
	[SerializeField] private float rangeY = 1f;

	private void Start() 
	{
		joint = GetComponentInParent<BreakJoint>();
		player = GameManager.player;

//		Physics2D.IgnoreCollision(joint.GetComponent<Collider2D>(), GameManager.player.GetComponent<CircleCollider2D>(), true);
//		Physics2D.IgnoreCollision(joint.GetComponent<Collider2D>(), GameManager.player.GetComponent<BoxCollider2D>(), true);
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag(Tag.SmartEnemyTag) )
		{
			if (joint.canEnemyCollide == false)
			{
				Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), other.gameObject.GetComponent<CapsuleCollider2D>(), true);
			}	
		}
	}

	private void CheckForHazard()
	{
		colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(rangeX, rangeY), 0f, hazardLayer);

		foreach (Collider2D coll in colliders)
		{
			Destroy(gameObject);
		}
	}

	private void CancelInvokes()
	{
		CancelInvoke(nameof(CheckForHazard));
	}

	public void HandleHazardCollision()
	{
		InvokeRepeating(nameof(CheckForHazard), checkTimer, checkTimer);
		Invoke(nameof(CancelInvokes), cancelCheckTimer);
	}

	private void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(rangeX, rangeY, 1f));
	}
}
