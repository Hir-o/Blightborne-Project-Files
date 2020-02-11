using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PostProcessingController : MonoBehaviour
{
    public static PostProcessingController Instance;

    [Header("Post Processing Code")]
    [SerializeField]
    private PostProcessVolume volume;

    private Vignette            _vignette;
    private ChromaticAberration _chromaticAberration;
    private ColorGrading        _colorGrading;

    [Header("Vignette Values")]
    [SerializeField]
    private float zeroIntensity = 0f;

    [SerializeField] private float insideDungeonIntensity = .6f;

    [Header("Escape Scene")]
    [SerializeField]
    private string escapeSceneName = "Escape";

    public void SetVolumeObject (PostProcessVolume newVolume)
    {
        volume = newVolume;

        volume.profile.TryGetSettings(out _vignette);
        volume.profile.TryGetSettings(out _chromaticAberration);
        volume.profile.TryGetSettings(out _colorGrading);
    }

    private void Awake ()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        volume.profile.TryGetSettings(out _vignette);
        volume.profile.TryGetSettings(out _chromaticAberration);
        volume.profile.TryGetSettings(out _colorGrading);
    }

    public void SetToZeroIntensity ()    { _vignette.intensity.value = zeroIntensity; }
    public void SetToDungeonIntensity () { _vignette.intensity.value = insideDungeonIntensity; }

    public void EnableChromaticAberration () { _chromaticAberration.enabled.value = true; }

    public void DisableChromaticAberration () { _chromaticAberration.enabled.value = false; }

    public void ResetSaturation ()
    {
        if (_colorGrading.saturation.value < 0f)
        {
            DOVirtual.Float(_colorGrading.saturation.value, 0f, 1f, Saturation);

            if (SceneManager.GetActiveScene().name != escapeSceneName)
            {
                if (EazySoundManager.GlobalMusicVolume != 0f)
                    AudioController.Instance.RiseGlobalMusicVolume();
                
                if (EazySoundManager.GlobalSoundsVolume  != 0f)
                    AudioController.Instance.RiselobalSFXGVolume();
            }
        }
    }

    public void SetSaturationToMinimum ()
    {
//        if (_colorGrading.saturation.value > -1)
//        {
//            DOVirtual.Float(_colorGrading.saturation.value, -60f, 1f, Saturation);
//
//            if (SceneManager.GetActiveScene().name != escapeSceneName)
//            {
//                if (EazySoundManager.GlobalMusicVolume != 0f)
//                    AudioController.Instance.SetGlobalMusicVolume(.2f);
//            }
//        }
    }

    private void Saturation (float x) { _colorGrading.saturation.value = x; }
}