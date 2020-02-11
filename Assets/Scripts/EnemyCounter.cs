using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    public                   int             enemyCounter = 67;
    [SerializeField] private TextMeshProUGUI enemyCounterTMPro;
    private                  TextMeshProUGUI EnemyCounterTMPro;

    private void Start ()
    {
        EnemyCounterTMPro = enemyCounterTMPro;

        EnemyCounterTMPro.text = "Enemies Left: " + enemyCounter;
    }

    public void UpdateEnemyCounter ()
    {
        enemyCounter--;
        EnemyCounterTMPro.text = "Enemies Left: " + enemyCounter;
    }
}