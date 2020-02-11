using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    private Rigidbody2D          rigidBody;
    private Vector2              direction;
    private Collider2D[]         colliders;
    private SpriteRenderer       spriteRenderer;
    private CircleCollider2D     circleCollider;
    private EnemyMele            enemyMele;
    private EnemyFollowMele      enemyFollowMele;
    private DemonicEyeBallHealth demonicHealth;
    private Tower                tower;

    [SerializeField] private int        damage = 10;
    [SerializeField] private GameObject explodeShockwave;
    [SerializeField] private float      throwSpeed = 2000f;
    [SerializeField] private float      radius     = 4f;
    [SerializeField]         LayerMask  enemyLayerMask;

    private void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();

        //damage += PlayerStats.Strength;
        rigidBody = GetComponent<Rigidbody2D>();
        Throw();
    }

    private void Throw ()
    {
        direction = new Vector2(Random.Range(-.4f, .4f), Random.Range(.8f, 1f));
        rigidBody.AddForce(direction * throwSpeed);
    }

    private void OnCollisionEnter2D (Collision2D other) { Explode(); }

    public void Explode ()
    {
        AudioController.Instance.FragmentSFX();
        explodeShockwave.SetActive(true);
        rigidBody.bodyType     = RigidbodyType2D.Static;
        spriteRenderer.sprite  = null;
        circleCollider.enabled = false;

        colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayerMask);

        foreach (Collider2D coll in colliders)
        {
            if (coll.gameObject.CompareTag(Tag.SmartEnemyTag))
            {
                enemyMele       = coll.gameObject.GetComponent<EnemyMele>();
                enemyFollowMele = coll.gameObject.GetComponent<EnemyFollowMele>();

                if (enemyMele.isBoss)
                    enemyMele.TakeFragmentDamageBoss(damage / 2);
                else
                    enemyMele.TakeDamage(damage);


                if (enemyFollowMele.GetPlayer() == null) enemyFollowMele.SetAndFollowPlayer();
            }
            else if (coll.gameObject.CompareTag(Tag.BossTag))
            {
                coll.gameObject.GetComponent<DemonicEyeBallHealth>().TakeDamage(damage);
            }
            else if (coll.gameObject.CompareTag(Tag.EnemyTag))
            {
                if (coll.gameObject.GetComponent<Enemy>() != null)
                    coll.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            }
            else if (coll.gameObject.CompareTag(Tag.TowerTag))
            {
                tower = coll.gameObject.GetComponent<Tower>();
                tower.DisableTower();
            }
            else if (coll.gameObject.CompareTag(Tag.CrateTag)) coll.gameObject.GetComponent<Crate>().DisableCrate();
        }

        StartCoroutine(AddEffect());
        Invoke("DestroyFragment", 10f);
    }

    private void DestroyFragment () { Destroy(this.gameObject); }

    public int GetDamage () //Needed because of EnemyBat script
    {
        return damage;
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private IEnumerator AddEffect ()
    {
        CameraShakeController.Instance.FireShotCameraShake();

        yield return new WaitForSeconds(.1f);
    }
}