using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    private static GameObject[] _objectsToEnable;
    public static  int          objectCount;

    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Ogre Boss Code")]
    [SerializeField]
    private Vector2 throwVelocity = new Vector2(0f, 8f);

    private EnemyMele[] _enemies;

    private void Start ()
    {
        _objectsToEnable = objectsToEnable;
        objectCount      = 0;
    }

    public static void EnableObstacle ()
    {
        _objectsToEnable[objectCount].SetActive(true);
        objectCount++;
    }

    public static void DisableObstacles ()
    {
        foreach (GameObject gObject in _objectsToEnable)
            if (gObject != null)
                Destroy(gObject);
    }

    public void ShakeGround ()
    {
        CameraShakeController.Instance.OgreBossWalkShake();

        _enemies = FindObjectsOfType<EnemyMele>();

        if (GameManager.player.GetIsJumping()                                == false &&
            GameManager.player.GetComponent<Animator>().GetBool("isFalling") == false &&
            GameManager.player.GetComponent<Animator>().GetBool("isRolling") == false &&
            GameManager.player.GetComponent<Animator>().GetBool("isShooting") == false &&
            GameManager.player.GetComponent<Animator>().GetBool("isHit") == false)
        {
            GameManager.player.GetComponent<Rigidbody2D>().velocity +=
                new Vector2(throwVelocity.x, throwVelocity.y + Random.Range(0f, 4f));
        }

        foreach (EnemyMele enemy in _enemies)
        {
            if (enemy.isBoss == false)
            {
                if (enemy.GetComponent<Rigidbody2D>().velocity.y >= 0f)
                    enemy.GetComponent<Rigidbody2D>().velocity +=
                        new Vector2(throwVelocity.x, throwVelocity.y + Random.Range(0f, 4f));
            }
        }
    }
}