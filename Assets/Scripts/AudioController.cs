using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;
using Random = UnityEngine.Random;

public class AudioController : Singleton<AudioController>
{
    [Header("Fade In/Out Values and Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float musicVolume = .5f;

    [Range(0f, 1f)] [SerializeField] private float sfxVolume = .5f;
    [SerializeField]                 private float fadeIn    = 2f;
    [SerializeField]                 private float fadeOut   = 2f;

    [Header("Escape theme volume and fades")]
    [Range(0f, 1f)]
    [SerializeField]
    private float escapeMusicVolume = .5f;

    [SerializeField] private float fadeInEscape  = 1f;
    [SerializeField] private float fadeOutEscape = 2f;

    [Header("Player SFX Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float walkSFXVolume = .3f;

    [Range(0f, 1f)] [SerializeField] private float powerShotSFXVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float jumpSFXVolume      = .3f;

    [Header("Interactables SFX Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float coinSFXVolume = .3f;

    [Range(0f, 1f)] [SerializeField] private float blipSFXVolume       = .4f;
    [Range(0f, 1f)] [SerializeField] private float checkpointSFXVolume = .3f;
    [Range(0f, 1f)] [SerializeField] private float gateSFXVolume       = .8f;
    [Range(0f, 1f)] [SerializeField] private float rollSFXVolume       = .8f;

    [Header("Boss SFX Volume and Fade In/Out")]
    [Range(0f, 1f)]
    [SerializeField]
    private float bossLaserSFXVolume = 1f;

    [SerializeField] private float fadeInBoss  = 3f;
    [SerializeField] private float fadeOutBoss = 3f;

    [Header("UI SFX Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float UISFXVolume = .3f;

    [Header("Music Themes")]
    [SerializeField]
    private AudioClip randTheme;

    [SerializeField] private AudioClip firstDungeonTheme;
    [SerializeField] private AudioClip secondDungeonTheme;
    [SerializeField] private AudioClip escapeTheme;
    [SerializeField] private AudioClip puzzleTheme;
    [SerializeField] private AudioClip miniBossTheme;
    [SerializeField] private AudioClip bossTheme;

    [Header("Player SFX")]
    [SerializeField]
    private AudioClip[] walkSFX;

    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip rollSFX;
    [SerializeField] private AudioClip reviveSFX;
    [SerializeField] private AudioClip teleportSFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip deathSFX;

    [Header("Arrow SFX")]
    [SerializeField]
    private AudioClip shootSFX;

    [SerializeField] private AudioClip powerShotSFX;
    [SerializeField] private AudioClip iceShotSFX;
    [SerializeField] private AudioClip fireShotSFX;
    [SerializeField] private AudioClip specialShotSFX;
    [SerializeField] private AudioClip fragmentSFX;
    [SerializeField] private AudioClip noEnergySFX;

    [Header("Item and Interactable SFX")]
    [SerializeField]
    private AudioClip doorOpenSFX;

    [SerializeField] private AudioClip doorSpecialOpenSFX; //boss & puzzle gates
    [SerializeField] private AudioClip chestSFX;
    [SerializeField] private AudioClip coinSFX;
    [SerializeField] private AudioClip mushroomSFX;
    [SerializeField] private AudioClip collectSFX; //keys, gems
    [SerializeField] private AudioClip openPortalSFX;
    [SerializeField] private AudioClip closePortalSFX;
    [SerializeField] private AudioClip crusherSFX;
    [SerializeField] private AudioClip wallBreachSFX;
    [SerializeField] private AudioClip gateSFX; //lever & pressure plate
    [SerializeField] private AudioClip saveGameSFX;
    [SerializeField] private AudioClip checkPointSFX;

    [Header("Dialogue SFX")]
    [SerializeField]
    private AudioClip characterTypoSFX;

    [Header("Shops (Abilities, Skills, Items) UI SFX")]
    [SerializeField]
    private AudioClip purchaseSFX; // trainer item 

    [SerializeField] private AudioClip upgradeSFX;     // abilities upgrade panel 
    [SerializeField] private AudioClip buttonClickSFX; // when button gets clicked

    [Header("Enemy SFX")]
    [SerializeField]
    private AudioClip enemyHitSFX;

    [SerializeField] private AudioClip laserSFX;

    [Header("UI SFX")]
    [SerializeField]
    private AudioClip buttonSelectUISFX;

    [SerializeField] private AudioClip buttonHoverUISFX;

    [Header("Options")]
    [SerializeField]
    private bool enableMusic = true;

    [SerializeField] private bool enableSFX = true;
    public static            bool EnableMusic, EnableSFX;

    [Header("Sounds Played")]
    [SerializeField]
    private float soundCapResetSpeed = .55f;

    [SerializeField] private float maxSoundsHit      = 3;
    [SerializeField] private float maxSoundsFragment = 3;
    [SerializeField] private float maxSoundsCoin     = 3;
    [SerializeField] private float maxSoundsEnemyHit = 3;
    [SerializeField] private float maxSoundsUI       = 2;
    [SerializeField] private float maxSoundsNoEnergy = 1;

    private float timePassed, unscaledTimePassed;

    private int soundsPlayedHit,
                soundsPlayedFragment,
                soundsPlayedCoin,
                soundsPlayedEnemyHit,
                soundsPlayedUI,
                soundPlayedNoEnergy;

    private AudioSource escapeThemeSource;
    private float       tempMusicVolume;

    //ID's
    private int musicID = -1;

    private int randThemeID,
                fDungeonThemeID = -2,
                sDungeonThemeID = -2,
                escapeThemeID   = -2,
                puzzleThemeID   = -2,
                miniBossThemeID = -2,
                bossThemeID     = -2;

    private int sfxID;

    private float globalSFXVolume, globalMusicVolume;

    private void Awake()
    {
        EnableMusic = enableMusic; //used in levelmanager to play music depending on the current level
        EnableSFX   = enableSFX;

        globalSFXVolume   = EazySoundManager.GlobalSoundsVolume;
        globalMusicVolume = EazySoundManager.GlobalMusicVolume;
    }

    private void Start()
    {
        if (EnableMusic) PlayRandTheme();
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > soundCapResetSpeed)
        {
            soundsPlayedHit      = 0;
            soundsPlayedFragment = 0;
            soundsPlayedCoin     = 0;
            soundsPlayedEnemyHit = 0;
            soundPlayedNoEnergy = 0;
            timePassed           = 0;
        }

        unscaledTimePassed += Time.unscaledDeltaTime;
        if (unscaledTimePassed > soundCapResetSpeed) soundsPlayedUI = 0;
    }

    #region Music

    public void PlayRandTheme()
    {
        if (musicID != randThemeID)
        {
            randThemeID = EazySoundManager.PlayMusic(randTheme, musicVolume, true, true, fadeIn, fadeOut);
            musicID     = randThemeID;
        }
    }

    public void PlayFirstDungeonTheme()
    {
        if (musicID != fDungeonThemeID)
        {
            fDungeonThemeID = EazySoundManager.PlayMusic(firstDungeonTheme, musicVolume, true, true, fadeIn, fadeOut);
            musicID         = fDungeonThemeID;
        }
    }

    public void PlaySecondDungeonTheme()
    {
        if (musicID != sDungeonThemeID)
        {
            sDungeonThemeID = EazySoundManager.PlayMusic(secondDungeonTheme, musicVolume, true, true, fadeIn, fadeOut);
            musicID         = sDungeonThemeID;
        }
    }

    public void PlayEscapeTheme()
    {
        if (musicID != escapeThemeID)
        {
            escapeMusicVolume = .5f;
            tempMusicVolume   = escapeMusicVolume;
            escapeThemeID =
                EazySoundManager.PlayMusic(escapeTheme, escapeMusicVolume, true, true, fadeInEscape, fadeOutEscape);
            musicID = escapeThemeID;
        }
    }

    public void PlayPuzzleTheme()
    {
        if (musicID != puzzleThemeID)
        {
            puzzleThemeID = EazySoundManager.PlayMusic(puzzleTheme, musicVolume, true, true, fadeIn, fadeOut);
            musicID       = puzzleThemeID;
        }
    }

    public void PlayMiniBossTheme()
    {
        if (musicID != miniBossThemeID)
        {
            miniBossThemeID = EazySoundManager.PlayMusic(miniBossTheme, musicVolume, true, true, fadeIn, fadeOut);
            musicID         = miniBossThemeID;
        }
    }

    public void PlayBossTheme()
    {
        if (musicID != bossThemeID)
        {
            bossThemeID = EazySoundManager.PlayMusic(bossTheme, musicVolume, true, true, fadeInBoss, fadeOutBoss);
            musicID     = bossThemeID;
        }
    }

    public void PauseMusic() { EazySoundManager.PauseAllMusic(); }

    public void ResumeMusic() { EazySoundManager.ResumeAllMusic(); }

    public void StopAllMusic() { EazySoundManager.StopAllMusic(); }

    public void ResetMusicID() { musicID = -1; }

    public void IncreaseEscapeThemeVolume(float newVolume)
    {
        if (escapeThemeSource == null) escapeThemeSource = FindObjectOfType<AudioSource>();

        if (tempMusicVolume < newVolume)
        {
            tempMusicVolume                    += 0.002f;
            EazySoundManager.GlobalMusicVolume =  tempMusicVolume;
        }
    }

    public void DecreaseEscapeThemeVolume(float newVolume)
    {
        if (escapeThemeSource == null) escapeThemeSource = FindObjectOfType<AudioSource>();

        if (tempMusicVolume > newVolume)
        {
            tempMusicVolume                    -= 0.002f;
            EazySoundManager.GlobalMusicVolume =  tempMusicVolume;
        }
    }

    #endregion

    #region PlayerSFX

    public void WalkSFX()
    {
        if (EnableSFX)
        {
            int randomWalkSFX = Random.Range(0, walkSFX.Length);
            sfxID = EazySoundManager.PlaySound(walkSFX[randomWalkSFX], walkSFXVolume);
        }
    }

    public void JumpSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(jumpSFX, jumpSFXVolume);
    }

    public void RollSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(rollSFX, rollSFXVolume);
    }

    public void ReviveSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(reviveSFX, sfxVolume);
    }

    public void TeleportSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(teleportSFX, sfxVolume);
    }

    public void HitSFX()
    {
        if (soundsPlayedHit < maxSoundsHit) { soundsPlayedHit++; }
        else { return; }

        if (EnableSFX) sfxID = EazySoundManager.PlaySound(hitSFX, sfxVolume);
    }

    public void DeathSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(deathSFX, sfxVolume);
    }

    #endregion

    #region ArrowSFX

    public void ShootSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(shootSFX, sfxVolume);
    }

    public void PowerShotSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(powerShotSFX, powerShotSFXVolume);
    }

