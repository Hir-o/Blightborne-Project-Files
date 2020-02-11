using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;

public class DemonicEyeballAttack : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPosition;

    private DemonicEyeballMovement _demonicEyeballMovement;
    private Player     player;
    private GameObject tempProjectile;
    private Vector3    playerFlatPosition;

    [Header("Attack Type")]
    [SerializeField]
    private float startShootingTimer = 2f;

    [SerializeField] private float      shootReloadTimer = 6f;
    [SerializeField] private GameObject projectile;

    public enum AttackType
    {
        Directional,
        Missle,
        Threefold,
        SixFold
    }

    public AttackType attackType = AttackType.Missle;

    [SerializeField] private float startingRotationAngleThreeFold = 45f;
    [SerializeField] private float startingRotationAngleSixFold   = 60f;

    public bool isAttacking, isShootingLaser;

    [Header("Missile Extra Attributes")]
    [SerializeField]
    private float misslieSpeed = 2f;

    [SerializeField] private float missileDestroyTimer = 8f;
    [SerializeField] private float missileReloadTimer  = 6f;

    [Header("Body Collision Damage")]
    [SerializeField]
    private int collisionDamage = 10;

    [Header("Laser Beam Attack")]
    public bool enableLaserAttack;

    [SerializeField] private float laserAttackInterval = 8f;

    [Header("Bullet Spread")]
    [SerializeField]
    private float threeFoldAttackSpread = 30f;

    [SerializeField] private float sixFoldAttackSpread = 20f;

    [Header("Ghost Code")]
    [SerializeField]
    private GameObject _ghostGameObject;

    [SerializeField] private GhostTrail _ghostTrailAttached;

    [SerializeField] private BossHealthbarController bossHealthbarController;

    private GhostTrail _ghostTrail;

    private float tempRotationAngle;

    private Animator            animator;
    private CameraShakeInstance _shaker;
    private PopUpController     _popUpController;

    private void Awake ()
    {
        animator         = GetComponent<Animator>();
        _popUpController = GetComponent<PopUpController>();
        _demonicEyeballMovement = GetComponent<DemonicEyeballMovement>();
    }

    // called from DemonicEyeMovement script when player gets into the attack zone of the boss
    public void InitializeAttack (Player tempPlayer)
    {
        isAttacking = true;
        player      = tempPlayer;

        GameManager.cameraTransition.ShowBars();
        GameManager.AreControlsEnabled     = false;
        GameManager.IsBossAnimationPlaying = true;

        _shaker = CameraShakeController.Instance.FinalBossEntryCameraShake();
        PostProcessingController.Instance.EnableChromaticAberration();

        Invoke(nameof(FinishEntryAnimation), 3f);
        StartCoroutine(Roar());
    }

    private void FinishEntryAnimation ()
    {
        StopCoroutine(Roar());
        GameManager.cameraTransition.HideBars();
        _shaker.StartFadeOut(1f);
        PostProcessingController.Instance.DisableChromaticAberration();
        GameManager.AreControlsEnabled     = true;
        GameManager.IsBossAnimationPlaying = false;
        InvokeRepeating(nameof(Attack), startShootingTimer, shootReloadTimer);

        if (bossHealthbarController.gameObject.activeSelf == false) bossHealthbarController.gameObject.SetActive(true);
    }

    private IEnumerator Roar ()
    {
        for (int i = 0; i < Mathf.RoundToInt(3f) * 2; i++)
        {
            _popUpController.FinalBossRoarPopUp();

            if (_ghostTrail == null) _ghostTrail = Instantiate(_ghostGameObject).GetComponent<GhostTrail>();

            _ghostTrail.ShowGhostFinalBoss(transform.position);
            _ghostTrailAttached.ShowGhostFinalBoss(transform.position);

            yield return new WaitForSeconds(.5f);
        }
    }

    private void Attack ()
    {
        if (player == null) { return; }

        if (_demonicEyeballMovement.isDeathAnimationActivated) { return; }

        animator.SetTrigger("attack");

        playerFlatPosition = new Vector3(player.transform.position.x, player.transform.position.y, 0f);

        if (attackType == AttackType.Threefold)
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
        else if (attackType == AttackType.SixFold)
        {
            tempRotationAngle = startingRotationAngleSixFold;
            for (int i = 0; i < 6; i++)
            {
                tempProjectile = Instantiate(projectile, projectileSpawnPosition.position, Quaternion.identity);
                tempProjectile.transform.LookAt(playerFlatPosition);
                tempProjectile.transform.Rotate(tempRotationAngle, 0f, 0f);
                tempRotationAngle -= sixFoldAttackSpread;
            }
        }
        else if (attackType == AttackType.Missle)
        {
            tempProjectile =
                Instantiate(projectile, projectileSpawnPosition.position, Quaternion.identity);
            tempProjectile.GetComponent<WardProjectile>().projectileSpeed = misslieSpeed;
            tempProjectile.GetComponent<WardProjectile>().destroyTimer    = missileDestroyTimer;
            shootReloadTimer                                              = missileReloadTimer;
            tempProjectile.transform.LookAt(playerFlatPosition);
            tempProjectile.GetComponent<WardProjectile>().target = player.transform;
        }
        else if (attackType == AttackType.Directional)
        {
            tempProjectile = Instantiate(projectile, projectileSpawnPosition.position, Quaternion.identity);
            tempProjectile.transform.LookAt(playerFlatPosition);
        }
    }

    public void EnableLaserBeamAttack ()
    {
        if (_demonicEyeballMovement.isDeathAnimationActivated) { return; }
        
        enableLaserAttack = true;
        InvokeRepeating(nameof(LaserBeamAttack), 0f, laserAttackInterval);
    }

    private void LaserBeamAttack ()
    {
        if (_demonicEyeballMovement.isDeathAnimationActivated) { return; }
        
        animator.SetTrigger("laserAttack");
        isShootingLaser = true;
        PostProcessingController.Instance.EnableChromaticAberration();
        CameraShakeController.Instance.FinalBossCameraShake(laserAttackInterval);
        StartCoroutine(LaserChargedUp());
    }
    
    private IEnumerator LaserChargedUp ()
    {
        if (_demonicEyeballMovement.isDeathAnimationActivated) { yield return null; }
        
        for (int i = 0; i < 8; i++) //hardcoded number
        {
            if (_ghostTrail == null) _ghostTrail = Instantiate(_ghostGameObject).GetComponent<GhostTrail>();

            _ghostTrail.ShowGhostFinalBoss(transform.position);
            _ghostTrailAttached.ShowGhostFinalBoss(transform.position);

            yield return new WaitForSeconds(.5f);
        }
    }

    public void ChangeAttackType (AttackType newAttackType) { attackType = newAttackType; }

    public void ChangeShootReloadTimer (float newTimer) { shootReloadTimer = newTimer; }

    public void CancelInvokeAttack () // called from animation event just before starting to charge laser
    {
        CancelInvoke(nameof(Attack));
    }

    public void StartInvokeAttack () // called from animation event after laser has been shot, also called in start
    {
        StopCoroutine(LaserChargedUp());
        InvokeRepeating(nameof(Attack), startShootingTimer, shootReloadTimer);
    }

    public void PlayLaserBeamSFX () // called from animation (laserAttack) event
    {
        AudioController.Instance.LaserSFX();
    }

    public void StopLaserBeamSFX () // called from animation (laserAttack) event
    {
        AudioController.Instance.StopLaserSFX();
        isShootingLaser = false;
        PostProcessingController.Instance.DisableChromaticAberration();
    }

    public void StopAllAttacks ()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    #region getters

    public int Damage ()
    {
        return Mathf.RoundToInt(collisionDamage -
                                (collisionDamage * (PlayerStats.Armor * PlayerStats.ArmorMultiplier) / 100));
    }

    #endregion
}