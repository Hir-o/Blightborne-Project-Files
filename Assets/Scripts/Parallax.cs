using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

    [SerializeField] private Transform[] parallaxedElements;
    [SerializeField] private float smoothing = 1f;
    private float[] parallaxScales;

    private Transform mainCamera;
    private Vector3 previousCameraPosition;

    //parallax variables
    private float parallaxPosX;
    private float parallaxPosY;
    private float backgroundTargetPosX;
    private float backgroundTargetPosY;
    private Vector3 backgroundTargetPosition;

    private void Start()
    {
        mainCamera = GameManager.mainCamera.transform;

        previousCameraPosition = mainCamera.position;

        parallaxScales = new float[parallaxedElements.Length];

        for (int i = 0; i < parallaxedElements.Length; i++)
        {
            parallaxScales[i] = parallaxedElements[i].position.z * -1;
        }
    }

    private void Update()
    {
        for(int i = 0; i < parallaxedElements.Length; i++)
        {
            parallaxPosX = (previousCameraPosition.x - mainCamera.position.x) * parallaxScales[i];
            parallaxPosY = (previousCameraPosition.y - mainCamera.position.y) * parallaxScales[i];

            backgroundTargetPosX = parallaxedElements[i].position.x + parallaxPosX;
            backgroundTargetPosY = parallaxedElements[i].position.y + parallaxPosY;

            backgroundTargetPosition = new Vector3
                (
                    backgroundTargetPosX, 
                    backgroundTargetPosY, 
                    parallaxedElements[i].position.z
                );

            parallaxedElements[i].position = Vector3.Lerp
                (
                    parallaxedElements[i].position,
                    backgroundTargetPosition,
                    smoothing * Time.deltaTime
                );
        }

        previousCameraPosition = mainCamera.position;
    }
}
