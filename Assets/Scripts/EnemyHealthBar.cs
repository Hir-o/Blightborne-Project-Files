using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour {

	private Enemy enemy;
	private EnemyBat enemyBat;
	private EnemyMele enemyMele;

	[SerializeField] private Animator animator;

	[SerializeField] private Transform healthBar;
	private float enemyMaxHealth;

	public enum EnemyType {Enemy, EnemyBat, SmartEnemy, None}
	public EnemyType enemyType = EnemyType.None;

	private bool isCoroutineRunning = false;

	private void Start() 
	{
		if (enemyType == EnemyType.Enemy)
		{
			enemy = GetComponent<Enemy>();
			enemyMaxHealth = enemy.GetHealth();
			healthBar.localScale = new Vector3((enemy.GetHealth() / enemyMaxHealth), healthBar.localScale.y, healthBar.localScale.z);
		}
		else if (enemyType == EnemyType.EnemyBat)
		{
			enemyBat = GetComponent<EnemyBat>();
			
			if (enemyBat.isBoss)
			{
				enemyMaxHealth = enemyBat.GetBossHealth();
				healthBar.localScale = new Vector3((enemyBat.GetBossHealth() / enemyMaxHealth), healthBar.localScale.y, healthBar.localScale.z);
			}
		}
		else if (enemyType == EnemyType.SmartEnemy)
		{
			enemyMele = GetComponent<EnemyMele>();
			enemyMaxHealth = enemyMele.GetHealth();
			healthBar.localScale = new Vector3((enemyMele.GetHealth() / enemyMaxHealth), healthBar.localScale.y, healthBar.localScale.z);
		}
	}

	public void UpdateHealthBar() // called "hit" from animation event
	{
		if (enemyType == EnemyType.Enemy)
		{
			ShowHealthBar();
			StartCoroutine(DecreaseHealth(enemy.GetHealth()));
		}
		else if (enemyType == EnemyType.EnemyBat)
		{	
			if (enemyBat.isBoss)
			{
				ShowHealthBar();
				StartCoroutine(DecreaseHealth(enemyBat.GetBossHealth()));
			}
		}
		else if (enemyType == EnemyType.SmartEnemy)
		{
			ShowHealthBar();
			StartCoroutine(DecreaseHealth(enemyMele.GetHealth()));
		}
	}

	public void ShowHealthBar()
	{
		animator.SetTrigger("fadeOut");
	}

	private IEnumerator DecreaseHealth (int health)
	{
		yield return new WaitForSeconds(.05f);
		healthBar.localScale = new Vector3((health / enemyMaxHealth), healthBar.localScale.y, healthBar.localScale.z);
	}
}
