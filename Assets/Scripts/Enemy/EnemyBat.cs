using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBat : MonoBehaviour
{
    [SerializeField] private int     damage               = 5;
    [SerializeField] private Vector2 throwBackForce       = new Vector2(5f, 1f);
    [SerializeField] private float   throwBackForceTime   = .4f;
    [SerializeField] private bool    isSpecialTreasuerBat = false;

    private Vector3                 tempPosition;
    private Transform               playerPosition;
    private Rigidbody2D             rigidbody;
    private Animator                animator;
    private Player                  player;
    private CircleCollider2D        circleCollider;
    private Arrow                   arrow;
    private Fragment                fragment;
    private BossHealthbarController bossHealthbarController;
    private AnimationClip[]         clips;
    private EnemyFollow             enemyFollow;

    private bool canHit = true;

    [Header("Coins")]
    [SerializeField]
    private GameObject[] coins;

    public  int minCoins = 1;
    public  int maxCoins = 3;
    private int coinDropFactor;
    private int randomCoinDrop;

    [Header("Bat Boss Code")]
    public bool isBoss = false;

    [SerializeField] private int   bossHealth = 200;
    private                  int   maxBossHealth;
    private                  float healthChecker;

    public enum BossType
    {
        Ice,
        Fire,
        None
    }

    public BossType bossType = BossType.None;

    [Header("Bat Boss Projectile Code")]
    [SerializeField]
    private Transform projectileSpawnPosition;

    private                  GameObject tempProjectile;
    [SerializeField] private float      startShootingTimer = 2f;
    [SerializeField] private float      shootReloadTimer   = 6f;
    [SerializeField] private GameObject projectile;
    public                   GameObject fireShield;

    public enum AttackType
    {
        Directional,
        Threefold
    }

    public                   AttackType attackType                     = AttackType.Directional;
    [SerializeField] private float      startingRotationAngleThreeFold = 45f;
    private                  float      tempRotationAngle;

    [Header("Boss Bullet Spread")]
    [SerializeField]
    private float threeFoldAttackSpread = 30f;

    private Vector3 playerFlatPosition;

    [Header("Gates to Unlock After Death")]
    [SerializeField]
    private Animator[] gateAnimators;

    [Header("Ring Health Steal Amount (%)")]
    [SerializeField]
    private int healthReturnedPercentage = 1;

    [Header("Blood Stain & VFX")]
    [SerializeField]
    private GameObject bloodStain;

    [SerializeField] private GameObject bloodVFX;

    private void Awake ()
    {
        rigidbody      = GetComponent<Rigidbody2D>();
        animator       = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        enemyFollow    = GetComponent<EnemyFollow>();
        coinDropFactor = PlayerStats.DropRate;
        maxBossHealth  = bossHealth;

        if (isBoss) bossHealthbarController = FindObjectOfType<BossHealthbarController>();
    }

    private void Start ()
    {
        player             = GameManager.player;
        playerPosition     = player.GetComponent<Transform>();
        tempPosition       = transform.position;
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.ProjectileTag))
        {
            Instantiate(bloodStain, transform.position, Quaternion.identity);
            Instantiate(bloodVFX,   transform.position, Quaternion.identity);

            AudioController.Instance.EnemyHitSFX();

            arrow = collision.gameObject.GetComponent<Arrow>();
            arrow.GetComponent<Animator>().SetBool("hasHitEnemy", true);

            randomCoinDrop = Random.Range(minCoins, maxCoins * coinDropFactor);

            if (isBoss == false)
            {
                for (int i = 0; i < randomCoinDrop; i++)
                    Instantiate(coins[Random.Range(0, coins.Length)], transform.position, Quaternion.identity);

                if (arrow.arrowType != Arrow.ArrowType.Power && arrow.arrowType != Arrow.ArrowType.Special)
                    Destroy(arrow.gameObject);

                animator.SetTrigger("hit");
                circleCollider.enabled              = false;
                GetComponent<EnemyFollow>().isAlive = false;
            }

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

            float delayTime = 0f;

            foreach (AnimationClip clip in clips)
                if (clip.name.Equals("Hit"))
                    delayTime = clip.length;

            if (isBoss == false)
            {
                if (PlayerStats.IsRingPurchased) PlayerStats.AddHealth(healthReturnedPercentage);

                GameManager.rippleEffect.SetNewRipplePosition(GameManager
                                                              .mainCamera.WorldToScreenPoint(transform.position));
                GameManager.Instance.HitStopEffect();
                Destroy(gameObject, delayTime);
            }
            else
            {
                if (fireShield.activeSelf) { return; }

                if (bossType == BossType.Ice && arrow.arrowType == Arrow.ArrowType.Ice)
                {
                    Destroy(arrow.gameObject);
                    return;
                }

                if (bossType == BossType.Fire && arrow.arrowType == Arrow.ArrowType.Fire)
                {
                    Destroy(arrow.gameObject);
                    return;
                }

                if (bossType == BossType.Fire && arrow.arrowType == Arrow.ArrowType.Ice)
                {
                    if (bossHealth < arrow.GetDamage() * 2)
                        bossHealth -= bossHealth;
                    else
                        bossHealth -= arrow.GetDamage() * 2;
                }
                else if (bossType == BossType.Ice && arrow.arrowType == Arrow.ArrowType.Fire)
                {
                    if (bossHealth < arrow.GetDamage() * 2)
                        bossHealth -= bossHealth;
                    else
                        bossHealth -= arrow.GetDamage() * 2;
                }
                else
                {
                    if (bossHealth < arrow.GetDamage())
                        bossHealth -= bossHealth;
                    else
                        bossHealth -= arrow.GetDamage();
                }

                animator.SetTrigger("hit");
                healthChecker = (float) bossHealth / (float) maxBossHealth;

                bossHealthbarController.UpdateBossHealth(healthChecker);

                if (healthChecker <= .5 && BossFightController.objectCount == 1)
                {
                    BossFightController.EnableObstacle();
                    attackType = AttackType.Threefold;
                    enemyFollow.ChangePhase();
                }
                else if (healthChecker <= .75 && BossFightController.objectCount == 0)
                {
                    BossFightController.EnableObstacle();
                    InitializeProjectileAttack();
                    enemyFollow.ChangePhase();
                }

                if (bossHealth <= 0)
                {
                    GameManager.Instance.HitStopEffect();
                    AudioController.Instance.StopAllMusic();

                    for (int i = 0; i < randomCoinDrop; i++)
                        Instantiate(coins[Random.Range(0, coins.Length)], transform.position, Quaternion.identity);

                    circleCollider.enabled              = false;
                    GetComponent<EnemyFollow>().isAlive = false;

                    foreach (Animator anim in gateAnimators) anim.SetBool("isLocked", false);

                    bossHealthbarController.gameObject.SetActive(false);

                    if (PlayerStats.IsRingPurchased) PlayerStats.AddHealth(healthReturnedPercentage);

                    GameManager.rippleEffect.SetNewRipplePosition(GameManager
                                                                  .mainCamera.WorldToScreenPoint(transform.position));
                    Destroy(gameObject, delayTime);
                    BossFightController.DisableObstacles();
                }

                Destroy(arrow.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag(Tag.ProjectileFragmentTag))
        {
            if (isBoss) { return; }

            GameManager.Instance.HitStopEffect();
            fragment = collision.gameObject.GetComponent<Fragment>();
            fragment.Explode();

            randomCoinDrop = Random.Range(minCoins, maxCoins * coinDropFactor);

            for (int i = 0; i < randomCoinDrop; i++)
                Instantiate(coins[Random.Range(0, coins.Length)], transform.position, Quaternion.identity);

            animator.SetTrigger("hit");
            circleCollider.enabled              = false;
            GetComponent<EnemyFollow>().isAlive = false;

            clips = animator.runtimeAnimatorController.animationClips;
            float delayTime = 0f;

            foreach (AnimationClip clip in clips)
                if (clip.name.Equals("Hit"))
                    delayTime = clip.length;

            if (PlayerStats.IsRingPurchased) PlayerStats.AddHealth(healthReturnedPercentage);

            GameManager.rippleEffect.SetNewRipplePosition(GameManager
                                                          .mainCamera.WorldToScreenPoint(transform.position));
            Destroy(gameObject, delayTime);
        }

        if (canHit == false) { return; }

        if (collision.gameObject.CompareTag(Tag.PlayerTag))
        {
            canHit               = false;

            if (player.GetIsAlive())
            {
                player.HitByBat(damage, isSpecialTreasuerBat);
                StartCoroutine(ThrowBackBat(true));
            }
        }
    }

    public void InstaKill ()
    {
        GameManager.Instance.HitStopEffect();
        animator.SetTrigger("hit");
        circleCollider.enabled              = false;
        GetComponent<EnemyFollow>().isAlive = false;

        AnimationClip[] clips     = animator.runtimeAnimatorController.animationClips;
        float           delayTime = 0f;

        foreach (AnimationClip clip in clips)
            if (clip.name.Equals("Hit"))
                delayTime = clip.length;

        GameManager.rippleEffect.SetNewRipplePosition(GameManager.mainCamera.WorldToScreenPoint(transform.position));
        Instantiate(bloodStain, transform.position, Quaternion.identity);
        Instantiate(bloodVFX,   transform.position, Quaternion.identity);
        Destroy(gameObject, delayTime);
    }
    
    private IEnumerator ThrowBackBat (bool throwBackBat)
    {
        enemyFollow.isMovingAwayFromPlayer = true;
        
        yield return new WaitForSeconds(throwBackForceTime);

        enemyFollow.isMovingAwayFromPlayer = false;
        canHit = true;
    }

    public void InitializeProjectileAttack () { InvokeRepeating(nameof(Attack), startShootingTimer, shootReloadTimer); }

    public void StopProjectileAttack () { CancelInvoke(); }

    private void Attack ()
    {
        if (player == null) { return; }

        playerFlatPosition = new Vector3(player.transform.position.x, player.transform.position.y, 0f);

        if (attackType == AttackType.Directional)
        {
            tempProjectile = Instantiate(projectile, projectileSpawnPosition.position, Quaternion.identity);
            tempProjectile.transform.LookAt(playerFlatPosition);
        }
        else if (attackType == AttackType.Threefold)
        {
            tempRotationAngle = startingRotationAngleThreeFold;
            for (int i = 0; i < 3; i++)
            {
                tempProjectile = Instantiate(projectile, projectileSpawnPosition.position, Quaternion.identity);
                tempProjectile.transform.LookAt(playerFlatPosition);
                tempProjectile.transform.Rotate(tempRotationAngle, 0f, 0f);
                tempRotationAngle -= threeFoldAttackSpread;
            }
        }
    }

    #region Getters

    public int GetBossHealth () { return bossHealth; }

    public BossHealthbarController GetBossHealthBarController () { return bossHealthbarController; }

    #endregion
}