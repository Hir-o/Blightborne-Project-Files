using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMele : MonoBehaviour
{
    [Header("Enemy Atributes")]
    [SerializeField]
    private int damage = 5;

    [SerializeField] private int     health             = 10;
    [SerializeField] private Vector2 throwBackForce     = new Vector2(5f, 1f);
    [SerializeField] private float   throwBackForceTime = .4f;

    private Vector3                 tempPosition;
    private Transform               playerPosition;
    private Rigidbody2D             rigidbody;
    private Animator                animator;
    private Player                  player;
    private Vector2                 tempThrowBackForce;
    private CircleCollider2D        circleCollider;
    private CapsuleCollider2D       capsuleCollider;
    private PopUpController         enemyPopup;
    private EnemyHealthBar          healthBar;
    private BossHealthbarController bossHealthbarController;
    private EnemyFollowMele         enemyFollowMele;

    private bool canHit = true;

    //New code for orcs
    private enum EnemyState
    {
        Idle,
        Run,
        Attack,
        Hit
    };

    private EnemyState enemyState = EnemyState.Idle;

    private float timeBetweenAttack;
    private float startTimeBetweenAttack;

    [SerializeField] private Transform attackPosition;
    [SerializeField] private float     attackRange;
    [SerializeField] private LayerMask playerMask, hazardMask, fallingSpikesMask;

    private Collider2D[]    playerToDamage;
    private AnimationClip[] clips;
    private float           delayTime;
    private Arrow           arrow;
    private bool            instaDeath = true;

    [Header("Coins")]
    [SerializeField]
    private GameObject[] coins;

    public  int minCoins = 1;
    public  int maxCoins = 3;
    private int coinDropFactor;
    private int randomCoinDrop;

    [Header("Boss Code")]
    public bool isBoss;

    [SerializeField] private float nextPhaseMovementSpeed = 8f;
    [SerializeField] private float nextPhaseAnimatorSpeed = 2f;

    [SerializeField] private ParticleSystem ghostParticles;

    public enum BossType
    {
        Ice,
        Fire,
        None
    }

    public BossType bossType = BossType.None;

    [Header("Ghost Code")]
    [SerializeField]
    private GameObject _ghostGameObject;

    private GhostTrail _ghostTrail;

    [SerializeField] private GhostTrail _ghostTrailAttached;

    private bool isGhostShown, enableGhost, killedByHazard;

    [SerializeField] private float ghostShowTimer = 0f;
    [SerializeField] private float ghostShowDelay = .5f;

    [Header("Gates to Unlock After Death")]
    [SerializeField]
    private Animator[] gateAnimators;

    private int   maxBossHealth;
    private float healthChecker;

    private EnemyWaveCount enemyWaveCount;
    private EnemyCounter   _enemyCounter;

    [Header("Ring Health Steal Amount (%)")]
    [SerializeField]
    private int healthReturnedPercentage = 2;

    [Header("Blood Stain & VFX")]
    [SerializeField]
    private GameObject bloodStain;

    [SerializeField] private GameObject bloodVFX;

    private void Awake ()
    {
        playerPosition  = FindObjectOfType<Player>().GetComponent<Transform>();
        player          = playerPosition.GetComponent<Player>();
        rigidbody       = GetComponent<Rigidbody2D>();
        animator        = GetComponent<Animator>();
        circleCollider  = GetComponent<CircleCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        enemyPopup      = GetComponent<PopUpController>();
        enemyFollowMele = GetComponent<EnemyFollowMele>();
        maxBossHealth   = health;

        if (isBoss == false) healthBar = GetComponent<EnemyHealthBar>();

        if (isBoss) bossHealthbarController = FindObjectOfType<BossHealthbarController>();

        if (GetComponent<EnemyWaveCount>()) enemyWaveCount = GetComponent<EnemyWaveCount>();
    }

    private void Start ()
    {
        tempPosition       = transform.position;
        tempThrowBackForce = throwBackForce;
        coinDropFactor     = PlayerStats.DropRate;

        SetStartTimeBetweenAttack();
    }

    private void Update ()
    {
        if (isBoss && enableGhost)
        {
            //Debug.Log(Mathf.Sign(transform.localScale.x));
            ghostParticles.transform.localScale =
                new Vector3(Mathf.Sign(transform.localScale.x),
                            ghostParticles.transform.localScale.y, ghostParticles.transform.localScale.z);

            if (animator.GetBool("isRunning"))
                ghostParticles.Play();
            else
                ghostParticles.Stop();
        }

        //New code, may contain bugs
        //todo testit
        if (player.GetIsAlive() == false) enemyState = EnemyState.Idle;
        //end of new code

        if (timeBetweenAttack <= 0f)
        {
            //then attack
            if (enemyState == EnemyState.Attack)
            {
                if (player.GetIsAlive() == false)
                    enemyState = EnemyState.Idle;
                else
                    enemyState = EnemyState.Run;
            }

            timeBetweenAttack = startTimeBetweenAttack;
        }
        else
            timeBetweenAttack -= Time.deltaTime;

        switch (enemyState)
        {
            case EnemyState.Idle:
                animator.SetBool("isIdle",      true);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isRunning",   false);
                break;
            case EnemyState.Run:
                animator.SetBool("isRunning",   true);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isIdle",      false);
                break;
            case EnemyState.Attack:
                animator.SetBool("isAttacking", true);
                animator.SetBool("isIdle",      false);
                animator.SetBool("isRunning",   false);
                break;
            case EnemyState.Hit:
                animator.SetBool("isAttacking", false);
                animator.SetBool("isIdle",      false);
                animator.SetBool("isRunning",   false);
                break;
        }
    }

    private void SetStartTimeBetweenAttack ()
    {
        clips = animator.runtimeAnimatorController.animationClips;

        startTimeBetweenAttack = 0f;

        foreach (AnimationClip clip in clips)
            if (clip.name.Equals("attack"))
                startTimeBetweenAttack = clip.length;
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) enemyState = EnemyState.Attack;
    }

    private void OnTriggerStay2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) enemyState = EnemyState.Attack;
    }

    private IEnumerator ThrowBackEnemy ()
    {
        yield return new WaitForSeconds(throwBackForceTime);
        rigidbody.velocity = new Vector2(0f, 0f);
        tempThrowBackForce = throwBackForce;
        canHit             = true;
    }

    public void Attack () // called by event
    {
        playerToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, playerMask);

        for (int i = 0; i < playerToDamage.Length; i++)
        {
            playerToDamage[i].GetComponent<Player>().HitByMeleEnemy(damage);
            tempThrowBackForce.x = throwBackForce.x * -transform.localScale.x;
            return;
        }
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }

    public void TakeDamage (int damageTaken)
    {
        AudioController.Instance.EnemyHitSFX();

        if (isBoss && arrow != null)
        {
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
                if (health < damageTaken * 2)
                    health -= health;
                else
                    health -= damageTaken * 2;
            }
            else if (bossType == BossType.Ice && arrow.arrowType == Arrow.ArrowType.Fire)
            {
                if (health < damageTaken * 2)
                    health -= health;
                else
                    health -= damageTaken * 2;
            }
            else
            {
                if (health < arrow.GetDamage())
                    health -= health;
                else
                    health -= arrow.GetDamage();
            }

            healthChecker = (float) health / (float) maxBossHealth;

            bossHealthbarController.UpdateBossHealth(healthChecker);

            if (healthChecker <= .5 && BossFightController.objectCount == 1)
            {
                ChangePhase();
                BossFightController.EnableObstacle();
            }
            else if (healthChecker <= .75 && BossFightController.objectCount == 0) BossFightController.EnableObstacle();
        }
        else
        {
            if (health < damageTaken)
                health -= health;
            else
                health -= damageTaken;
        }

        enemyState = EnemyState.Hit;
        enemyPopup.EnablePopUp();
        animator.SetTrigger("hit");
    }

    public void TakeFragmentDamageBoss (int damageTaken) //aplies for bosses only
    {
        if (health < damageTaken)
            health -= health;
        else
            health -= damageTaken;

        healthChecker = (float) health / (float) maxBossHealth;
        bossHealthbarController.UpdateBossHealth(healthChecker);

        enemyState = EnemyState.Hit;
        animator.SetTrigger("hit");
    }

    public void CheckForDead () //called by event and from the collisionEnter2D
    {
        Instantiate(bloodStain, transform.position, Quaternion.identity);
        Instantiate(bloodVFX,   transform.position, Quaternion.identity);

        if (health <= 0)
        {
            if (killedByHazard == false)
                GameManager.Instance.HitStopEffect();

            if (enemyWaveCount)
            {
                _enemyCounter = FindObjectOfType<EnemyCounter>();
                _enemyCounter.UpdateEnemyCounter();
            }

            if (PlayerStats.IsRingPurchased) PlayerStats.AddHealth(healthReturnedPercentage);

            enemyFollowMele.isAlive = false;
            clips                   = animator.runtimeAnimatorController.animationClips;
            delayTime               = 0f;
            randomCoinDrop          = Random.Range(minCoins, maxCoins * coinDropFactor);

            for (int i = 0; i < randomCoinDrop; i++)
                Instantiate(coins[Random.Range(0, coins.Length)], transform.position, Quaternion.identity);

            foreach (AnimationClip clip in clips)
                if (clip.name.Equals("hit"))
                    delayTime = clip.length;

            if (isBoss)
            {
                AudioController.Instance.StopAllMusic();

                foreach (Animator anim in gateAnimators)
                {
                    anim.SetBool("isLocked", false);
                    BossFightController.DisableObstacles();
                }

                bossHealthbarController.gameObject.SetActive(false);
            }

            GameManager.rippleEffect.SetNewRipplePosition(GameManager
                                                          .mainCamera.WorldToScreenPoint(transform.position));
            Destroy(gameObject, delayTime);
        }

        if (isBoss == false) healthBar.UpdateHealthBar();
    }

    public void SetStateToRun () // called from EnemyFollowMele
    {
        enemyState = EnemyState.Run;
    }

    public void SetStateToIdle () // called from EnemyFollowMele
    {
        enemyState = EnemyState.Idle;
    }

    public void ResetStartTimeBetweenAttack () //called by event
    {
        timeBetweenAttack = startTimeBetweenAttack;
    }

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.CompareTag(Tag.BridgeTag))
        {
            if (other.gameObject.GetComponentInParent<BreakJoint>() != null)
                other.gameObject.GetComponentInParent<BreakJoint>().AddEnemy(this);
        }

        if (capsuleCollider.IsTouchingLayers(hazardMask))
            if (instaDeath) InstaKill();
    }

    private void OnCollisionExit2D (Collision2D other)
    {
        if (other.gameObject.CompareTag(Tag.BridgeTag))
        {
            if (other.gameObject.GetComponentInParent<BreakJoint>() != null)
                other.gameObject.GetComponentInParent<BreakJoint>().RemoveEnemy(this);
        }
    }

    public void InstaKill ()
    {
        instaDeath = false;
        killedByHazard = true;
        enemyState = EnemyState.Hit;
        animator.SetTrigger("hit");
        health -= health;
        CheckForDead();
    }

    public void SetArrowType (Arrow tempArrow) { arrow = tempArrow; }

    public int GetHealth () { return health; }

    public BossHealthbarController GetBossHealthbarController () { return bossHealthbarController; }

    public void ChangePhase ()
    {
        if (ghostParticles.gameObject.activeSelf == false) ghostParticles.gameObject.SetActive(true);

        enableGhost = true;
        enemyFollowMele.SetMovementSpeed(nextPhaseMovementSpeed);
        animator.speed = nextPhaseAnimatorSpeed;
    }
}