using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class PlayerPortals : MonoBehaviour {

	[SerializeField] private Portal bluePortal;
	[SerializeField] private Portal redPortal;

	public Portal _bluePortal;
	public Portal _redPortal;

	public bool isRedPortalActive = false;
	public bool isBluePortalActive = false;

	[SerializeField] private Vector3 offset;

	private void Update() 
	{
		if (GameManager.AllowUseOfPortals)
		{
			if (isBluePortalActive == false && InputControl.GetButtonDown("Blue Portal"))
			{
				_bluePortal = Instantiate(bluePortal, transform.position + offset, Quaternion.identity).GetComponent<Portal>();
				isBluePortalActive = true;

				_bluePortal.SetPortalTypeBlue();
				AudioController.Instance.OpenPortalSFX();

				GameManager.hud.UpdateBluePortalImage();
			}

			if (isRedPortalActive == false && InputControl.GetButtonDown("Red Portal"))
			{
				_redPortal = Instantiate(redPortal, transform.position + offset, Quaternion.identity).GetComponent<Portal>();
				isRedPortalActive = true;

				_redPortal.SetPortalTypeRed();
				AudioController.Instance.OpenPortalSFX();

				GameManager.hud.UpdateRedPortalImage();
			}
		}
	}

	public void DestroyBluePortal()
	{
		Destroy(_bluePortal.gameObject, .5f);
		isBluePortalActive = false;
		AudioController.Instance.ClosePortalSFX();

		GameManager.hud.UpdateBluePortalImage();
	}
	
	public void DestroyRedPortal()
	{
		Destroy(_redPortal.gameObject, .5f);
		isRedPortalActive = false;
		AudioController.Instance.ClosePortalSFX();

		GameManager.hud.UpdateRedPortalImage();
	}

}
