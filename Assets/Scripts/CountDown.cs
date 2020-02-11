using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDown : MonoBehaviour
{
    [SerializeField] private float           timer        = 180f;
    [SerializeField] private float           restartTimer = 4f;
    private                  TextMeshProUGUI countdown;

    private                  int    minutes;
    private                  int    seconds;
    [SerializeField] private string sceneName;

    private bool sceneFailed = false;

    [SerializeField] private EnemyCounter _enemyCounter;

    private void Start () { countdown = GetComponent<TextMeshProUGUI>(); }

    private void Update ()
    {
        if (timer > 0f)
        {
            timer   -= Time.deltaTime;
            minutes =  Mathf.FloorToInt(timer / 60F);
            seconds =  Mathf.FloorToInt(timer - minutes * 60);

            if (timer < 0f)
                countdown.text = string.Format("{0:0}:{1:00}", 0, 0);
            else
                countdown.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        else if (sceneFailed == false)
        {
            sceneFailed = true;
            Invoke(nameof(RestartLevel), restartTimer);
        }
    }

    private void RestartLevel ()
    {
        GameManager.cameraTransition.StartSwipeIn();

        Invoke(nameof(InitializeRestart), 1.4f);
    }

    private void InitializeRestart ()
    {
        Physics2D.IgnoreLayerCollision(12, 13, false);
        _enemyCounter.enemyCounter = 67;
        LevelManager.LoadSceneWithName(sceneName, true);
    }
}