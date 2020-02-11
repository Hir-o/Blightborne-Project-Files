using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleAmountManager : MonoBehaviour
{
    [SerializeField] private float enemyRippleAmount      = 5f;
    [SerializeField] private float enemySmartRippleAmount = 5f;
    [SerializeField] private float dashRippleAmount       = 10f;

    public static float EnemyRippleAmount;
    public static float EnemySmartRippleAmount;
    public static float DashRippleAmount;

    private void Start ()
    {
        EnemyRippleAmount      = enemyRippleAmount;
        EnemySmartRippleAmount = enemySmartRippleAmount;
        DashRippleAmount       = dashRippleAmount;
    }
}