using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

	[SerializeField] private DemonicEyeBallHealth demonicEyeBall;
	[SerializeField] private GameObject otherTower;
	
	private Animator animator;
	private AnimationClip[] clips;

	private float delayTime;

	private void OnEnable() 
	{
		demonicEyeBall.ActivateShield();	
	}

	private void Awake() 
	{
		animator = GetComponent<Animator>();
	}

	public void DisableTower()
	{
		animator.SetTrigger("hit");
		AudioController.Instance.EnemyHitSFX();

		clips = animator.runtimeAnimatorController.animationClips;
		delayTime = 0f;

		foreach (AnimationClip clip in clips)
		{
			if (clip.name.Equals("hit"))
			{
				delayTime = clip.length;
			}
		}

		Invoke(nameof(InitializeDisabling), delayTime);

		if (otherTower.activeSelf == false)
		{
			demonicEyeBall.DisableShield();
		}
	}

	private void InitializeDisabling()
	{
		this.gameObject.SetActive(false);
	}

}
