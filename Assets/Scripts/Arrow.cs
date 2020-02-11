using UnityEngine;
using UnityEngine.SceneManagement;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float throwXSpeed;
    [SerializeField] private float throwYSpeed;
    [SerializeField] private int   damage;
    [SerializeField] private float arrowPenetrationValue = 0.2f;

    [SerializeField] private GameObject trail;

    private Rigidbody2D    rigidbody;
    private Rigidbody2D    playerRigidbody;
    private BoxCollider2D  boxCollider;
    private Animator       animator;
    private SpriteRenderer spriteRenderer;

    private float                throwDirectionX;
    private Collider2D[]         colliders;
    private Vector2              throwDirection;
    private Vector2              arrowVelocity;
    private GameObject           trailObject;
    private Vector2              stillVelocity;
    private Enemy                enemy;
    private EnemyMele            enemyMele;
    private EnemyFollowMele      enemyFollowMele;
    private DemonicEyeBallHealth demonicEyeBall;
    private Tower                tower;

    [SerializeField] private LayerMask movingTrapsMask;

    [Header("Particle Code")]
    public bool isSpeacialArrow = false;

    public bool hasObjectsToSpawn = false;

    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private GameObject FragmentSpawner;

    [Header("Arrow Type Attributes")]
    [SerializeField]
    private bool isUnDestructable;

    public enum ArrowType
    {
        Fire,
        Ice,
        Special,
        Power,
        Normal
    }

    public ArrowType arrowType = ArrowType.Normal;

    [SerializeField] private float     radius;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float     slowFactor = 1f;

    private float specialArrowSacrificeDamage;

    public int GetDamage () { return damage; }

    private int powerCounter;

    void Start ()
    {
        damage          += PlayerStats.Strength * PlayerStats.StrengthMultiplier;
        rigidbody       =  GetComponent<Rigidbody2D>();
        playerRigidbody =  GameManager.player.GetComponent<Rigidbody2D>();
        boxCollider     =  GetComponent<BoxCollider2D>();
        animator        =  GetComponent<Animator>();
        spriteRenderer  =  GetComponent<SpriteRenderer>();

        ThrowSelf();

        if (arrowType == ArrowType.Special)
        {
            if (SceneManager.GetActiveScene().name    != "Rand's House"
                || SceneManager.GetActiveScene().name != "Katun Village"
                || SceneManager.GetActiveScene().name != "Shop")
                if (PlayerStats.Health >= PlayerStats.TotalHealth / 2f)
                {
                    PlayerStats.Health -= (PlayerStats.TotalHealth * 25f) / 100;
                    HealthBar.UpdateHealthBarStatus();
                }
        }
    }

    private void ThrowSelf ()
    {
        throwDirection       = new Vector2(playerRigidbody.transform.localScale.x, 1f);
        transform.localScale = throwDirection; //rotate sprite

        arrowVelocity      =  new Vector2(throwDirection.x * throwXSpeed, throwYSpeed);
        rigidbody.velocity += arrowVelocity;

        trailObject = Instantiate(trail, transform.position, Quaternion.identity) as GameObject;
        trailObject.GetComponent<TrailFollow>().SetTarget(transform);
        trailObject.GetComponent<TrailFollow>().ShootTrail(arrowVelocity);
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        if (arrowType != ArrowType.Power) stillVelocity = new Vector2(0f, 0f);

        if (isSpeacialArrow)
        {
            particle.transform.parent = null;
            Instantiate(hitParticle, transform.position, Quaternion.identity);

            if (hasObjectsToSpawn == true) Instantiate(FragmentSpawner, transform.position, Quaternion.identity);

            if (isUnDestructable == false) Destroy(particle.gameObject, 2f);
        }

        if (boxCollider.IsTouchingLayers(movingTrapsMask)) Destroy(this.gameObject);

        if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (collision.gameObject.CompareTag(Tag.BridgeTag)) Destroy(this.gameObject);

            if (collision.gameObject.CompareTag(Tag.CrateTag))
            {
                Destroy(this.gameObject);
                collision.gameObject.GetComponent<Crate>().DisableCrate();
            }

            rigidbody.isKinematic = true;
            boxCollider.enabled   = false;

            rigidbody.velocity = stillVelocity;

            throwDirectionX    =  playerRigidbody.transform.localScale.x;
            transform.position += new Vector3(throwDirectionX * arrowPenetrationValue, 0f, 0f);
        }
        else if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            animator.SetBool("hasHitEnemy", true);

            if (collision.gameObject.CompareTag(Tag.SmartEnemyTag))
            {
                enemyMele       = collision.gameObject.GetComponent<EnemyMele>();
                enemyFollowMele = collision.gameObject.GetComponent<EnemyFollowMele>();
                enemyMele.SetArrowType(this);
                enemyMele.TakeDamage(damage);

                if (arrowType == ArrowType.Ice)
                {
                    colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayerMask);

                    foreach (Collider2D coll in colliders)
                        if (coll.GetType() == typeof (CapsuleCollider2D))
                            if (coll.gameObject.GetComponent<EnemyMele>().isBoss == false) //todo test slowing on bosses
                                coll.gameObject.GetComponent<EnemyFollowMele>().Slow(slowFactor);
                }

                if (arrowType == ArrowType.Power)
                {
                    Physics2D.IgnoreCollision(boxCollider, collision.gameObject.GetComponent<Collider2D>());
                    powerCounter++;
                }

                if (enemyFollowMele.GetPlayer() == null) enemyFollowMele.SetAndFollowPlayer();
            }
            else if (collision.gameObject.CompareTag(Tag.BossTag))
            {
                demonicEyeBall = collision.gameObject.GetComponent<DemonicEyeBallHealth>();
                demonicEyeBall.TakeDamage(damage);
            }
            else if (collision.gameObject.CompareTag(Tag.TowerTag))
            {
                tower = collision.gameObject.GetComponent<Tower>();
                tower.DisableTower();
            }
            else
            {
                if (arrowType == ArrowType.Power)
                {
                    Physics2D.IgnoreCollision(boxCollider, collision.gameObject.GetComponent<Collider2D>());
                    powerCounter++;
                }

                enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
            }

            if (arrowType == ArrowType.Power)
            {
                if (powerCounter >= 3) { Destroy(gameObject); }
                else
                {
                    rigidbody.isKinematic = true;
                    boxCollider.enabled   = false;
                    rigidbody.velocity    = stillVelocity;
                }
            }
            else
            {
                rigidbody.isKinematic = true;
                boxCollider.enabled   = false;
                rigidbody.velocity    = stillVelocity;

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.SmartEnemyTag))
            if (collision.GetType() != typeof (CapsuleCollider2D))
                return;

        if (isSpeacialArrow)
        {
            if (isUnDestructable == false && arrowType != ArrowType.Power)
            {
                particle.transform.parent = null;
                Destroy(particle.gameObject, 2f);
            }

            Instantiate(hitParticle, transform.position, Quaternion.identity);

            if (hasObjectsToSpawn) Instantiate(FragmentSpawner, transform.position, Quaternion.identity);
        }

        if (boxCollider.IsTouchingLayers(movingTrapsMask)) Destroy(this.gameObject);

        if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (collision.gameObject.CompareTag(Tag.BridgeTag)) Destroy(this.gameObject);

            if (collision.gameObject.CompareTag(Tag.CrateTag))
            {
                if (isUnDestructable == false) Destroy(this.gameObject);

                collision.gameObject.GetComponent<Crate>().DisableCrate();
            }

            if (isUnDestructable == false)
            {
                rigidbody.isKinematic = true;
                boxCollider.enabled   = false;

                rigidbody.velocity = stillVelocity;

                throwDirectionX    =  playerRigidbody.transform.localScale.x;
                transform.position += new Vector3(throwDirectionX * arrowPenetrationValue, 0f, 0f);
            }
        }
        else if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            animator.SetBool("hasHitEnemy", true);

            if (collision.gameObject.CompareTag(Tag.SmartEnemyTag))
            {
                enemyMele       = collision.gameObject.GetComponent<EnemyMele>();
                enemyFollowMele = collision.gameObject.GetComponent<EnemyFollowMele>();
                enemyMele.SetArrowType(this);
                enemyMele.TakeDamage(damage);

                if (arrowType == ArrowType.Ice)
                {
                    colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayerMask);

                    foreach (Collider2D coll in colliders)
                        if (coll.GetType() == typeof (CapsuleCollider2D))
                            coll.gameObject.GetComponent<EnemyFollowMele>().Slow(slowFactor);
                }

                if (enemyFollowMele.GetPlayer() == null) enemyFollowMele.SetAndFollowPlayer();

                if (arrowType == ArrowType.Power)
                {
                    Physics2D.IgnoreCollision(boxCollider, collision.gameObject.GetComponent<Collider2D>());
                    powerCounter++;
                }
            }
            else if (collision.gameObject.CompareTag(Tag.BossTag))
            {
                demonicEyeBall = collision.gameObject.GetComponent<DemonicEyeBallHealth>();
                demonicEyeBall.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag(Tag.TowerTag))
            {
                tower = collision.gameObject.GetComponent<Tower>();
                tower.DisableTower();
            }
            else
            {
                if (arrowType == ArrowType.Power)
                {
                    Physics2D.IgnoreCollision(boxCollider, collision.gameObject.GetComponent<Collider2D>());
                    powerCounter++;
                }

                enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null) enemy.TakeDamage(damage);
            }

            if (isUnDestructable) { return; }

            if (arrowType == ArrowType.Power)
            {
                if (powerCounter >= 3)
                {
                    rigidbody.isKinematic = true;
                    boxCollider.enabled   = false;
                    rigidbody.velocity    = stillVelocity;

                    Destroy(gameObject);
                }
            }
            else
            {
                rigidbody.isKinematic = true;
                boxCollider.enabled   = false;
                rigidbody.velocity    = stillVelocity;

                Destroy(gameObject);
            }
        }
    }

    private void OnBecameInvisible ()
    {
        spriteRenderer.enabled = false;
        animator.enabled       = false;
        rigidbody.isKinematic  = true;
        boxCollider.enabled    = false;

        Destroy(this);
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}