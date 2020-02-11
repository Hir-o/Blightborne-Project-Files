using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using DG.Tweening;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject bowArrow;

    [Header("Special Arrow Types")]
    [SerializeField]
    GameObject powerBowArrow;

    [SerializeField] GameObject fireBowArrow;
    [SerializeField] GameObject iceBowArrow;
    [SerializeField] GameObject specialBowArrow;

    [SerializeField] float bowYOffset = .02f;
    [SerializeField] float bowXOffset = 1f;

    [Header("Stamina Required for Attack Types")]
    [SerializeField]
    float staminaRequired = 20f;

    [SerializeField] float staminaRequiredIce               = 50f;
    [SerializeField] float staminaRequiredFire              = 75f;
    [SerializeField] float staminaRequiredSpecialPercentage = 80f;

    [Header("Animation 'Time' Variables")]
    [SerializeField]
    float floatingTime = .2f;

    [SerializeField] float shootWaitTime = .2f;

    private float startShootWaitTime = .2f;

    private Animator                animator;
    private Rigidbody2D             rigidbody;
    private BoxCollider2D           feetCollider;
    private CircleCollider2D        circleCollider;
    private Player                  player;
    private CinemachineCameraShaker cameraShaker;
    private PopUpController         playerPopup;

    private bool jumpShoot;
    private bool isShooting;

    private Vector2 stillVelocity;
    private Vector3 arrowOffset;

    [Header("Power Arrow Charged Icon")]
    [SerializeField]
    private GameObject powerArrowIcon;

    private int arrowCounter = 1;

    private void Start()
    {
        shootWaitTime = shootWaitTime -
                        (shootWaitTime * (PlayerStats.AttackSpeed * PlayerStats.AttackSpeedMultiplier) / 100);

        animator       = GetComponent<Animator>();
        rigidbody      = GetComponent<Rigidbody2D>();
        feetCollider   = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        player         = GetComponent<Player>();
        playerPopup    = GetComponent<PopUpController>();
        cameraShaker   = FindObjectOfType<CinemachineCameraShaker>();

        powerArrowIcon.SetActive(false);
    }

    private void LateUpdate()
    {
        if (GameManager.AreControlsEnabled == false) { return; }

        if (player.GetIsAlive() == false) { return; }

        if (player.GetIsTeleporting()) { return; }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit")) { return; }

        if (animator.GetBool("isShooting")) { return; }

        ShootArrow();
    }

    private void ShootArrow()
    {
        if (player.isDeactivated) { return; }

        if (GameManager.hud.abilityPanel.activeSelf) { return; }

        if (GameManager.menu.gameObject.activeSelf) { return; }

        //if (animator.GetBool("isClimbing")) { return; }

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) jumpShoot = false;

        if (isShooting) { return; }

        if (InputControl.GetButtonDown("Fire1"))
        {
            if (PlayerStats.Stamina < staminaRequired)
            {
                AudioController.Instance.NoEnergySFX(playerPopup);
                return;
            }

            if (PlayerStats.Stamina < staminaRequiredIce && PlayerStats.CurrentAbilitySelctedIndex == 1 &&
                PlayerStats.UnlockIceArrow)
            {
                AudioController.Instance.NoEnergySFX(playerPopup);
                return;
            }

            if (PlayerStats.Stamina < staminaRequiredFire && PlayerStats.CurrentAbilitySelctedIndex == 2 &&
                PlayerStats.UnlockFireArrow)
            {
                AudioController.Instance.NoEnergySFX(playerPopup);
                return;
            }

            if ((PlayerStats.Stamina <= (PlayerStats.TotalStamina * staminaRequiredSpecialPercentage) / 100f) &&
                PlayerStats.CurrentAbilitySelctedIndex == 3                                                   &&
                PlayerStats.UnlockSpecialArrow)
            {
                AudioController.Instance.NoEnergySFX(playerPopup);
                return;
            }
            else if (PlayerStats.Health                     <= PlayerStats.TotalHealth / 2f &&
                     PlayerStats.CurrentAbilitySelctedIndex == 3                            &&
                     PlayerStats.UnlockSpecialArrow)
            {
                AudioController.Instance.NoHPSFX(playerPopup);
                return;
            }

            if (arrowCounter == 2 && PlayerStats.UnlockPowerArrow) powerArrowIcon.SetActive(true);

            shootWaitTime = startShootWaitTime -
                            (startShootWaitTime * (PlayerStats.AttackSpeed * PlayerStats.AttackSpeedMultiplier) / 100);

            animator.SetBool("isJumping", false);

            player.IsShooting(true);
            animator.SetBool("isShooting", true);

            stillVelocity      = new Vector2(0f, 0f);
            rigidbody.velocity = stillVelocity;
            player.CancelInvokeRolling();


            isShooting = true;

            if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) &&
                !feetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
                jumpShoot = true;
            else
                jumpShoot = false;

            if (jumpShoot) rigidbody.gravityScale = .05f;

            StartCoroutine(InstantiateArrow());
            StartCoroutine(ShakeCamera());
        }
    }

    private IEnumerator ShakeCamera()
    {
        yield return new WaitForSeconds(.12f);

        CameraShakeController.Instance.GenericCameraShake();
    }

    private IEnumerator InstantiateArrow()
    {
        if (animator.GetBool("isRolling")) animator.SetBool("isRolling", false);

        yield return new WaitForSeconds(shootWaitTime);

        arrowOffset = new Vector3(
                                  transform.position.x +
                                  transform.localScale.x * bowXOffset, //the offset with direction on x
                                  transform.position.y - bowYOffset,
                                  0f
                                 );

        if (PlayerStats.CurrentAbilitySelctedIndex == 0)
        {
            if (arrowCounter == 3 && PlayerStats.UnlockPowerArrow)
            {
                Instantiate(powerBowArrow, arrowOffset, Quaternion.identity);
                arrowCounter = 0;
                PlayerStats.ReduceStamina(staminaRequired);
                StaminaBar.UpdateStaminaBarStatus();
                powerArrowIcon.SetActive(false);
                AudioController.Instance.PowerShotSFX();
            }
            else
            {
                Instantiate(bowArrow, arrowOffset, Quaternion.identity);

                PlayerStats.ReduceStamina(staminaRequired);
                StaminaBar.UpdateStaminaBarStatus();

                AudioController.Instance.ShootSFX();
            }
        }
        else if (PlayerStats.CurrentAbilitySelctedIndex == 1 && PlayerStats.UnlockIceArrow)
        {
            if (PlayerStats.Stamina >= staminaRequiredIce)
            {
                Instantiate(iceBowArrow, arrowOffset, Quaternion.identity);
                PlayerStats.ReduceStamina(staminaRequiredIce);
                StaminaBar.UpdateStaminaBarStatus();
                AudioController.Instance.IceShotSFX();
            }
        }
        else if (PlayerStats.CurrentAbilitySelctedIndex == 2 && PlayerStats.UnlockFireArrow)
        {
            if (PlayerStats.Stamina >= staminaRequiredFire)
            {
                Instantiate(fireBowArrow, arrowOffset, Quaternion.identity);
                PlayerStats.ReduceStamina(staminaRequiredFire);
                StaminaBar.UpdateStaminaBarStatus();
                AudioController.Instance.FireShotSFX();
            }
        }
        else if (PlayerStats.CurrentAbilitySelctedIndex == 3 && PlayerStats.UnlockSpecialArrow)
        {
            if (PlayerStats.Health >= PlayerStats.TotalHealth / 2f &&
                (PlayerStats.Stamina >= (PlayerStats.TotalStamina * staminaRequiredSpecialPercentage) / 100f))
            {
                Instantiate(specialBowArrow, arrowOffset, Quaternion.identity);

                PlayerStats.Stamina -= (PlayerStats.TotalStamina * staminaRequiredSpecialPercentage) / 100f;
                StaminaBar.UpdateStaminaBarStatus();
                AudioController.Instance.SpecialShotSFX();
            }
        }

        if (PlayerStats.CurrentAbilitySelctedIndex == 0 && PlayerStats.UnlockPowerArrow) arrowCounter++;

        StartCoroutine(SetShootingToFalse());
    }

    private IEnumerator SetShootingToFalse() //called through animation event
    {
        yield return new WaitForSeconds(floatingTime);

        animator.SetBool("isShooting", false);
        player.IsShooting(false);
        rigidbody.gravityScale = .7f;

        isShooting = false;
    }

    public void SetPowerIcon(GameObject newPowerArrowIcon) { powerArrowIcon = newPowerArrowIcon; }

    public void InterruptShooting()
    {
        StopAllCoroutines();
        animator.SetBool("isShooting", false);
        isShooting = false;
    }
}