using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;

public class DemonicEyeBallHealth : MonoBehaviour
{
    [SerializeField] private int health = 1000;

    private int   maxBossHealth;
    private float healthChecker;
    private float delayTime;

    private AnimationClip[]         clips;
    private DemonicEyeballAttack    demonicAttack;
    private HitHandler              hitHandler;
    private BossHealthbarController bossHealthbarController;
    private DemonicEyeballMovement _demonicEyeballMovement;

    [SerializeField] bool hasShield;

    [Header("Destroy Boss Timer")]
    [SerializeField] private float destroyBossTimer = 3f;

    [Header("Shield Object")]
    [SerializeField]
    private GameObject shield;

    [Header("Artifact")]
    [SerializeField]
    private GameObject artifact;

    [Header("Entry Door")]
    [SerializeField]
    private GameObject entryDoor;
    
    [Header("Ghost Code")]
    [SerializeField]
    private GameObject _ghostGameObject;

    [SerializeField] private GhostTrail _ghostTrailAttached;

    private GhostTrail _ghostTrail;
    
    private CameraShakeInstance _shaker;

    [Header("Entry Door")]
    [SerializeField]
    private LevelExit _entryDoor;
    

    private void Awake ()
    {
        maxBossHealth = health;
        hitHandler    = GetComponent<HitHandler>();
        _demonicEyeballMovement = GetComponent<DemonicEyeballMovement>();

        bossHealthbarController = FindObjectOfType<BossHealthbarController>();

        bossHealthbarController.gameObject.SetActive(false); // disable healthbar initially
    }

    public void TakeDamage (int damageAmount)
    {
        AudioController.Instance.EnemyHitSFX();

        if (hasShield) { return; }

        if (health < damageAmount)
            health -= health;
        else
            health -= damageAmount;

        CheckForDead();

        healthChecker = (float) health / (float) maxBossHealth;

        bossHealthbarController.UpdateBossHealth(healthChecker); // updates healthbar UI

        if (healthChecker <= .5 && BossFightController.objectCount == 2)
        {
            BossFightController.EnableObstacle();
            demonicAttack.ChangeAttackType(DemonicEyeballAttack.AttackType.SixFold);
        }
        else if (healthChecker <= .7 && BossFightController.objectCount == 1)
        {
            BossFightController.EnableObstacle();
            demonicAttack.ChangeAttackType(DemonicEyeballAttack.AttackType.Threefold);

            if (demonicAttack.enableLaserAttack == false) demonicAttack.EnableLaserBeamAttack();
        }
        else if (healthChecker <= .85 && BossFightController.objectCount == 0) BossFightController.EnableObstacle();
    }

    private void CheckForDead ()
    {
        if (health <= 0 && GameManager.IsBossAnimationPlaying == false)
        {
            GameManager.player.isImmuneToProjectiles = true;
            GameManager.Instance.HitStopEffect();
            //GameManager.Instance.LoadEscapeLevel();
            AudioController.Instance.StopAllMusic();
            _entryDoor.DisableDoor();

            bossHealthbarController.gameObject.SetActive(false);

            GameManager.rippleEffect.SetNewRipplePosition(GameManager
                                                          .mainCamera.WorldToScreenPoint(transform.position));
            
            entryDoor.GetComponent<Collider2D>().enabled = false;

            InitiateDeathSequence();
        }
    }

    public void SetDemonicAttackObject (DemonicEyeballAttack newDemonicAttack) { demonicAttack = newDemonicAttack; }

    public void ActivateShield ()
    {
        shield.SetActive(true);
        hasShield = true;
    }

    public void DisableShield ()
    {
        shield.SetActive(false);
        hasShield = false;
    }

    private void InitiateDeathSequence ()
    {
        _demonicEyeballMovement.isDeathAnimationActivated = true;
        demonicAttack.StopAllAttacks();
        GameManager.cameraTransition.ShowBars();
        GameManager.AreControlsEnabled = false;
        GameManager.IsBossAnimationPlaying = true;

        _shaker = CameraShakeController.Instance.FinalBossEntryCameraShake();
        _shaker.StartFadeOut(3f);
        PostProcessingController.Instance.EnableChromaticAberration();
        
        StartCoroutine(DestroyBossObject());
    }

    private IEnumerator DestroyBossObject ()
    {
        for (int i = 0; i < destroyBossTimer * 2; i++)
        {
            if (_ghostTrail == null) _ghostTrail = Instantiate(_ghostGameObject).GetComponent<GhostTrail>();

            _ghostTrail.ShowGhostFinalBoss(transform.position);
            _ghostTrailAttached.ShowGhostFinalBoss(transform.position);

            yield return new WaitForSeconds(.5f);
        }
        
        GameManager.cameraTransition.HideBars();
        PostProcessingController.Instance.DisableChromaticAberration();
        GameManager.AreControlsEnabled     = true;
        GameManager.IsBossAnimationPlaying = false;
        _demonicEyeballMovement.isDeathAnimationActivated = false;
        hitHandler.PlayDeathParticles();
        
        artifact.transform.parent = null;
        artifact.SetActive(true);
        artifact.GetComponent<Artifact>().FollowPlayer();
        artifact.GetComponent<Artifact>().EnableDoor();
        
        Destroy(gameObject, delayTime);
    }
}