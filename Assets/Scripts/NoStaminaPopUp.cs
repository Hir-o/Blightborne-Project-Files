using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoStaminaPopUp : MonoBehaviour
{
    public static NoStaminaPopUp Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}