    public void IceShotSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(iceShotSFX, sfxVolume);
    }

    public void FireShotSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(fireShotSFX, sfxVolume);
    }

    public void SpecialShotSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(specialShotSFX, sfxVolume);
    }

    public void FragmentSFX()
    {
        if (soundsPlayedFragment < maxSoundsFragment) { soundsPlayedFragment++; }
        else { return; }

        if (EnableSFX) sfxID = EazySoundManager.PlaySound(fragmentSFX, sfxVolume);
    }

    public void NoEnergySFX()
    {
        if (soundPlayedNoEnergy < maxSoundsNoEnergy) { soundPlayedNoEnergy++; }
        else { return; }

        if (EnableSFX) sfxID = EazySoundManager.PlaySound(noEnergySFX, sfxVolume);
    }

    public void NoEnergySFX(PopUpController playerPopup)
    {
        if (soundPlayedNoEnergy < maxSoundsNoEnergy) { soundPlayedNoEnergy++; }
        else { return; }

        if (EnableSFX)
        {
            sfxID = EazySoundManager.PlaySound(noEnergySFX, sfxVolume);
            playerPopup.NoStaminaPopUp();
        }
    }
    
    public void NoHPSFX(PopUpController playerPopup)
    {
        if (soundPlayedNoEnergy < maxSoundsNoEnergy) { soundPlayedNoEnergy++; }
        else { return; }

        if (EnableSFX)
        {
            sfxID = EazySoundManager.PlaySound(noEnergySFX, sfxVolume);
            playerPopup.NoHpPopup();
        }
    }

    #endregion

    #region ItemSFX

    public void DoorOpenSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(doorOpenSFX, sfxVolume);
    }

    public void DoorSpecialOpenSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(doorSpecialOpenSFX, sfxVolume);
    }

    public void ChestSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(chestSFX, sfxVolume);
    }

    public void CoinSFX()
    {
        if (soundsPlayedCoin < maxSoundsCoin) { soundsPlayedCoin++; }
        else { return; }

        if (EnableSFX) sfxID = EazySoundManager.PlaySound(coinSFX, coinSFXVolume);
    }

    public void MushroomSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(mushroomSFX, sfxVolume);
    }

    public void CollectSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(collectSFX, sfxVolume);
    }

    public void OpenPortalSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(openPortalSFX, sfxVolume);
    }

    public void ClosePortalSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(closePortalSFX, sfxVolume);
    }

    public void CrusherSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(crusherSFX, sfxVolume);
    }

    public void WallBreachSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(wallBreachSFX, sfxVolume);
    }

    public void GateSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(gateSFX, gateSFXVolume);
    }

    public void SaveGameSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(saveGameSFX, sfxVolume);
    }

    public void CheckpointSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(checkPointSFX, checkpointSFXVolume);
    }

    #endregion

    #region DialogueSFX

    public void CharacterTypoSFX(float pitchLevel)
    {
        if (EnableSFX)
        {
            sfxID                                 = EazySoundManager.PlaySound(characterTypoSFX, blipSFXVolume);
            FindObjectOfType<AudioSource>().pitch = pitchLevel;
        }
    }

    #endregion

    #region ShopSFX

    public void PurchaseSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(purchaseSFX, sfxVolume);
    }

    public void UpgradeSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(upgradeSFX, sfxVolume);
    }

    public void ButtonClickSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(buttonClickSFX, sfxVolume);
    }

    #endregion

    #region EnemySFX

    public void EnemyHitSFX()
    {
        if (soundsPlayedEnemyHit < maxSoundsEnemyHit) { soundsPlayedEnemyHit++; }
        else { return; }

        if (EnableSFX) sfxID = EazySoundManager.PlaySound(enemyHitSFX, sfxVolume);
    }

    public void LaserSFX()
    {
        if (EnableSFX) sfxID = EazySoundManager.PlaySound(laserSFX, bossLaserSFXVolume);
    }

    public void StopLaserSFX()
    {
        if (EnableSFX)
        {
            Audio laserSFXAudio = EazySoundManager.GetSoundAudio(laserSFX);
            laserSFXAudio.Stop();
        }
    }

    #endregion

    #region UI SFX

    public void ButtonSelectSFX()
    {
        if (soundsPlayedUI < maxSoundsUI) { soundsPlayedUI++; }
        else { return; }

        if (EnableSFX) sfxID = EazySoundManager.PlayUISound(buttonSelectUISFX, UISFXVolume);
    }

    public void ButtonHoverSFX()
    {
        if (soundsPlayedUI < maxSoundsUI) { soundsPlayedUI++; }
        else { return; }

        if (EnableSFX) sfxID = EazySoundManager.PlayUISound(buttonHoverUISFX, UISFXVolume);
    }

    #endregion

    #region Volume Setters

    public void SetGlobalMusicVolume(float volume)
    {
        EazySoundManager.GlobalMusicVolume    = volume;
        EazySoundManager.GlobalUISoundsVolume = volume;
        EazySoundManager.GlobalSoundsVolume   = volume;
    }

    public void LowerGlobalMusicVolume() { EazySoundManager.GlobalMusicVolume = 0f; }

    public void RiseGlobalMusicVolume() { EazySoundManager.GlobalMusicVolume = globalMusicVolume; }

    public void LowerGlobalSFXVolume()
    {
        EazySoundManager.GlobalUISoundsVolume = 0f;
        EazySoundManager.GlobalSoundsVolume   = 0f;
    }

    public void RiselobalSFXGVolume()
    {
        EazySoundManager.GlobalUISoundsVolume = globalSFXVolume;
        EazySoundManager.GlobalSoundsVolume   = globalSFXVolume;
    }

    #endregion
}