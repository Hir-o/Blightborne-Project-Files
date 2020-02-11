using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemyChecker : MonoBehaviour
{
    [SerializeField] private EnemyMele[]  enemies;
    [SerializeField] private float        nextSceneTimer = 4f;
    [SerializeField] private GameObject   levelClearedPanel;
    private                  EnemyCounter _enemyCounter;

    private void Start ()
    {
        _enemyCounter = FindObjectOfType<EnemyCounter>();
        InvokeRepeating("CheckForAliveEnemies", 126f, 2f);
    }

    private void CheckForAliveEnemies ()
    {
        enemies = FindObjectsOfType<EnemyMele>();

        if (enemies.Length == 0)
        {
            CancelInvoke("CheckForAliveEnemies");
            levelClearedPanel.SetActive(true);
            Invoke(nameof(LoadNextScene), nextSceneTimer);
        }
    }

    private void LoadNextScene ()
    {
        GameManager.cameraTransition.StartSwipeIn();

        Invoke(nameof(InitializeLoading), 1.4f);
    }

    private void InitializeLoading ()
    {
        Physics2D.IgnoreLayerCollision(12, 13, false);
//		_enemyCounter.enemyCounter = 67;
        LevelManager.LoadNextScene();
    }
}