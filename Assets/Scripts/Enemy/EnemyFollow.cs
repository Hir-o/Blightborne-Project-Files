using System.Collections;
using UnityEngine;
using WaterRippleForScreens;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float minMovementSpeed  = 3f;
    [SerializeField] private float maxMovementSpeed  = 7f;
    [SerializeField] private float movementSpeed     = 2f;
    [SerializeField] private float attackingSpeed    = 3f;
    [SerializeField] private float stoppingDistance  = 1f; //also used for boss bat resting position
    [SerializeField] private float attackingDistance = 1f;
    [SerializeField] private float radius            = 3f;

    private Vector3                 startingPosition;
    private Transform               player;
    private Vector3                 tempPosition;
    private Animator                animator;
    private BossHealthbarController bossHealthbarController;
    private Collider2D[]            colliders;
    private EnemyBat                enemyBat;

    private bool isPlayerAlive = true;

    public bool isAlive = true;

    private bool    isBoss;
    private Vector2 originalSize;

    [Header("Boss Code")]
    [SerializeField]
    private float reducedBatSpeed = 3f;

    [SerializeField] private float            restTimer = 8f;
    [SerializeField] private float            sizeX;
    [SerializeField] private float            sizeY;
    [SerializeField] private Vector3          offset; // for collision detection offset
    [SerializeField] private GameObject       pointEffector;
    [SerializeField] private RippleEffectBoss bossRippleEffect;
    [SerializeField] private PopUpController  _popUpController;

    [Header("Sinusoidal Movement Code")]
    [SerializeField]
    private float frequency = 10f;

    [SerializeField] private float magnitude    = .5f;
    [SerializeField] private float minMagnitude = 3f;
    [SerializeField] private float maxMagnitude = 8f;

    [Header("Resting Position")]
    [SerializeField]
    private Transform restingPosition;

    public bool isMovingAwayFromPlayer;

    private bool isResting;

    private float startingMovementSpeed;

    private void Awake ()
    {
        if (isBoss == false)
        {
            magnitude     = Random.Range(minMagnitude,     maxMagnitude);
            movementSpeed = Random.Range(minMovementSpeed, maxMovementSpeed);
        }

        animator              = GetComponent<Animator>();
        enemyBat              = GetComponent<EnemyBat>();
        startingPosition      = transform.position;
        originalSize          = new Vector2(transform.localScale.x, transform.localScale.y);
        startingMovementSpeed = movementSpeed;
        attackingSpeed        = startingMovementSpeed + 1f;
    }

    private void Start ()
    {
        isBoss = GetComponent<EnemyBat>().isBoss;

        bossHealthbarController = GetComponent<EnemyBat>().GetBossHealthBarController();

        if (bossHealthbarController != null)
            bossHealthbarController.gameObject.SetActive(false); // disable healthbar initially
    }

    private void LateUpdate ()
    {
        if (isAlive == false) { return; }

        CheckForPlayer();

        if (isPlayerAlive == false && Vector2.Distance(transform.position, startingPosition) > 0f)
            ReturnToStartingPosition();

        FollowPlayer();
    }

    private void CheckForPlayer ()
    {
        if (isBoss == false)
            colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        else
            colliders = Physics2D.OverlapBoxAll(transform.position + offset, new Vector2(sizeX, sizeY), 0f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag(Tag.PlayerTag))
            {
                if (bossHealthbarController != null)
                    if (bossHealthbarController.gameObject.activeSelf == false)
                        bossHealthbarController.gameObject.SetActive(true); // disable healthbar initially

                player = collider.GetComponent<Transform>();

                if (player.GetComponent<Player>().GetIsAlive() == true)
                {
                    isPlayerAlive = true;
                    animator.SetBool("isPlayerInRadius", true);
                    animator.SetBool("isReturning",      false);
                }
            }
        }
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireCube(transform.position + offset, new Vector3(sizeX, sizeY, 1f));
    }

    private void FollowPlayer ()
    {
        if (player == null) { return; }

        if (player.GetComponent<Player>().GetIsAlive() == false)
        {
            isPlayerAlive = false;
            player        = null;
            return;
        }

        LookAtPlayer();

        if (isResting)
        {
            if (movementSpeed != reducedBatSpeed) movementSpeed = startingMovementSpeed;

            if (Vector2.Distance(restingPosition.position, transform.position) > stoppingDistance)
            {
                transform.position = Vector2.MoveTowards
                    (
                     transform.position,
                     restingPosition.position,
                     movementSpeed * Time.deltaTime
                    );
            }
        }
        else if (isMovingAwayFromPlayer)
        {
            transform.position = Vector2.MoveTowards
                (
                 transform.position,
                 (new Vector3(player.transform.position.x * -100, player.transform.position.y * -100f,
                              player.transform.position.z)) +
                 transform.up * Mathf.Sin(Time.time * frequency) * magnitude,
                 movementSpeed * Time.deltaTime
                );
        }
        else
        {
            if (Vector2.Distance(player.position, transform.position) > attackingDistance)
            {
                if (movementSpeed != reducedBatSpeed) movementSpeed = startingMovementSpeed;

                if (Vector2.Distance(player.position, transform.position) > stoppingDistance)
                {
                    transform.position = Vector2.MoveTowards
                        (
                         transform.position,
                         player.position + transform.up * Mathf.Sin(Time.time * frequency) * magnitude,
                         movementSpeed * Time.deltaTime
                        );
                }
            }
            else if (Vector2.Distance(player.position, transform.position) < attackingDistance)
            {
                if (movementSpeed != reducedBatSpeed) movementSpeed = attackingSpeed;

                if (Vector2.Distance(player.position, transform.position) > stoppingDistance)
                {
                    transform.position = Vector2.MoveTowards
                        (
                         transform.position,
                         player.position,
                         movementSpeed * Time.deltaTime
                        );
                }
            }
        }
    }

    private void ReturnToStartingPosition ()
    {
        LookAtStartingPosition();

        transform.position = Vector2.MoveTowards
            (
             transform.position,
             startingPosition,
             movementSpeed * Time.deltaTime
            );

        if (Vector2.Distance(transform.position, startingPosition) == 0f)
        {
            animator.SetBool("isPlayerInRadius", false);
            animator.SetBool("isReturning",      true);
        }
    }

    private void LookAtPlayer ()
    {
        Vector3 direction = transform.position - player.position;

        if (direction.x > 0f)
            transform.localScale                        = new Vector2(-originalSize.x, originalSize.y);
        else if (direction.x < 0f) transform.localScale = new Vector2(originalSize.x,  originalSize.y);
    }

    private void LookAtStartingPosition ()
    {
        Vector3 direction = transform.position - startingPosition;

        if (direction.x > 0f)
            transform.localScale                        = new Vector2(-originalSize.x, originalSize.y);
        else if (direction.x < 0f) transform.localScale = new Vector2(originalSize.x,  originalSize.y);
    }

    public void ChangePhase ()
    {
        isResting = true;
        //movementSpeed = reducedBatSpeed;
        enemyBat.StopProjectileAttack();
        enemyBat.fireShield.SetActive(true);
        pointEffector.SetActive(true);

        CameraShakeController.Instance.BatBossCameraShake(restTimer);
        PostProcessingController.Instance.EnableChromaticAberration();

        StopCoroutine(nameof(Reset));
        StopCoroutine(nameof(Rage));

        StartCoroutine(nameof(Reset));
        StartCoroutine(nameof(Rage));
    }

    private IEnumerator Reset ()
    {
        yield return new WaitForSeconds(restTimer);

        movementSpeed = startingMovementSpeed;
        enemyBat.fireShield.SetActive(false);
        enemyBat.InitializeProjectileAttack();
        isResting = false;

        PostProcessingController.Instance.DisableChromaticAberration();
    }

    private IEnumerator Rage ()
    {
        for (int i = 0; i < Mathf.RoundToInt(restTimer) * 2; i++)
        {
            bossRippleEffect.SetNewRipplePositionBatBoss(GameManager
                                                         .mainCamera.WorldToScreenPoint(transform.position));

            _popUpController.BatRoarPopUp();

            yield return new WaitForSeconds(.5f);
        }

        pointEffector.SetActive(false);
    }
}