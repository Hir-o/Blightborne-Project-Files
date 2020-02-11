using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleEffectValues : MonoBehaviour
{
    public static RippleEffectValues Instance;
    
    [Header("Generic Ripple Values")]
    public float waveTime;
    public float waveInternalRadio;
    public float waveScale;
    public float waveSpeed;
    public float waveFrequency;
    
    [Header("Bat Boss Rage Ripple Values")]
    public float bWaveTime;
    public float bWaveInternalRadio;
    public float bWaveScale;
    public float bWaveSpeed;
    public float bWaveFrequency;
    
    private void Awake ()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
