using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonicEyeballMovement : MonoBehaviour
{
    [Header("Demonic Eye Movement Attributes")]
    [SerializeField]
    private float movementSpeed = 2f;

    [SerializeField] private float movementSpeedLaserAttack = 2f;

    [Header("Check for Player")]
    [SerializeField]
    private Transform checkPosition;

    [SerializeField] private float boxSizeX = 10f;

    [SerializeField] private float     boxSizeY = 10f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float     playerOffsetY;

    private Vector3              targetDirection;
    private Vector3              followYAxis;
    private Vector2              originalSize;
    private Collider2D[]         colliders;
    private Transform            player;
    private DemonicEyeballAttack demonicAttack;
    private DemonicEyeBallHealth demonicHealth;

    public  bool isAlive = true;
    public  bool isDeathAnimationActivated;
    private bool isPlayerAlive = true;

    private void Start ()
    {
        originalSize  = new Vector2(transform.localScale.x, transform.localScale.y);
        demonicAttack = GetComponent<DemonicEyeballAttack>();
        demonicHealth = GetComponent<DemonicEyeBallHealth>();

        demonicHealth.SetDemonicAttackObject(demonicAttack);

        //bossHealthbarController = FindObjectOfType<BossHealthbarController>();
    }

    private void FixedUpdate ()
    {
        if (isAlive == false) { return; }
        if (isDeathAnimationActivated) { return; }

        CheckForPlayer();
        FollowPlayer();
    }

    private void CheckForPlayer ()
    {
        colliders = Physics2D.OverlapBoxAll(checkPosition.position, new Vector2(boxSizeX, boxSizeY), 0f, playerMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag(Tag.PlayerTag))
            {
                player = collider.GetComponent<Transform>();

                if (demonicAttack.isAttacking == false)
                {
                    AudioController.Instance.PlayBossTheme();

                    // send player reference to DemonicEyeballAttack, and start attacking the player with projectiles
                    demonicAttack.InitializeAttack(player.GetComponent<Player>());
                }

                if (player.GetComponent<Player>().GetIsAlive()) isPlayerAlive = true;
            }
        }
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

        followYAxis = new Vector3(transform.position.x, player.position.y + playerOffsetY, transform.position.z);

        if (demonicAttack.isShootingLaser == false)
            transform.position = Vector2.Lerp(transform.position, followYAxis, movementSpeed * Time.deltaTime);
        else
            transform.position = Vector2.MoveTowards
                (
                 transform.position,
                 followYAxis,
                 movementSpeedLaserAttack * Time.deltaTime
                );
    }

    private void LookAtPlayer ()
    {
        targetDirection = transform.position - player.position;

        if (targetDirection.x > 0f)
            transform.localScale                              = new Vector2(originalSize.x,  originalSize.y);
        else if (targetDirection.x < 0f) transform.localScale = new Vector2(-originalSize.x, originalSize.y);
    }

    public void IncreaseBossMovementSpeed (float amount) { movementSpeed += amount; }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(checkPosition.position, new Vector3(boxSizeX, boxSizeY, 1f));
    }
}