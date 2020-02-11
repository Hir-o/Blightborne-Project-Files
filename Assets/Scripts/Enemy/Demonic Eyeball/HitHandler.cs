using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitHandler : MonoBehaviour {

	[SerializeField] private GameObject hitParticles;
	[SerializeField] private GameObject deathParticles;
	[SerializeField] private float particleDestroyTimer = 2f;
	[SerializeField] private float deathParticleDestroyTimer = 3f;

	[SerializeField] private Vector3 deathParticlesOffset;

	private GameObject tempParticles;

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag(Tag.ProjectileTag))
		{
			if (other.gameObject.GetComponent<Arrow>().arrowType == Arrow.ArrowType.Special)
				return;
		}

		if (other.gameObject.CompareTag(Tag.ProjectileTag)
			|| other.gameObject.CompareTag(Tag.ProjectileFragmentTag))
		{
			tempParticles = Instantiate(hitParticles, other.transform.position, Quaternion.identity);
			Destroy(tempParticles, particleDestroyTimer);
		}	
	}

	public void PlayDeathParticles() // called from demonic eyeball health checkfordead method
	{
		tempParticles = Instantiate(deathParticles, transform.position + deathParticlesOffset, Quaternion.identity);
		Destroy(tempParticles, deathParticleDestroyTimer);
	}

}
