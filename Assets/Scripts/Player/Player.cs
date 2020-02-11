using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private PlayerAttack playerAttack;

    [Tooltip("Player Specific Variables")]
    [SerializeField]
    private float movementSpeed = 10f;

    [SerializeField] private float   jumpSpeed           = 10f;
    [SerializeField] private float   jumpTime            = .5f;
    [SerializeField] private float   climbingSpeed       = 5f;
    [SerializeField] private float   rollingSpeed        = 30f;
    [SerializeField] private float   rollStaminaRequired = 10f;
    [SerializeField] private float   wallSlideMaxSpeed   = 3f;
    [SerializeField] private Vector2 throwBackForce      = new Vector2(2f, 20f);

    [Header("Wall Jump Variables")]
    [SerializeField]
    private Vector2 wallLeap;

    private bool isAlive       = true;
    private bool isTeleporting = true;
    private bool isShooting;
    private bool isJumpPressed = false;
    private bool isJumping;
    private bool isWallJumping;
    private bool isHoldingJump;
    private bool isSliding;
    private bool isRolling, canInterruptRolling;
    private bool isClimbingLadder;
    private bool isBeingHit;
    private bool isRollingMidAir;
    public  bool isDeactivated;

    private float startGravityScale;
    private float controlHorizontal, controlHorizontalLeft, controlHorizontalRight;
    private float horizontalSpeed;

    public int collisionLayer;
    public int playerLayer;

    private float jumpTimeCounter;

    private Rigidbody2D             rigidbody;
    private Animator                animator;
    private CapsuleCollider2D       bodyCollider;
    private BoxCollider2D           feetCollider;
    private CircleCollider2D        headCollider;
    private Enemy                   enemy;
    private DemonicEyeballAttack    boss;
    private PlayerParticles         playerParticles;
    private CinemachineCameraShaker cameraShaker;
    private PopUpController         playerPopup;

    private Vector2 jumpVelocityToAdd, climbVelocity, slideDownVelocity, playerVelocity;
    private float   controlVertical;
    private float   verticalSpeed;
    private bool    hasPlayerSpeed, isFallingAfterSlide, isJumpingOverMushroom, interruptJump;

    [Header("Physic Materials")]
    public PhysicsMaterial2D zeroFriction;

    public PhysicsMaterial2D gluey;

    [Header("Enemy hit bool (For jumping)")]
    [SerializeField]
    private bool canJump = true;

    [Header("Crate Collision Code")]
    [SerializeField]
    private float crateIgnoreTimer = .5f;

    [SerializeField] private float        crateRangeX;
    [SerializeField] private float        crateRangeY;
    [SerializeField] private Collider2D[] crateColliders;

    [Header("Roll Again Timer")]
    [SerializeField]
    private float rollDuration = .26f;

    [SerializeField] private float rollInterruptDuration = .15f;

    [SerializeField]
    private float rollAgainTimer, rollInterruptTimer;


    [Header("Slime Attack Timers")]
    [SerializeField]
    private float slimeAttackTimer;

    [SerializeField] private float slimeAttackDuration = .1f;

    [Header("Bat Attack Timers")]
    [SerializeField]
    private float batAttackTimer;

    [SerializeField] private float batAttackDuration = .1f;

    [Header("Mele Enemy Timers")]
    [SerializeField]
    private float meleEnemyAttackTimer;

    [SerializeField] private float meleEnemyAttackDuration = .1f;

    [Header("Hazard Timers & Variables")]
    [SerializeField]
    private float hazardDamageTimer;

    [SerializeField] private float hazardDamageDuration = .5f;
    [SerializeField] private int   hazardDamage         = 25;

    [Header("Hit While Jumping Timers & Variables")]
    [SerializeField]
    private float hitWhileJumpingTimer;

    [SerializeField] private float hitWhileJumpingDuration = .5f;
    private                  bool  isHitWhileJumping;

    [Header("Collision Check Timers")]
    [SerializeField]
    private float collisionTimer;

    [SerializeField] private float collisionDuration = 2f;

    [Header("Change direction while shooting")]
    [SerializeField]
    private float flipAttackTimer;

    [SerializeField] private float flipAttackDelay = .4f;

    [Header("Ghost Code")]
    [SerializeField]
    private GameObject _ghostGameObject;

    private GhostTrail _ghostTrail;

    [SerializeField] private GhostTrail _ghostTrailAttached;

    [Header("Squash & Stretch Animation")]
    [SerializeField]
    private Animator squashStretchAnimator;

    [Header("Better Jump Code")]
    [SerializeField]
    private float fallMultiplier = 2.5f;

    [SerializeField] private float lowJumpMultiplier = 2f;

    private bool hitBySlime,
                 hitByBat,
                 hitByHazard,
                 hitByMeleEnemy,
                 isJumpStored,
                 hasRolledWhileSliding,
                 isRollingBeforeJump;

    private Vector2 dir;
    private float   rollDirection;

    [HideInInspector] public bool playRollVFX;
    [HideInInspector] public bool isImmuneToProjectiles;

    //used to track the first time the player falls on bridge and play the squash animation once
    private int bridgeCounterHop;

    private enum RunningState
    {
        Running,
        Stopped
    }

    private RunningState runningState = RunningState.Stopped;

    public void IsAlive(bool _isAlive) { isAlive = _isAlive; }

    public void IsShooting(bool _isShooting) { isShooting = _isShooting; }

    public bool GetIsAlive()   { return isAlive; }
    public bool GetIsJumping() { return isJumping; }

    public bool GetIsTeleporting() { return isTeleporting; }

    public bool GetIsRollingMidAir() { return isRollingMidAir; }

    void Start()
    {
        playerAttack    = GetComponent<PlayerAttack>();
        rigidbody       = GetComponent<Rigidbody2D>();
        animator        = GetComponent<Animator>();
        bodyCollider    = GetComponent<CapsuleCollider2D>();
        feetCollider    = GetComponent<BoxCollider2D>();
        headCollider    = GetComponent<CircleCollider2D>();
        cameraShaker    = FindObjectOfType<CinemachineCameraShaker>();
        playerParticles = GetComponent<PlayerParticles>();
        playerPopup     = GetComponent<PopUpController>();

        collisionLayer = LayerMask.NameToLayer("Enemy");
        playerLayer    = LayerMask.NameToLayer("Player");

        startGravityScale = rigidbody.gravityScale;
        Physics2D.IgnoreLayerCollision(collisionLayer, playerLayer, false);
    }

    void FixedUpdate()
    {
        if (isShooting) rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);

        if (GameManager.IsAbilityPanelOpen)
        {
            if (rigidbody.bodyType != RigidbodyType2D.Static)
                rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);

            if (animator.GetBool("isRunning")) animator.SetBool("isRunning", false);

            return;
        }

        CheckForFallingTransition();

        if (isDeactivated)
        {
            if (rigidbody.bodyType != RigidbodyType2D.Static) rigidbody.velocity = new Vector2(0f, 0f);

            StopAllAnimations();

            return;
        }

        if (GameManager.AreControlsEnabled == false)
        {
            StopAllAnimations();
            rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);
            return;
        }

        if (isAlive == false) { return; }

        if (isTeleporting) { return; }

        CheckForLayerCollision();

        if (PlayerStats.Stamina < rollStaminaRequired && InputControl.GetButtonDown("Roll"))
        {
            AudioController.Instance.NoEnergySFX(playerPopup);
        }

        Roll();
        Jump();
        HoldJump();

        if (isShooting)
        {
            if (isJumpStored) isJumpStored = false;

            if (flipAttackTimer < flipAttackDelay)
            {
                flipAttackTimer += Time.fixedDeltaTime;
                FlipSpriteWhileAttacking();
            }

            return;
        }
        else if (flipAttackTimer != 0f) flipAttackTimer = 0f;

        if (PlayerStats.IsPyramidPurchased) CheckForFalling(); // Increase drag value

        CheckForRolling();

        if (isRolling) return;

        Run();
        FlipSprite();
        ClimbLadder();

        CheckHitBySlime();
        CheckHitByBat();
        CheckHitByHazard();
        CheckHitByMeleEnemy();
        CheckHitWhileJumping();
    }

    private void CheckForFallingTransition()
    {
        if (rigidbody.velocity.y < -0.1)
        {
            if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) == false)
            {
                animator.SetBool("isSliding", false);
                animator.SetBool("isFalling", true);
            }

