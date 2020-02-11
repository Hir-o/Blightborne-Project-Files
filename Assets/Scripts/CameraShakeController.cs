using System;
using EZCameraShake;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    public static CameraShakeController Instance;

    [Header("Roll Shake Values")]
    [SerializeField]
    private float shakeMagnitude = .8f;

    [SerializeField] private float shakeRoughness   = .8f;
    [SerializeField] private float shakeFadeInTime  = .1f;
    [SerializeField] private float shakeFadeOutTime = .2f;

    [Header("Fire Shot Shake Values")]
    [SerializeField]
    private float fireShotshakeMagnitude = .8f;

    [SerializeField] private float fireShotshakeRoughness   = .8f;
    [SerializeField] private float fireShotshakeFadeInTime  = .1f;
    [SerializeField] private float fireShotshakeFadeOutTime = .2f;

    [Header("Lever Shake Values")]
    [SerializeField]
    private float leverShakeMagnitude = .8f;

    [SerializeField] private float leverShakeRoughness   = .8f;
    [SerializeField] private float leverShakeFadeInTime  = .1f;
    [SerializeField] private float leverShakeFadeOutTime = .2f;

    [Header("Bat Boss Shake Values")]
    [SerializeField]
    private float batBossShakeMagnitude = .8f;

    [SerializeField] private float batBossShakeRoughness   = .8f;
    [SerializeField] private float batBossShakeFadeInTime  = .1f;
    [SerializeField] private float batBossShakeFadeOutTime = 10f;
    
    [Header("Ogre Boss Walk Shake Values")]
    [SerializeField]
    private float ogreBossWalkShakeMagnitude = .8f;

    [SerializeField] private float ogreBossWalkShakeRoughness   = .8f;
    [SerializeField] private float ogreBossWalkShakeFadeInTime  = .1f;
    [SerializeField] private float ogreBossWalkShakeFadeOutTime = .2f;

    [Header("Final Boss Shake Values")]
    [SerializeField]
    private float bossEntryAnimationShakeMagnitude = .8f;
    
    [SerializeField] private float bossEntryAnimationShakeRoughness = .8f;
    [SerializeField] private float bossEntryFadeInTime = .1f;
    [SerializeField] private float bossEntryFadeOutTime = 3f;
    [SerializeField] private float finalBossShakeMagnitude = .8f;
    [SerializeField] private float finalBossShakeRoughness   = .8f;
    [SerializeField] private float finalBossShakeFadeInTime  = .1f;
    [SerializeField] private float finalBossShakeFadeOutTime = 10f;

    private void Awake ()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GenericCameraShake ()
    {
        //GameManager.cameraShaker.ShakeCamera(RollDuration, RollAmplitude, RollFrequency);
        CameraShaker.Instance.ShakeOnce(shakeMagnitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);
    }
    
    public void FireShotCameraShake () // note it is used by fragments exploding not the actual fire shot
    {
        //GameManager.cameraShaker.ShakeCamera(RollDuration, RollAmplitude, RollFrequency);
        CameraShaker.Instance.ShakeOnce(fireShotshakeMagnitude, fireShotshakeRoughness, fireShotshakeFadeInTime,
                                        fireShotshakeFadeOutTime);
    }

    public void ShakeCameraLever ()
    {
        //GameManager.cameraShaker.ShakeCamera(LeverDuration, LeverAmplitude, LeverFrequency);
        CameraShaker.Instance.ShakeOnce(leverShakeMagnitude, leverShakeRoughness, leverShakeFadeInTime,
                                        leverShakeFadeOutTime);
    }

    public void BatBossCameraShake (float duration)
    {
        CameraShaker.Instance.ShakeOnce(batBossShakeMagnitude, batBossShakeRoughness, batBossShakeFadeInTime,
                                        duration + batBossShakeFadeOutTime);
    }
    
    public void OgreBossWalkShake ()
    {
        CameraShaker.Instance.ShakeOnce(ogreBossWalkShakeMagnitude, ogreBossWalkShakeRoughness, ogreBossWalkShakeFadeInTime,
                                        ogreBossWalkShakeFadeOutTime);
    }
    
    public CameraShakeInstance FinalBossEntryCameraShake ()
    {
        CameraShakeInstance shaker = CameraShaker.Instance.StartShake(bossEntryAnimationShakeMagnitude, bossEntryAnimationShakeRoughness, bossEntryFadeInTime);

        return shaker;
    }

    public void FinalBossCameraShake (float duration)
    {
        CameraShaker.Instance.ShakeOnce(finalBossShakeMagnitude, finalBossShakeRoughness, finalBossShakeFadeInTime,
                                        duration + finalBossShakeFadeOutTime);
    }
}