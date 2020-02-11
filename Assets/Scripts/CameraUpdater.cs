using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraUpdater : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField]
    private float timeUntilTransitionComplete = 2f;

    [Header("Virtual Camera Position Offset")]
    [SerializeField]
    private float screenXValue = .2f;

    [SerializeField] private float screenYValue = .2f;

    [Header("Scene Virtual Camera")]
    [SerializeField]
    private CinemachineVirtualCamera runVirtualCamera;

    private CinemachineFramingTransposer _framingTransposer;

    private void Start ()
    {
        _framingTransposer = runVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        CinemachineOffset.Instance.newYVal = screenYValue;

        UpdateMScreenX();
        UpdateMScreenY();
    }

    private void UpdateMScreenX ()
    {
        DOVirtual.Float(_framingTransposer.m_ScreenX, screenXValue, timeUntilTransitionComplete, MScreenX);
    }

    private void UpdateMScreenY ()
    {
        DOVirtual.Float(_framingTransposer.m_ScreenY, screenYValue, timeUntilTransitionComplete, MScreenY);
    }

    private void MScreenX (float x) { _framingTransposer.m_ScreenX = x; }

    private void MScreenY (float y) { _framingTransposer.m_ScreenY = y; }
}