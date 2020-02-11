using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour {
	
	private enum PlateState { Locked, Unlocked };
	private PlateState plateState = PlateState.Locked;

	private Animator animator;

	[SerializeField] private GameObject[] objectsToInteract;
	[SerializeField] private GameObject firstGameObject;

	[Header("If Puzzle Level")]
	[SerializeField] private bool isReverseGate = false;

	[Header("Maze Level Special Pressure Plate Code")]
	[SerializeField] private bool isSpecialPressurePlate;
	[SerializeField] private GameObject crate;
	[SerializeField] private GameObject crateOutline;
	[SerializeField] private float offsetY = .5f;

	private void Start() 
	{
		animator = GetComponent<Animator>();
		InvokeRepeating("CheckForCrate", 1f, 1f);
	}

	private void CheckForCrate() 
	{
		if (firstGameObject != null)
		{
			if (firstGameObject.CompareTag(Tag.CrateTag))
			{
				if (firstGameObject.activeSelf == false)
				{
					animator.SetBool("isBeingPushed", false);
					plateState = PlateState.Locked;
					AddObstacles();
					firstGameObject = null;
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.GetType() == typeof(CapsuleCollider2D))
		{
			if (firstGameObject == null)
				firstGameObject = other.gameObject;
			else
				return;

			if (other.gameObject.CompareTag(Tag.PlayerTag)
			|| other.gameObject.CompareTag(Tag.SmartEnemyTag))
			{
				animator.SetBool("isBeingPushed", true);
				plateState = PlateState.Unlocked;
				RemoveObstacles();
			}
		}

		if (other.gameObject.CompareTag(Tag.CrateTag))
		{
			if (firstGameObject == null)
				firstGameObject = other.gameObject;
			else
				return;

			animator.SetBool("isBeingPushed", true);
			plateState = PlateState.Unlocked;
			RemoveObstacles();

			if (isSpecialPressurePlate)
			{
				crateOutline.transform.position = new Vector2(transform.position.x, transform.position.y + offsetY);
				crate.GetComponent<Crate>().SetSpawnLocation(new Vector2(transform.position.x, transform.position.y + offsetY));
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		if (firstGameObject == null)
		{
			if (other.GetType() == typeof(CapsuleCollider2D))
			{
				if (other.gameObject.CompareTag(Tag.PlayerTag)
				|| other.gameObject.CompareTag(Tag.SmartEnemyTag))
				{
					firstGameObject = other.gameObject;
					animator.SetBool("isBeingPushed", true);
					plateState = PlateState.Unlocked;
					RemoveObstacles();
				}
			}

			if (other.gameObject.CompareTag(Tag.CrateTag))
			{
				firstGameObject = other.gameObject;
				animator.SetBool("isBeingPushed", true);
				plateState = PlateState.Unlocked;
				RemoveObstacles();
			}
		}
		
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.gameObject != firstGameObject)
		{
			return;
		}

		if (other.GetType() == typeof(CapsuleCollider2D))
		{
			if (other.gameObject.CompareTag(Tag.PlayerTag)
			|| other.gameObject.CompareTag(Tag.SmartEnemyTag))
			{
				animator.SetBool("isBeingPushed", false);
				plateState = PlateState.Locked;
				AddObstacles();
				firstGameObject = null;
			}
		}

		if (other.gameObject.CompareTag(Tag.CrateTag))
		{
			animator.SetBool("isBeingPushed", false);
			plateState = PlateState.Locked;
			AddObstacles();
			firstGameObject = null;
		}
	}

	private void RemoveObstacles()
	{
		foreach (GameObject obstacle in objectsToInteract)
		{
			if (isReverseGate == false)
			{
				if (obstacle.GetComponent<Animator>().GetBool("isLocked") == true)
				{
					obstacle.GetComponent<Animator>().SetBool("isLocked", false);
				}
			}
			else
			{
				if (obstacle.GetComponent<Animator>().GetBool("isLocked") == false)
				{
					obstacle.GetComponent<Animator>().SetBool("isLocked", true);
				}
			}
		}
	}

	private void AddObstacles()
	{
		foreach (GameObject obstacle in objectsToInteract)
		{
			if (isReverseGate == false)
			{
				if (obstacle.GetComponent<Animator>().GetBool("isLocked") == false)
				{
					obstacle.GetComponent<Animator>().SetBool("isLocked", true);
				}
			}
			else
			{
				if (obstacle.GetComponent<Animator>().GetBool("isLocked") == true)
				{
					obstacle.GetComponent<Animator>().SetBool("isLocked", false);
				}
			}
		}
	}

}
