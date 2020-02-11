using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Portal : MonoBehaviour {

	[SerializeField] private GameObject buttonSprite;
	private Player player;
	private bool canActivate = false;
	private PlayerPortals playerPortals;
	private Animator animator;
	
	public enum PortalType {Blue, Red};
	public PortalType portalType;

	[SerializeField] private float teleportTime = .05f;

	private void Start() 
	{
		player = GetComponent<Player>();
		animator = GetComponent<Animator>();
	}

	private void Update() 
	{	
		if (canActivate == true)
		{
			if (portalType == PortalType.Blue)
			{
				if (InputControl.GetButtonDown("Interact"))
				{
					if (playerPortals._redPortal != null)
					{
						player.GetComponent<Animator>().SetTrigger("interactPortal");
						Invoke("TeleportToRedPortal", teleportTime);
					}
				}

				if (playerPortals.isBluePortalActive && InputControl.GetButtonDown("Blue Portal"))
				{
					animator.SetTrigger("close");
					playerPortals.DestroyBluePortal();
				}
			}

			if (portalType == PortalType.Red)
			{
				if (InputControl.GetButtonDown("Interact"))
				{
					if (playerPortals._bluePortal != null)
					{
						player.GetComponent<Animator>().SetTrigger("interactPortal");
						Invoke("TeleportToBluePortal", teleportTime);
					}
				}

				if (playerPortals.isRedPortalActive == true && InputControl.GetButtonDown("Red Portal"))
				{
					animator.SetTrigger("close");
					playerPortals.DestroyRedPortal();
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == (Tag.PlayerTag))
        {
            buttonSprite.SetActive(true);
			player = other.gameObject.GetComponent<Player>();
			playerPortals = other.gameObject.GetComponent<PlayerPortals>();
			canActivate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == (Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
			canActivate = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
	{
        if (this.gameObject.transform.localScale.x > 0f)
            buttonSprite.transform.localScale = new Vector3(1f, 1f, 1f);
        else if (this.gameObject.transform.localScale.x < 0f)
            buttonSprite.transform.localScale = new Vector3(-1f, 1f, 1f);

        if (player.isDeactivated == true) { return; }

        if(other.tag == (Tag.PlayerTag))
        {
            if(player.GetComponent<Animator>().GetBool("isRolling") == true) { return; }
            if(player.GetComponent<Animator>().GetBool("isClimbing") == true) { return; }
            if(player.GetComponent<Animator>().GetBool("isSliding") == true) { return; }
            if(player.GetComponent<Animator>().GetBool("isJumping") == true) { return; }
        }
    }

	private void TeleportToRedPortal()
	{
		player.transform.position = playerPortals._redPortal.transform.position;
	}

	private void TeleportToBluePortal()
	{
		player.transform.position = playerPortals._bluePortal.transform.position;
	}

	public void SetPortalTypeBlue()
	{
		portalType = PortalType.Blue;
	}

	public void SetPortalTypeRed()
	{
		portalType = PortalType.Red;
	}

}
