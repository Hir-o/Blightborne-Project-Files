using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour {

    [SerializeField] int priority = 0;
    
    private Animator animator;
    [SerializeField] private GameObject particleHolder;
    [SerializeField] private bool isParticleSystemInvisible = false;

    private FourMovingPlatforms fourMovingPlatforms;
    private Player player;
    [SerializeField] private bool canReplenish = true;

    private void Start() 
    {
        animator = GetComponent<Animator>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(Tag.PlayerTag))
        {
            if (canReplenish)
            {
                PlayerStats.ResetHealth();
                PlayerStats.ResetStamina();
                HealthBar.UpdateHealthBarStatus();
                StaminaBar.UpdateStaminaBarStatus();
                canReplenish = false;
            }
            
            animator.SetTrigger("check");

            if (isParticleSystemInvisible == false)
                particleHolder.SetActive(true);
                
            if(priority > PlayerStats.CheckpointPriority)
            {
                PlayerStats.CheckpointPriority = priority;
                PlayerStats.CheckpointLocation = gameObject.transform.position;

                if (SceneManager.GetActiveScene().name == "Dungeon II-IV")
                {
                    player = GameManager.player;
                    
                    if (priority == 3)
                    {
                        PlayerStats.CheckpointLocation = new Vector3(transform.position.x,
                                                                transform.position.y,
                                                                player.transform.position.z);
                        
                        AudioController.Instance.CheckpointSFX(); 
                    }
                    else
                    {
                        PlayerStats.CheckpointLocation = new Vector3(transform.position.x,
                                                                -2.05f,
                                                                player.transform.position.z);
                    }
                }
                else
                    AudioController.Instance.CheckpointSFX();
            }
        }
    }
}
