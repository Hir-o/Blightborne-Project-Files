using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour {

	private BoxCollider2D boxCollider;
	private Animator animator;

	[Range(0f,1f)][SerializeField] private float startAnimationFrame;
	[Header("*NEEDS TO BE THE SAME AS THE ANIMATION NAME THE ANIMATOR CONTAINS")]
	[SerializeField] private string animationName;
	
	private void Awake() 
	{
		animator = GetComponent<Animator>();	
	}

	private void Start() 
	{
		boxCollider = GetComponent<BoxCollider2D>();
		animator.Play(animationName, 0, startAnimationFrame);
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (boxCollider.enabled == false) { return; }

		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			other.GetComponent<Player>().InstaKill();
		}

		if (other.gameObject.CompareTag(Tag.SmartEnemyTag))
		{
			other.GetComponent<EnemyMele>().InstaKill();
		}
	}
}
