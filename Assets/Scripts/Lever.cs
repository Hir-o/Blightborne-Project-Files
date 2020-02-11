using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Lever : MonoBehaviour
{
    [SerializeField] private Sprite       leverUp;
    [SerializeField] private GameObject[] objectsToInteract;
    [SerializeField] private GameObject   buttonSprite;

    private enum LeverState
    {
        Locked,
        Unlocked
    };

    private LeverState leverState = LeverState.Locked;

    private bool           canActivate = false;
    private Player         player;
    private BoxCollider2D  boxCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Freezing Lever Code")]
    [SerializeField]
    private bool isFreezable;

    [SerializeField] private GameObject freezeIcon;
    [SerializeField] private float      freezeDuration = 8f;
    [SerializeField] private Sprite     leverDown;
    private                  bool       isFreezed = false;

    [Header("Burnt Lever Code")]
    [SerializeField]
    private bool isBurnable;

    [SerializeField] private GameObject burnIcon;
    [SerializeField] private Sprite     leverDownBurnt;
    private                  bool       isBurnt = false;

    private Arrow                   currentArrow;
    private CinemachineCameraShaker cameraShaker;

    [Header("Lever Tints")]
    [SerializeField]
    private Color normalColor;

    [SerializeField] private Color frozenColor;
    [SerializeField] private Color burntColor;

    [Header("Portal Check")]
    [SerializeField]
    private float checkForPortalInverval = 1f;

    [SerializeField] private float        rangeX = 3.4f;
    [SerializeField] private float        rangeY = .5f;
    [SerializeField] private Collider2D[] portals;
    [SerializeField] private LayerMask    portalMask;

    [Header("Camera Shake Values")]
    [SerializeField]
    private float duration = .6f;

    [SerializeField] private float amplitude = .3f;
    [SerializeField] private float frequency = 3f;

    private Animator _animator;

    private void Start ()
    {
        _animator = buttonSprite.GetComponent<Animator>();
        boxCollider    = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShaker   = FindObjectOfType<CinemachineCameraShaker>();

        if (isBurnable == true) spriteRenderer.color = frozenColor;

        InvokeRepeating(nameof(CheckForPortal), checkForPortalInverval, checkForPortalInverval);
    }

    private void Update ()
    {
        if (portals.Length > 0) { return; }

        if (canActivate && leverState == LeverState.Locked)
        {
            if (isBurnable) { return; }

            if (InputControl.GetButtonDown("Interact"))
            {
                InteractWithLever();

                if (isFreezable) Invoke(nameof(DeactivateLever), 1f);
            }
        }

        if (leverState == LeverState.Unlocked)
        {
            boxCollider.isTrigger = false;
            boxCollider.enabled   = false;
            buttonSprite.SetActive(false);
        }
    }

    private void CheckForPortal ()
    {
        portals = Physics2D.OverlapBoxAll(transform.position, new Vector2(rangeX, rangeY), 0f, portalMask);
    }

    public void InteractWithLever ()
    {
        if (isFreezable)
        {
            freezeIcon.SetActive(false);
            spriteRenderer.color = frozenColor;
        }

        if (isBurnable)
        {
            burnIcon.SetActive(false);
            spriteRenderer.color = burntColor;
        }

        spriteRenderer.sprite = leverUp;

        foreach (GameObject obstacle in objectsToInteract)
        {
            obstacle.GetComponent<Animator>().SetBool("isLocked", false);
        }

        leverState = LeverState.Unlocked;

        GameManager.rippleEffect.SetNewRipplePosition(GameManager.mainCamera.WorldToScreenPoint(transform.position));
        
        CameraShakeController.Instance.ShakeCameraLever();
        
        AudioController.Instance.GateSFX();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.PlayerTag))
        {
            if (isBurnable == false && isFreezable == false)
            {
                if (portals.Length == 0)
                {
                    player = other.gameObject.GetComponent<Player>();
                    buttonSprite.SetActive(true);
                    canActivate = true;
                }
            }
        }

        if (isFreezable)
        {
            if (other.gameObject.CompareTag(Tag.ProjectileTag))
            {
                currentArrow = other.gameObject.GetComponent<Arrow>();

                if (currentArrow.arrowType == Arrow.ArrowType.Ice)
                {
                    if (isFreezed == false)
                    {
                        isFreezed = true;
                        InteractWithLever();
                        Invoke(nameof(DeactivateLever), freezeDuration);
                        Destroy(currentArrow.gameObject);
                    }
                }
            }
        }

        if (isBurnable)
        {
            if (other.gameObject.CompareTag(Tag.ProjectileTag))
            {
                currentArrow = other.gameObject.GetComponent<Arrow>();

                if (currentArrow.arrowType == Arrow.ArrowType.Fire)
                {
                    if (isBurnt == false)
                    {
                        isBurnt = true;
                        InteractWithLever();
                        Destroy(currentArrow.gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D (Collider2D other)
    {
        if (isFreezable) { return; }

        if (isBurnable) { return; }

        if (portals.Length > 0)
        {
            buttonSprite.SetActive(false);
            canActivate = false;
            return;
        }

        if (player != null)
            if (player.isDeactivated) { return; }
        
        if (_animator != null)
        {
            switch (KeyBindings.CurrentProfile)
            {
                case KeyBindings.Profile.Profile1:
                    _animator.SetBool("E",    false);
                    _animator.SetBool("Down", true);
                    break;
                case KeyBindings.Profile.Profile2:
                    _animator.SetBool("Down", false);
                    _animator.SetBool("E",    true);
                    break;
                case KeyBindings.Profile.Profile3:
                    _animator.SetBool("E",    false);
                    _animator.SetBool("Down", true);
                    break;
            }
        }

        if (other.CompareTag(Tag.PlayerTag))
        {
            if (player.GetComponent<Animator>().GetBool("isRolling")) { return; }

            if (player.GetComponent<Animator>().GetBool("isClimbing")) { return; }

            if (player.GetComponent<Animator>().GetBool("isSliding")) { return; }

            if (player.GetComponent<Animator>().GetBool("isJumping")) { return; }
            
            GameManager.Instance.peekDisabled = true;

            if (buttonSprite.activeSelf == false)
            {
                player = other.gameObject.GetComponent<Player>();
                buttonSprite.SetActive(true);
                canActivate = true;
            }
        }
    }

    private void OnTriggerExit2D (Collider2D other)
    {
        if (portals.Length > 0) { return; }

        if (isBurnable) { return; }

        if (other.CompareTag(Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
            canActivate = false;
            GameManager.Instance.peekDisabled = false;
        }
    }

    private void DeactivateLever ()
    {
        spriteRenderer.color = normalColor;
        leverState           = LeverState.Locked;
        freezeIcon.SetActive(true);
        boxCollider.isTrigger = true;
        boxCollider.enabled   = true;
        buttonSprite.SetActive(false);
        isFreezed = false;

        foreach (GameObject obstacle in objectsToInteract)
            obstacle.GetComponent<Animator>().SetBool("isLocked", true);

        spriteRenderer.sprite = leverDown;
        
        CameraShakeController.Instance.ShakeCameraLever();
        
        AudioController.Instance.GateSFX();
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(rangeX, rangeY, 1f));
    }
}