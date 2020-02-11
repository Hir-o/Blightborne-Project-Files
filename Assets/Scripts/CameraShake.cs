using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    [SerializeField] GameObject cameraParent;

    [SerializeField] float shakeAmount = 0f;

    private void Awake()
    {
        if (cameraParent == null)
        {
            cameraParent = Camera.main.GetComponentInParent<Transform>().gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Shake(2f, .1f);
    }

    public void Shake(float amount, float length)
    {
        shakeAmount = amount;

        InvokeRepeating("BeginShake", 0f, 0.01f);
        Invoke("StopShake", length);
    }

    private void BeginShake()
    {
        if(shakeAmount > Mathf.Epsilon)
        {
            Vector3 camParentPosition = Camera.main.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;

            camParentPosition.x += offsetX;
            camParentPosition.y += offsetY;

            cameraParent.transform.position = camParentPosition;
        }
    }

    private void StopShake()
    {
        CancelInvoke("BeginShake");
        cameraParent.transform.localPosition = Vector3.zero;
    }

}
