using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Animal : MonoBehaviour {

	private Animator animator;

	[SerializeField] private Transform pointB;
	[SerializeField] private float movementSpeed;
	[SerializeField] private float waitTime;
	[SerializeField] private float accuracy = .2f;

	private Vector3 npcVector;
	private Vector3 pointA;
	// used only for purpose of storing the current transform temporarily
	private Vector3 tempVector; 

	private bool isWalking;
	private bool isIdle;

	private Vector3 direction;

	private void Awake()
	{
		Assert.IsNotNull(pointB);
	}

	void Start () 
	{
		animator = GetComponent<Animator>();
		pointA = transform.position;
		npcVector = new Vector3(movementSpeed, 0f, 0f);
	}
	
	void LateUpdate () 
	{
		direction = pointB.position - transform.position;

		if(direction.magnitude > accuracy)
		{
			animator.SetBool("isWalking", true);
 			transform.Translate(
									direction.normalized * movementSpeed * Time.deltaTime,
									Space.World
								);
		}
		else if(animator.GetBool("isWalking") == true)
		{
			StartCoroutine("Wait", waitTime);	
		}   
	}

	private IEnumerator Wait(float waitTime)
	{
		animator.SetBool("isWalking", false);

		yield return new WaitForSeconds(waitTime);

		tempVector = pointB.position;
		pointB.position = pointA;
		pointA = tempVector;

		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

}
