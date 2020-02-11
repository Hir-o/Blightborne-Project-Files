using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	[SerializeField] private Transform pointB;
	[SerializeField] private float movementSpeed;
	[SerializeField] private float waitTime;
	[SerializeField] private float accuracy = .2f;

	private Vector3 npcVector;
	private Vector3 pointA;
	// used only for purpose of storing the current transform temporarily
	private Vector3 tempVector; 

	private bool isIdle;

	private Vector3 direction;

	[Header("Is the platform a one way only platform")]
	[SerializeField] private bool isOneWay = false;

	void Start () 
	{
		pointA = transform.position;
		npcVector = new Vector3(movementSpeed, 0f, 0f);
	}
	
	void LateUpdate () 
	{
		direction = pointB.position - transform.position;

		if(direction.magnitude > accuracy)
		{
 			transform.Translate(
									direction.normalized * movementSpeed * Time.deltaTime,
									Space.World
								);
		}
		else if (direction.magnitude <= accuracy && isIdle == false && isOneWay == false)
		{
			isIdle = true;
			StartCoroutine("Wait", waitTime);	
		}   
	}

	private IEnumerator Wait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		tempVector = pointB.position;
		pointB.position = pointA;
		pointA = tempVector;
		isIdle = false;
	}
	
	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			other.transform.SetParent(this.transform);
		}	
	}

	private void OnCollisionExit2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			other.transform.SetParent(null);
		}	
	}
}
