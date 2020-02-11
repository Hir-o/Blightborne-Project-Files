using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakJoint : MonoBehaviour
{
    private HingeJoint2D[]  joints;
    private List<EnemyMele> enemies = new List<EnemyMele>();
    private Rigidbody2D     enemyRigidBody;

    [SerializeField] private float mass;
    [SerializeField] private float massBreakPoint;
    [SerializeField] private float minBreakTime;
    [SerializeField] private float maxBreakTime;
    [SerializeField] private float breakForce;

    private enum BridgeState
    {
        Destroyed,
        Intact
    }

    private BridgeState bridgeState = BridgeState.Intact;

    [Header("Layers to ignore collision with")]
    private Player player;

    [HideInInspector] public bool canEnemyCollide = true;

    private void Start ()
    {
        joints = GetComponentsInChildren<HingeJoint2D>();
        player = GameManager.player;
    }

    public void AddEnemy (EnemyMele enemy)
    {
        if (enemies != null)
        {
            foreach (EnemyMele e in enemies)
                if (e == enemy) { return; }
        }

        enemies.Add(enemy);

        AffectMass(enemy.GetComponent<Rigidbody2D>().mass);
    }

    public void RemoveEnemy (EnemyMele enemy)
    {
        if (enemies != null)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemy == enemies[i])
                {
                    AffectMass(-enemies[i].GetComponent<Rigidbody2D>().mass);
                    enemies.Remove(enemies[i]);
                }
            }
        }
    }

    private void AffectMass (float enemyMass)
    {
        mass += enemyMass;

        if (mass >= massBreakPoint)
        {
            if (bridgeState == BridgeState.Intact)
            {
                Invoke(nameof(BreakBridge), Random.Range(minBreakTime, maxBreakTime));
                bridgeState = BridgeState.Destroyed;
            }
        }
    }

    private void BreakBridge ()
    {
        if (Random.value < .5f)
        {
            Destroy(joints[1]);
            Destroy(joints[2]);
        }
        else
        {
            Destroy(joints[joints.Length - 2]);
            Destroy(joints[joints.Length - 3]);
        }

        Destroy(joints[5]);
        Destroy(joints[6]);
        Destroy(joints[7]);
        Destroy(joints[8]);

        foreach (HingeJoint2D joint in joints)
        {
            joint.breakForce = breakForce;
            Physics2D.IgnoreCollision(joint.GetComponent<Collider2D>(), player.GetComponent<CapsuleCollider2D>(), true);
            canEnemyCollide        = false;
            joint.gameObject.layer = 0;

            joint.GetComponent<BridgeJointsCollisionChecker>().HandleHazardCollision();
        }
    }
}