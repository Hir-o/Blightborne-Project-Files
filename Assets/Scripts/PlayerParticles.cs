using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem runningParticles;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem rollParticles;
    [SerializeField] private ParticleSystem dashParticleTrail;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem laserhitParticles;
    [SerializeField] private ParticleSystem slideParticles;

    [SerializeField] private Transform runningParticlesPos;
    [SerializeField] private Transform jumpParticlesPos;
    [SerializeField] private Transform rollParticlesPos;

    private ParticleSystem tempParticles;
    private Vector3        tempVector;
    private Player         player;
    private Rigidbody2D    rigidbody;
    private Animator animator;

    private void Start ()
    {
        player    = GetComponent<Player>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void PlayRunningParticles () // called from animation event
    {
        if (rigidbody.velocity.y < -0.1) { return; }

        tempParticles =
            Instantiate(runningParticles, runningParticlesPos.position, Quaternion.identity);

        tempVector = new Vector3(player.transform.localScale.x, 1f, 1f);

        tempParticles.transform.localScale = tempVector;

        Destroy(tempParticles.gameObject, 2f);
    }

    public void PlayJumpParticles () // called from animation event and PlayerMushroomJump
    {
        tempParticles = Instantiate(jumpParticles, jumpParticlesPos.position, Quaternion.identity);
        tempVector    = new Vector3(player.transform.localScale.x, 1f, 1f);

        tempParticles.transform.localScale = tempVector;

        Destroy(tempParticles.gameObject, 2f);
    }

    public void PlayRollParticles () // called from player "roll" method
    {
        dashParticleTrail.Play();

        Invoke(nameof(StopDashTrailParticle), .2f);

        if (player.playRollVFX == false || player.GetIsRollingMidAir()) { return; }

        tempParticles = Instantiate(rollParticles, rollParticlesPos.position, Quaternion.identity);
        tempVector    = new Vector3(player.transform.localScale.x, 1f, 1f);

        tempParticles.transform.localScale = tempVector;

        Destroy(tempParticles.gameObject, 2f);
    }

    public void PlayHitParticles () // called from player animation event and player HitByLaser()
    {
        tempParticles = Instantiate(hitParticles, transform.position, Quaternion.identity);
        tempVector    = new Vector3(player.transform.localScale.x, 1f, 1f);

        tempParticles.transform.localScale = tempVector;

        Destroy(tempParticles.gameObject, 2f);
    }

    public void PlayLaserHitParticles () // called from player HitByLAser()
    {
        tempParticles                      = Instantiate(laserhitParticles, transform.position, Quaternion.identity);
        tempVector                         = new Vector3(player.transform.localScale.x, 1f, 1f);
        tempParticles.transform.localScale = tempVector;

        Destroy(tempParticles.gameObject, 2f);
    }

    public void PlaySlideParticles ()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit")) { return; }
        
        slideParticles.Play();
    }

    public void StopSlideParticles ()
    {
        slideParticles.Stop();
    }

    private void StopDashTrailParticle () { dashParticleTrail.Stop(); }
}