//            playerParticles.StopSlideParticles();
//
//            if (isShooting == false)
//                rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
//        else if (rigidbody.velocity.y          > 0      && !InputControl.GetButton("Jump") &&
//                 animator.GetBool("isRolling") == false && isShooting == false && isJumpingOverMushroom == false)
//            rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        else
            animator.SetBool("isFalling", false);
    }

    private void CheckForRolling()
    {
        if (isRolling)
        {
            if (canInterruptRolling == false)
            {
                if (rollInterruptTimer < rollInterruptDuration)
                    rollInterruptTimer += Time.fixedDeltaTime;
                else { canInterruptRolling = true; }
            }

            if (rollAgainTimer < rollDuration)
                rollAgainTimer += Time.fixedDeltaTime;
            else
            {
                animator.SetBool("isRolling", false);
                isRolling      = false;
                rollAgainTimer = 0f;
                Physics2D.IgnoreLayerCollision(collisionLayer, playerLayer, false);
                canInterruptRolling = false;
                rollInterruptTimer  = 0f;
            }
        }
    }

    private void CheckForFalling()
    {
        if (rigidbody.velocity.y < -0.1)
            rigidbody.drag = PlayerStats.FallingDragLevel;
        else
            rigidbody.drag = 0f;
    }

    private void CheckHitBySlime()
    {
        if (hitBySlime)
        {
            if (slimeAttackTimer < slimeAttackDuration)
                slimeAttackTimer += Time.fixedDeltaTime;
            else
            {
                hitBySlime       = false;
                slimeAttackTimer = 0f;
            }
        }
    }

    private void CheckHitByBat()
    {
        if (hitByBat)
        {
            if (batAttackTimer < batAttackDuration)
                batAttackTimer += Time.fixedDeltaTime;
            else
            {
                hitByBat       = false;
                batAttackTimer = 0f;
            }
        }
    }

    private void CheckHitByHazard()
    {
        if (hitByHazard)
        {
            if (hazardDamageTimer < hazardDamageDuration)
                hazardDamageTimer += Time.fixedDeltaTime;
            else
            {
                hitByHazard       = false;
                hazardDamageTimer = 0f;
            }
        }
    }

    private void CheckHitByMeleEnemy()
    {
        if (hitByMeleEnemy)
        {
            if (meleEnemyAttackTimer < meleEnemyAttackDuration)
                meleEnemyAttackTimer += Time.fixedDeltaTime;
            else
            {
                hitByMeleEnemy       = false;
                meleEnemyAttackTimer = 0f;
            }
        }
    }

    private void CheckHitWhileJumping()
    {
        if (isHitWhileJumping)
        {
            if (hitWhileJumpingTimer < hitWhileJumpingDuration)
                hitWhileJumpingTimer += Time.fixedDeltaTime;
            else
            {
                isHitWhileJumping    = false;
                hitWhileJumpingTimer = 0f;
            }
        }
    }

    private void CheckForLayerCollision()
    {
        if (Physics2D.GetIgnoreLayerCollision(collisionLayer, playerLayer))
        {
            if (collisionTimer < collisionDuration)
                collisionTimer += Time.fixedDeltaTime;
            else
            {
                collisionTimer = 0f;
                Physics2D.IgnoreLayerCollision(collisionLayer, playerLayer, false);
            }
        }
        else { collisionTimer = 0f; }
    }

    private void Run()
    {
        //value between -1 and +1, when throwing left or right
        controlHorizontal = InputControl.GetAxisRaw("Horizontal");
        horizontalSpeed   = controlHorizontal * movementSpeed;

        playerVelocity = new Vector2(horizontalSpeed, rigidbody.velocity.y);

        if (rigidbody.bodyType == RigidbodyType2D.Dynamic) rigidbody.velocity = playerVelocity;

        if (isJumping) { return; }

        if (isSliding) { return; }

        RunAnimation();
    }

    private void Jump()
    {
        if (isHitWhileJumping) { return; }

        if (canJump == false) { return; }

        if (isJumpStored == false)
        {
            if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))
                && isWallJumping == false) { return; }
        }
        else if (animator.GetBool("isJumping")) { return; }

        if (isClimbingLadder) { return; }

        if (animator.GetBool("isHit")) { return; }

        if (InputControl.GetButtonDown("Jump"))
        {
            if (rigidbody.velocity.y > 0.1)
            {
                if (isShooting)
                {
                    isShooting = false;
                    playerAttack.InterruptShooting();
                }
            }

            if (animator.GetBool("isRolling") && isRollingMidAir == false)
            {
                if (PlayerStats.IsRolling)
                {
                    //StopCoroutine(RollWait());
                    CancelInvoke(nameof(SetIsRollingToFalse));
                    isRollingMidAir       = false;
                    PlayerStats.IsRolling = false;
                    isRolling             = false;
                    DOTween.KillAll();
                    rigidbody.drag = 0f;

                    _ghostTrail.ShowGhost();
                    _ghostTrailAttached.ShowGhost();
                }
            }

            rigidbody.gravityScale = startGravityScale;
            playerParticles.PlayJumpParticles();

            jumpTimeCounter = jumpTime;

            animator.SetBool("isSliding", false);
            animator.SetBool("isRolling", false);

            playerParticles.StopSlideParticles();

            isJumping = false;

            if (isWallJumping)
            {
                rigidbody.velocity = Vector2.zero;

                wallLeap.x *= -transform.localScale.x;

                rigidbody.velocity += wallLeap;
            }
            else
            {
                if (isJumpStored) rigidbody.velocity = Vector2.zero;
                jumpVelocityToAdd  = new Vector2(0f, jumpSpeed);
                rigidbody.velocity = jumpVelocityToAdd;

                isHoldingJump = true;
            }

            isWallJumping = false;
            interruptJump = false;
            isJumping     = true;
            JumpSoundSFX();

            animator.SetBool("isRunning", false);
            squashStretchAnimator.SetTrigger("stretch");
            JumpAnimation();
        }
    }

    private void HoldJump()
    {
        if (animator.GetBool("isHit")) { return; }

        if (isHitWhileJumping) { return; }

        if (interruptJump) { return; }

        if (InputControl.GetButton("Jump") && isJumping)
        {
            if (rigidbody.velocity.y > 0.1)
            {
                if (isShooting)
                {
                    isShooting = false;
                    playerAttack.InterruptShooting();
                }
            }

            if (animator.GetBool("isRolling"))
            {
                if (PlayerStats.IsRolling)
                {
                    CancelInvoke(nameof(SetIsRollingToFalse));
                    isRollingMidAir       = false;
                    PlayerStats.IsRolling = false;
                    isRolling             = false;
                    rigidbody.velocity    = Vector2.zero;
                }
            }

            animator.SetBool("isSliding", false);

            playerParticles.StopSlideParticles();

            if (isWallJumping)
                return;
            else
            {
                if (isHoldingJump == false) { return; }

                if (jumpTimeCounter > 0f)
                {
                    rigidbody.velocity += Vector2.up + new Vector2(0f, .6f);
                    jumpTimeCounter    -= Time.deltaTime;
                }
            }

            isHoldingJump = true;
        }
    }

    private void Roll()
    {
        if (AssistController.IsInfiniteDash == false)
        {
            if (PlayerStats.Stamina < rollStaminaRequired) { return; }

            if (animator.GetBool("isClimbing")) { return; }

            if (isRollingMidAir) { return; }

            if (hasRolledWhileSliding) { return; }

            if (isRollingBeforeJump && isJumping == false) { return; }
        }

        if (isJumping && isRollingMidAir == false)
        {
            if (InputControl.GetButtonDown("Roll"))
            {
                if (canInterruptRolling == false)
                    if (isRolling) { return; }
                    else
                    {
                        rollAgainTimer      = 0f;
                        rollInterruptTimer  = 0f;
                        canInterruptRolling = false;
                        animator.SetBool("isRolling", false);
                    }

                if (isShooting)
                {
                    isShooting = false;
                    playerAttack.InterruptShooting();
                }

                if (animator.GetBool("isSliding")) hasRolledWhileSliding = true;

                playRollVFX = false;
                CancelInvoke(nameof(SetIsRollingToFalse));
                DisableCrateCollision();
                isRollingMidAir        = true;
                rigidbody.velocity     = Vector2.zero;
                PlayerStats.IsRolling  = true;
                rigidbody.gravityScale = 0f;
                isRolling              = true;
                animator.SetBool("isRunning", false);
                animator.SetBool("isJumping", false);
                RollingAnimation();
                isJumpStored = false;
                isJumping    = false;
            }

            return;
        }

        if (InputControl.GetButtonDown("Roll"))
        {
            if (canInterruptRolling == false)
            {
                if (isRolling) { return; }
            }
            else
            {
                rollAgainTimer      = 0f;
                rollInterruptTimer  = 0f;
                canInterruptRolling = false;
            }

            if (isShooting)
            {
                isShooting = false;
                playerAttack.InterruptShooting();
            }

            if (animator.GetBool("isSliding")) hasRolledWhileSliding = true;

            playRollVFX = true;
            CancelInvoke(nameof(SetIsRollingToFalse));
            DisableCrateCollision();
            isRollingMidAir        = true;
            rigidbody.velocity     = Vector2.zero;
            PlayerStats.IsRolling  = true;
            rigidbody.gravityScale = 0f;
            isRolling              = true;
            animator.SetBool("isRunning", false);
            RollingAnimation();
            isJumpStored = false;
        }
    }

    private void DisableCrateCollision()
    {
        crateColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(crateRangeX, crateRangeY), 0f);

        foreach (Collider2D coll in crateColliders)
            if (coll.CompareTag(Tag.CrateTag))
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), coll.GetComponent<Collider2D>(),
                                          true);

        Invoke(nameof(SetIsRollingToFalse), crateIgnoreTimer);
    }

    //flip the player sprite left or right based on the movement throwing
    private void FlipSprite()
    {
        hasPlayerSpeed = Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon;

        if (hasPlayerSpeed) transform.localScale = new Vector2(Mathf.Sign(rigidbody.velocity.x), 1f);
    }

    private void FlipSpriteWhileAttacking()
    {
        if (Input.GetAxis("Horizontal") < 0)
            transform.localScale                                       = new Vector2(-1f, 1f);
        else if (Input.GetAxis("Horizontal") > 0) transform.localScale = new Vector2(1f,  1f);

        //Works only on keyboard
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            transform.localScale = new Vector2(1f, 1f);
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            transform.localScale = new Vector2(-1f, 1f);
    }

    private void ClimbLadder()
    {
        if (animator.GetBool("isHit")) { return; }

        if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            animator.SetBool("isClimbing", false);
            isClimbingLadder = false;

            if (animator.GetBool("isRolling") == false) rigidbody.gravityScale = startGravityScale;

            return;
        }
        else
        {
            animator.SetBool("isJumping", false);
            Vector2 stillYVelocity = new Vector2(rigidbody.velocity.x, 0f);
            rigidbody.velocity = stillYVelocity;

            rigidbody.gravityScale = 0f;
        }

        if (InputControl.GetButton("Up") || InputControl.GetButton("Down")) // || InputControl.GetAxis("Vertical") != 0f
        {
            isClimbingLadder = true;
            controlVertical  = InputControl.GetAxisRaw("Vertical");
            verticalSpeed    = controlVertical * climbingSpeed;

            climbVelocity      = new Vector2(0f, verticalSpeed);
            rigidbody.velocity = climbVelocity;

            Vector2 playerVelocity = new Vector2(horizontalSpeed, rigidbody.velocity.y);
            if (InputControl.GetButton("Right") || InputControl.GetButton("Left"))
            {
                controlHorizontal = InputControl.GetAxisRaw("Horizontal");
                horizontalSpeed   = controlHorizontal * movementSpeed;

                rigidbody.velocity = playerVelocity;
            }

            animator.SetBool("isSliding", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", false);

            playerParticles.StopSlideParticles();

            ClimbAnimation();
        }

        if (InputControl.GetButtonUp("Up") || InputControl.GetButtonUp("Down"))
        {
            Vector2 stillVelocity = Vector2.zero;
            rigidbody.velocity = stillVelocity;

            rigidbody.gravityScale = 0f;

            ClimbAnimation();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);
        CheckForWallClimb(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.LavaTag)) { return; }

        if (collision.gameObject.CompareTag(Tag.EctoplasmTag)) { return; }

        ProcessCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.GroundTag))
        {
            //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Roll") && isJumping == false)
            if (isRolling && isJumping == false)
                isRollingMidAir = true;
            else
                isRollingMidAir = false;

            if (isJumping) hasRolledWhileSliding = false;

            isJumpStored = true;
            playerParticles.StopSlideParticles();
        }

        if (collision.gameObject.CompareTag(Tag.BridgeTag))
        {
            isJumpStored     = true;
            bridgeCounterHop = 0;
            animator.SetBool("isOnBridge", false);
            playerParticles.StopSlideParticles();
        }

        if (collision.gameObject.CompareTag(Tag.CrateTag))
        {
            isJumpStored = true;
            animator.SetBool("isOnBridge", false);
            playerParticles.StopSlideParticles();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.LadderTag))
        {
            isJumpStored    = false;
            isRollingMidAir = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.LadderTag))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Roll") && isJumping == false)
                isRollingMidAir = true;

            isJumpStored = true;
        }
    }

    private void ProcessCollision(Collision2D collision)
    {
        if (isAlive == false) { return; }

        if (headCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;

        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))
            && !feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (rigidbody.velocity.y <= -wallSlideMaxSpeed)
            {
                slideDownVelocity  = new Vector2(0f, -wallSlideMaxSpeed);
                rigidbody.velocity = slideDownVelocity;
                animator.SetBool("isSliding", true);
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isRunning", false);

                isJumping = false;
                //isRollingMidAir     = false;
                isRollingBeforeJump = false;
                playerParticles.PlaySlideParticles();
            }

            isJumpStored = false;
        }
        else if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            isJumpingOverMushroom = false;

            if (animator.GetBool("isRolling") == false) rigidbody.gravityScale = startGravityScale;

            if (collision.gameObject.CompareTag(Tag.BridgeTag)) animator.SetBool("isOnBridge", true);

            if (animator.GetBool("isOnBridge") == false || bridgeCounterHop == 0)
            {
                if (animator.GetBool("isJumping") || animator.GetBool("isFalling"))
                    squashStretchAnimator.SetTrigger("squash");

                bridgeCounterHop++;
            }

            if (isShooting) rigidbody.gravityScale = startGravityScale;

            animator.SetBool("isJumping", false);
            animator.SetBool("isSliding", false);

            if (isSliding)
                transform.localScale =
                    new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

            isSliding             = false;
            isJumping             = false;
            isRollingMidAir       = false;
            isJumpStored          = false;
            hasRolledWhileSliding = false;
            isRollingBeforeJump   = false;

            playerParticles.StopSlideParticles();
        }

        if (collision.gameObject.layer == collisionLayer)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit")) { return; }

            if (collision.gameObject.CompareTag(Tag.SmartEnemyTag)) { return; }
            else if (collision.gameObject.CompareTag(Tag.TowerTag)) { return; }

            if (hitBySlime)
                if (slimeAttackTimer < slimeAttackDuration)
                    return;

            hitBySlime = true;

            HitAnimation();

            CameraShakeController.Instance.GenericCameraShake();

            if (AssistController.IsInvicibility == false)
            {
                if (collision.gameObject.CompareTag(Tag.BossTag))
                {
                    boss = collision.gameObject.GetComponent<DemonicEyeballAttack>();

                    PlayerStats.Health -= boss.Damage() * AssistController.EnemyStrength / 100;
                }
                else
                {
                    enemy = collision.gameObject.GetComponent<Enemy>();

                    PlayerStats.Health -= enemy.Damage() * AssistController.EnemyStrength / 100;
                }
            }

            if (isJumping) interruptJump = true;
