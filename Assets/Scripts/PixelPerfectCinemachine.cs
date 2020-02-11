using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PixelPerfectCinemachine : MonoBehaviour {

    private Camera mainCamera;
    private CinemachineStateDrivenCamera stateDrivenCamera;
    private GameObject virtualCameraObject;
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        mainCamera = Camera.main;
        stateDrivenCamera = GetComponent<CinemachineStateDrivenCamera>();
    }

    void Update()
    {
        virtualCameraObject = stateDrivenCamera.LiveChild.VirtualCameraGameObject;
        virtualCamera = virtualCameraObject.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.OrthographicSize = mainCamera.orthographicSize;
    }

}
