using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private int health = 10;
    [SerializeField] private int damage = 1;
    [SerializeField] private Vector2 throwBackForce = new Vector2(0f, 5f);

    private Rigidbody2D rigidbody;
    private Animator animator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D groundCollider;

    [Header("Coins")]
    [SerializeField] private GameObject[] coins;
    public int minCoins = 1;
    public int maxCoins = 3;
    private int coinDropFactor;
    private int randomCoinDrop;

    private float delayTime;
    private AnimationClip[] clips;

    private EnemyHealthBar healthBar;

    [Header("Ring Health Steal Amount (%)")]
    [SerializeField] private int healthReturnedPercentage = 1;

    [Header("Blood Stain & VFX")]
    [SerializeField] private GameObject bloodStain;

    [SerializeField] private GameObject bloodVFX;
    
    void Start () 
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        groundCollider = GetComponent<BoxCollider2D>();
        coinDropFactor = PlayerStats.DropRate;
        healthBar = GetComponent<EnemyHealthBar>();
    }

	void LateUpdate () 
    {
        ProcessMovement();
	}

    private void ProcessMovement()
    {
        if(IsFacingRight())
            rigidbody.velocity = new Vector2(moveSpeed, rigidbody.velocity.y);
        else
            rigidbody.velocity = new Vector2(-moveSpeed, rigidbody.velocity.y);
    }

    private bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (groundCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    public void TakeDamage(int damageTaken)
    {
        AudioController.Instance.EnemyHitSFX();
        animator.SetTrigger("hit");
        
        Instantiate(bloodStain, transform.position, Quaternion.identity);
        Instantiate(bloodVFX, transform.position, Quaternion.identity);

        if (health < damageTaken)
            health -= health;
        else
            health -= damageTaken;
        
        if (health <= 0)
        {
            GameManager.Instance.HitStopEffect();
            
            clips = animator.runtimeAnimatorController.animationClips;
            delayTime = 0f;
            randomCoinDrop = Random.Range(minCoins, maxCoins * coinDropFactor);

            for (int i = 0; i < randomCoinDrop; i++)
				Instantiate(coins[Random.Range(0, coins.Length)], transform.position, Quaternion.identity);

            foreach (AnimationClip clip in clips)
                if(clip.name.Equals("Enemy_Hit"))
                    delayTime = clip.length;

            GameManager.rippleEffect.SetNewRipplePosition(GameManager.mainCamera.WorldToScreenPoint(transform.position));
            Destroy(gameObject, delayTime);
        }

        healthBar.UpdateHealthBar();
    }

    public void InstaKill()
    {
        AudioController.Instance.EnemyHitSFX();
        animator.SetTrigger("hit");

        health -= health;

        if (health <= 0)
        {
            //GameManager.Instance.SlowMotion();
            clips = animator.runtimeAnimatorController.animationClips;
            delayTime = 0f;
            randomCoinDrop = Random.Range(minCoins, maxCoins * coinDropFactor);

            for (int i = 0; i < randomCoinDrop; i++)
				Instantiate(coins[Random.Range(0, coins.Length)], transform.position, Quaternion.identity);

            foreach (AnimationClip clip in clips)
                if(clip.name.Equals("Enemy_Hit"))
                    delayTime = clip.length;

            GameManager.rippleEffect.SetNewRipplePosition(GameManager.mainCamera.WorldToScreenPoint(transform.position));
            Instantiate(bloodStain, transform.position, Quaternion.identity);
            Instantiate(bloodVFX, transform.position, Quaternion.identity);
            Destroy(gameObject, delayTime);
        }

        healthBar.UpdateHealthBar();
    }

    #region getters

    public int Damage() 
    {
        return Mathf.RoundToInt(damage - (damage * (PlayerStats.Armor * PlayerStats.ArmorMultiplier) / 100)); 
    }

    public int GetHealth()
    {
        return health;
    }

    #endregion
}
