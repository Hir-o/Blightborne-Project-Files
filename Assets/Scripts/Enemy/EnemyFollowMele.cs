using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowMele : MonoBehaviour {

    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float minMovementSpeed = 2f;
    [SerializeField] private float maxMovementSpeed = 2f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float verticalSpeed = 8f;
    [SerializeField] private float attackRangeX = 1f;
    [SerializeField] private float attackRangeY = 1f;

    private Vector3 startingPosition;
    private Transform player;
    private Vector3 tempPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EnemyMele enemyMele;
    private BossHealthbarController bossHealthbarController;

    private bool isPlayerAlive = true;
    public bool isAlive = true;

    private Collider2D[] colliders;
    private bool isRunning = false;
    private Vector2 tempPlayerPosition;
    private Vector3 playerDirection, direction;
    [SerializeField] private LayerMask enemyMask;
    private Collider2D[] enemies;
    private Vector2 tempScaleVector;

    [Header("Status Effects Atributes")]
    [SerializeField] private float freezeRecoveryTimer = 3f;
    private float tempMoveSpeed;
    private Color startingColor;
    [SerializeField] private Color freezeColor;

    [Header("Wall Breach Code (Increased Attack Range)")]
    [SerializeField] private float increasedAttackRangeX = 15f;
    [SerializeField] private float increasedAttackRangeY = 1f;

    public void SetMovementSpeed (float newMovementSpeed) { movementSpeed = newMovementSpeed;}
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPosition = transform.position;
        enemyMele = GetComponent<EnemyMele>();
        tempScaleVector = new Vector3(transform.localScale.x, transform.localScale.y);

        startingColor = spriteRenderer.color;
        movementSpeed = Random.Range(minMovementSpeed, maxMovementSpeed);
    }

    private void Start()
    {
        bossHealthbarController = enemyMele.GetBossHealthbarController();

        if (bossHealthbarController != null)
            bossHealthbarController.gameObject.SetActive(false); // disable healthbar initially
    }

    private void LateUpdate()
    {
        if(isAlive == false) { return; }

        CheckForPlayer();

        // if (isPlayerAlive == false && Vector2.Distance(transform.position, startingPosition) > 0f)
        // {
        //     ReturnToStartingPosition();
        // }

        if (animator.GetBool("isAttacking") == false)
            FollowPlayer();
    }

    private void CheckForPlayer()
    {
        colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(attackRangeX, attackRangeY), 0);

        foreach(Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag(Tag.PlayerTag))
            {
                player = collider.GetComponent<Transform>();

                if (bossHealthbarController != null)
                    if (bossHealthbarController.gameObject.activeSelf == false)
                        bossHealthbarController.gameObject.SetActive(true);

                if (isRunning == false)
                {
                    enemyMele.SetStateToRun();
                    isRunning = true;
                }

                if (player.GetComponent<Player>().GetIsAlive())
                    isPlayerAlive = true; 
            }
        }
    }

    public void FollowPlayer()
    {
        if(player == null) { return; }
      
        if (player.GetComponent<Player>().GetIsAlive() == false)
        {
            isPlayerAlive = false;
            player = null;
            return;
        }

        LookAtPlayer();
        tempPlayerPosition = new Vector2(player.position.x, transform.position.y);

        if (Vector2.Distance(tempPlayerPosition, transform.position) > stoppingDistance)
        {
            enemyMele.SetStateToRun();
            transform.position = Vector2.MoveTowards
                (
                    transform.position,
                    tempPlayerPosition,
                    movementSpeed * Time.deltaTime
                );
        }
    }

    private void ReturnToStartingPosition()
    {
        LookAtStartingPosition();

        transform.position = Vector2.MoveTowards
            (
                transform.position,
                startingPosition,
                movementSpeed * Time.deltaTime
            );

        if(Vector2.Distance(transform.position, startingPosition) == 0f)
        {  
            animator.SetBool("isPlayerInRadius", false);
            animator.SetBool("isReturning", true);
        }
    }

    private void LookAtPlayer()
    {
        tempScaleVector = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        playerDirection = transform.position - player.position;

        if (playerDirection.x > 0f)
            transform.localScale = new Vector2(-tempScaleVector.x, tempScaleVector.y);
        else if (playerDirection.x < 0f)
            transform.localScale = new Vector2(tempScaleVector.x, tempScaleVector.y);
    }

    private void LookAtStartingPosition()
    {
         tempScaleVector = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        direction = transform.position - startingPosition;

        if (direction.x > 0f)
            transform.localScale = new Vector2(-tempScaleVector.x, tempScaleVector.y);
        else if (direction.x < 0f)
            transform.localScale = new Vector2(tempScaleVector.x, tempScaleVector.y);
    }

    public Transform GetPlayer()
    {
        return player;
    }

    public void SetAndFollowPlayer()
    {
        player = FindObjectOfType<Player>().GetComponent<Transform>();

        if(player == null) { return; }
        
        if (player.GetComponent<Player>().GetIsAlive() == false)
        {
            isPlayerAlive = false;
            player = null;
            return;
        }

        LookAtPlayer();
        tempPlayerPosition = new Vector2(player.position.x, transform.position.y);
        if (Vector2.Distance(tempPlayerPosition, transform.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards
                (
                    transform.position,
                    tempPlayerPosition,
                    movementSpeed * Time.deltaTime
                );
        }

        enemyMele.SetStateToRun();

        enemies = Physics2D.OverlapBoxAll(transform.position, new Vector2(attackRangeX, attackRangeY + 3), enemyMask);
                
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CompareTag(Tag.SmartEnemyTag))
                if (enemies[i].GetComponent<EnemyFollowMele>().GetPlayer() == null)
                    enemies[i].GetComponent<EnemyFollowMele>().SetAndFollowPlayer();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(attackRangeX, attackRangeY, 1));
    }

    public void IncreaseAttackRange()
    {
        attackRangeX = increasedAttackRangeX;
        attackRangeY = increasedAttackRangeY;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetType() == typeof(CompositeCollider2D))
        {
            player = null;
            enemyMele.SetStateToIdle();
        }
    }

    public void Slow(float slowFactor)
    {
        animator.speed = .5f;
        tempMoveSpeed = movementSpeed;
        movementSpeed -= slowFactor;

        if (movementSpeed <= 0f)
            movementSpeed = 0f;

        spriteRenderer.color = freezeColor;
        Invoke(nameof(ResetSpeed), freezeRecoveryTimer);
    }

    private void ResetSpeed()
    {
        movementSpeed = Random.Range(minMovementSpeed, maxMovementSpeed);
        spriteRenderer.color = startingColor;
        animator.speed = 1f;
    }

    public void Reset()
    {
        player = null;
        enemyMele.SetStateToIdle();
    }
}
