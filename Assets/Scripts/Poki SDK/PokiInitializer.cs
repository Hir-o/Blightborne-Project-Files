using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokiInitializer : MonoBehaviour
{
    private void Awake ()
    {
        PokiUnitySDK.Instance.init();
    }
}