//            if (animator.GetBool("isJumping") != true)
//            {
            isHitWhileJumping  = true;
            rigidbody.velocity = new Vector2(0f, 0f);
            rigidbody.velocity = throwBackForce;
//            }

            if (PlayerStats.Health <= 0)
            {
                if (PlayerStats.IsPendantReady == false)
                    DeathAnimation();
                else
                {
                    PlayerStats.RestoreHealthFromPendant();
                    AudioController.Instance.ReviveSFX();
                }
            }

            HealthBar.UpdateHealthBarStatus();
        }
        else if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazard")))
        {
            animator.SetBool("isClimbing", false);
            animator.SetBool("isRolling",  false);
            animator.SetBool("isJumping",  false);
            animator.SetBool("isSliding",  false);

            playerParticles.StopSlideParticles();

            if (hitByHazard)
                if (hazardDamageTimer < hazardDamageDuration)
                    return;

            hitByHazard = true;

            rigidbody.velocity = Vector2.zero;
            rigidbody.velocity = throwBackForce;

            if (collision.gameObject.CompareTag(Tag.LavaTag))
                PlayerStats.Health -= PlayerStats.Health;
            else
            {
                if (AssistController.IsInvicibility == false)
                    PlayerStats.Health -= hazardDamage * AssistController.HazardDamage / 100;
            }

            HitAnimation();

            CameraShakeController.Instance.GenericCameraShake();

            if (PlayerStats.Health <= 0)
            {
                if (PlayerStats.IsPendantReady == false || collision.gameObject.CompareTag(Tag.LavaTag))
                    DeathAnimation();
                else
                {
                    PlayerStats.RestoreHealthFromPendant();
                    AudioController.Instance.ReviveSFX();
                }
            }

            HealthBar.UpdateHealthBarStatus();
        }
    }

    private void CheckForWallClimb(Collision2D collision)
    {
        if (headCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))
            && !feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            isWallJumping = true;
            isSliding     = true;
            isHoldingJump = false;

            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);

            if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
            {
                animator.SetBool("isSliding", true);
                playerParticles.PlaySlideParticles();
            }
        }
        else
            isWallJumping = false;
    }

    private void SetIsTeleportingToFalse() //fired from animation event
    {
        isTeleporting = false;
    }

    private void SetIsBeingHitToFalse() //fired from animation event
    {
        isBeingHit = false;
        animator.SetBool("isHit", false);
    }

    #region animations

    private void RunAnimation()
    {
        bool hasPlayerSpeed = Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon;

        switch (hasPlayerSpeed)
        {
            case true:
                runningState = RunningState.Running;
                break;
            case false:
                runningState = RunningState.Stopped;
                break;
        }

        animator.SetBool("isRunning", hasPlayerSpeed);
    }

    private void ClimbAnimation()
    {
        bool hasPlayerVerticalSpeed = Mathf.Abs(rigidbody.velocity.y) > Mathf.Epsilon;

        animator.SetBool("isClimbing", hasPlayerVerticalSpeed);
    }

    private void DeathAnimation()
    {
        isDeactivated               = true;
        bodyCollider.enabled        = false;
        rigidbody.bodyType          = RigidbodyType2D.Static;
        isAlive                     = false;
        bodyCollider.sharedMaterial = gluey;
        animator.SetTrigger("hasDied");

        Physics2D.IgnoreLayerCollision(collisionLayer, playerLayer, true);
    }

    public void TeleportAnimation()
    {
        rigidbody.gravityScale = 0f;
        Vector2 stillVelocity = Vector2.zero;
        rigidbody.velocity    = stillVelocity;
        rigidbody.isKinematic = true;

        bodyCollider.enabled = false;
        feetCollider.enabled = false;
        headCollider.enabled = false;

        animator.SetTrigger("isTeleporting");

        isTeleporting = true;
    }

    public void HitAnimation()
    {
        if (animator.GetBool("isShooting")) animator.SetBool("isShooting", false);

        rigidbody.gravityScale = startGravityScale;
        animator.SetBool("isHit", true);
    }

    public void JumpAnimation() { animator.SetBool("isJumping", true); }

    public void RollingAnimation()
    {
        animator.SetBool("isRolling", false);
        animator.SetBool("isRolling", true);
        squashStretchAnimator.SetTrigger("squash");

        RotateAfterImage();
        RollSoundSFX();

        Physics2D.IgnoreLayerCollision(collisionLayer, playerLayer, true);

        CameraShakeController.Instance.GenericCameraShake();

        GameManager.rippleEffect.SetNewRipplePosition(GameManager.mainCamera.WorldToScreenPoint(transform.position));

        if (AssistController.IsInfiniteDash == false) PlayerStats.ReduceStamina(rollStaminaRequired);

        StaminaBar.UpdateStaminaBarStatus();

        rollDirection = transform.localScale.x;

        if (InputControl.GetAxisRaw("Horizontal") != 0f ||
            InputControl.GetAxisRaw("Vertical")   != 0f)
            dir = new Vector2(InputControl.GetAxisRaw("Horizontal"),
                              InputControl.GetAxisRaw("Vertical"));
        else
            dir = new Vector2(rollDirection, rigidbody.velocity.y);

        if (dir.y != 0f && isRollingBeforeJump == false) isRollingBeforeJump = true;

        Vector2 playerVelocity = dir.normalized * rollingSpeed;
        rigidbody.velocity = Vector2.zero;
        rigidbody.velocity = playerVelocity;

        StartCoroutine(RollWait());
    }

    private IEnumerator RollWait()
    {
        if (_ghostTrail == null) _ghostTrail = Instantiate(_ghostGameObject).GetComponent<GhostTrail>();

        _ghostTrail.ShowGhost();
        _ghostTrailAttached.ShowGhost();

        DOVirtual.Float(20, 0, .3f, RigidbodyDrag);

        yield return new WaitForSeconds(.3f);
    }

    public void StopAllAnimations()
    {
        animator.SetBool("isRunning",  false);
        animator.SetBool("isClimbing", false);
        animator.SetBool("isRolling",  false);
        animator.SetBool("isJumping",  false);
        animator.SetBool("isSliding",  false);

        playerParticles.StopSlideParticles();
    }

    void RigidbodyDrag(float x) { rigidbody.drag = x; }

    #endregion

    public void HitByBat(int damage, bool isSpecialTreasuerBat)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit")) { return; }

        if (rigidbody.bodyType == RigidbodyType2D.Static) { return; }

        if (hitByBat == false)
            hitByBat = true;
        else
            return;

        HitAnimation();
        TakeDamage(damage - (damage * (PlayerStats.Armor * PlayerStats.ArmorMultiplier) / 100) *
                   AssistController.EnemyStrength / 100);

        CameraShakeController.Instance.GenericCameraShake();

        if (animator.GetBool("isJumping") != true) { rigidbody.velocity = throwBackForce; }
        else if (animator.GetBool("isJumping") && isSpecialTreasuerBat) rigidbody.velocity = throwBackForce;

        if (PlayerStats.Health <= 0)
        {
            if (PlayerStats.IsPendantReady == false)
                DeathAnimation();
            else
            {
                PlayerStats.RestoreHealthFromPendant();
                AudioController.Instance.ReviveSFX();
            }
        }

        HealthBar.UpdateHealthBarStatus();
    }

    public void HitByMeleEnemy(int damage)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit")) { return; }

        if (rigidbody.bodyType == RigidbodyType2D.Static) { return; }

        if (hitByMeleEnemy == false)
            hitByMeleEnemy = true;
        else
            return;

        HitAnimation();
        TakeDamage((damage - (damage * (PlayerStats.Armor * PlayerStats.ArmorMultiplier) / 100)) *
                   AssistController.EnemyStrength / 100);

        CameraShakeController.Instance.GenericCameraShake();

        if (animator.GetBool("isJumping") != true)
        {
            if (rigidbody.bodyType != RigidbodyType2D.Static)
            {
                rigidbody.velocity = new Vector2(0f, 0f);
                rigidbody.velocity = throwBackForce;
            }
        }

        if (PlayerStats.Health <= 0)
        {
            if (PlayerStats.IsPendantReady == false)
                DeathAnimation();
            else
            {
                PlayerStats.RestoreHealthFromPendant();
                AudioController.Instance.ReviveSFX();
            }
        }

        HealthBar.UpdateHealthBarStatus();
    }

    public void HitByHazards(int damage)
    {
        if (rigidbody.bodyType == RigidbodyType2D.Static) { return; }

        animator.SetBool("isClimbing", false);
        animator.SetBool("isRolling",  false);
        animator.SetBool("isJumping",  false);
        animator.SetBool("isSliding",  false);

        playerParticles.StopSlideParticles();

        HitAnimation();
        TakeDamage((damage - (damage * (PlayerStats.Armor * PlayerStats.ArmorMultiplier) / 100)) *
                   AssistController.HazardDamage / 100);

        CameraShakeController.Instance.GenericCameraShake();

        rigidbody.velocity = new Vector2(0f, 0f);
        rigidbody.velocity = throwBackForce;

        if (PlayerStats.Health <= 0)
        {
            if (PlayerStats.IsPendantReady == false)
                DeathAnimation();
            else
            {
                PlayerStats.RestoreHealthFromPendant();
                AudioController.Instance.ReviveSFX();
            }
        }

        HealthBar.UpdateHealthBarStatus();
    }

    public void HitByLaser(int damage)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Roll")) { return; }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit")) { return; }

        if (rigidbody.bodyType == RigidbodyType2D.Static) { return; }

        playerParticles.PlayHitParticles();
        playerParticles.PlayLaserHitParticles();

        TakeDamage((damage - (damage * (PlayerStats.Armor * PlayerStats.ArmorMultiplier) / 100)) *
                   AssistController.EnemyStrength / 100);

        cameraShaker.ShakeCamera(0.05f, 0.7f, 10f);

        if (PlayerStats.Health <= 0)
        {
            if (PlayerStats.IsPendantReady == false)
                DeathAnimation();
            else
            {
                PlayerStats.RestoreHealthFromPendant();
                AudioController.Instance.ReviveSFX();
            }
        }

        HealthBar.UpdateHealthBarStatus();
    }

    private void TakeDamage(float damage)
    {
        if (AssistController.IsInvicibility == false && isImmuneToProjectiles == false) PlayerStats.Health -= damage;
    }

    public void InstaKill()
    {
        HitAnimation();

        PlayerStats.Health = 0;

        CameraShakeController.Instance.GenericCameraShake();

        if (PlayerStats.Health <= 0) DeathAnimation();

        HealthBar.UpdateHealthBarStatus();
    }

    public void AllowRolling() // called from PlayerMushroomJump
    {
        isRollingMidAir       = false;
        isJumpingOverMushroom = true;
        isJumping             = true;
    }

    public void SetCanJumpToTrue() //called from hit animation event
    {
        canJump = true;
    }

    public void SetCanJumpToFalse() //called from hit animation event
    {
        canJump = false;
    }

    public void SetIsRollingToFalse()
    {
        if (PlayerStats.IsRolling)
        {
            PlayerStats.IsRolling  = false;
            rigidbody.gravityScale = startGravityScale;
        }
    }

    public void RotateAfterImage() // called from rolling animation
    {
        //afterimageRotation.Rotate(transform);
    }

    private void ShowPopUp() // fired from (hit) animation event
    {
        playerPopup.EnablePopUp();
    }

    public void WalkSoundSFX() // called from (run) animation event
    {
        AudioController.Instance.WalkSFX();
    }

    public void JumpSoundSFX() { AudioController.Instance.JumpSFX(); }

    public void RollSoundSFX() // called from (roll) animation event
    {
        AudioController.Instance.RollSFX();
    }

    public void TeleportSoundSFX() // called from (portalin) animation event
    {
        AudioController.Instance.TeleportSFX();
    }

    public void HitSoudSFX() // called from (hit) animation event
    {
        AudioController.Instance.HitSFX();
    }

    public void DeathSoundSFX() // called from (death) animation event
    {
        AudioController.Instance.StopAllMusic();
        AudioController.Instance.DeathSFX();
        AudioController.Instance.ResetMusicID();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(crateRangeX, crateRangeY, 1f));
    }

    public void CancelInvokeRolling() { CancelInvoke(nameof(SetIsRollingToFalse)); }
}