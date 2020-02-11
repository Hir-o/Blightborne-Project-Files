using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreach : MonoBehaviour {
	
	[SerializeField] private GameObject breach;
	[SerializeField] private Transform[] spawnLocations;
	[SerializeField] private LayerMask playerMask;

	[Header("Enemies to be spawned")]
	[SerializeField] private GameObject[] enemyTypes;
	[SerializeField] private int minEnemies, maxEnemies;
	[SerializeField] private float enemySpawningDuration;
	[SerializeField] private Vector3 spawnOffset;

	private enum BreachState {Hidden, Explored};
	private BreachState breachState = BreachState.Hidden;

	private Collider2D[] colliders;	
	private int enemyNumber;
	private IEnumerator spawningEnemiesCoroutine;
	private Animator animator;
	private int randomNumberGenerated;
	private Player player;
	private EnemyFollowMele enemyMele;
	private Collider2D collider;

	private void Start() 
	{
		transform.position = spawnLocations[Random.Range(0, spawnLocations.Length)].position;
		animator = GetComponent<Animator>();
		collider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (breachState == BreachState.Explored) { return; }

		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			animator.enabled = true;
			breach.SetActive(true);
			player = other.gameObject.GetComponent<Player>();
			collider.enabled = false;
		}
	}

	public void SpawnEnemies() //called by animation event
	{
		
		enemyNumber = Random.Range(minEnemies, maxEnemies);
		AudioController.Instance.WallBreachSFX();

		spawningEnemiesCoroutine = SpawnEnemy(enemySpawningDuration);
		StartCoroutine(spawningEnemiesCoroutine);
	}

	private IEnumerator SpawnEnemy(float waitTime)
	{
		while (true)
		{
			enemyNumber--;

			if (enemyNumber <= 0)
			{
				StopCoroutine(spawningEnemiesCoroutine);
			}

			if (enemyTypes.Length == 3)
			{
				randomNumberGenerated = Random.Range(0,10);
			}
			else if (enemyTypes.Length == 2)
			{
				randomNumberGenerated = Random.Range(0,8);
			}
			else if (enemyTypes.Length == 1)
			{
				randomNumberGenerated = Random.Range(0,5);
			}

			if (randomNumberGenerated <= 5)
			{
				enemyMele = Instantiate(enemyTypes[0], transform.position + spawnOffset, Quaternion.identity).GetComponent<EnemyFollowMele>();
				enemyMele.IncreaseAttackRange();
			}
			else if (randomNumberGenerated > 5 && randomNumberGenerated <= 8)
			{
				enemyMele = Instantiate(enemyTypes[1], transform.position + spawnOffset, Quaternion.identity).GetComponent<EnemyFollowMele>();
				enemyMele.IncreaseAttackRange();
			}
			else
			{
				enemyMele = Instantiate(enemyTypes[2], transform.position + spawnOffset, Quaternion.identity).GetComponent<EnemyFollowMele>();
				enemyMele.IncreaseAttackRange();
			}

			yield return new WaitForSeconds(waitTime);
		}
	}
}
