using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public static HUD             Instance;
    public        TextMeshProUGUI coinsText;

    private Player player;
    private Canvas thisCanvas;

    [Header("Skill Icon Gameobject")]
    [SerializeField]
    private GameObject powerShotIcon;

    [Header("Collectable Images")]
    [SerializeField]
    private Image blueKey;

    [SerializeField] private Image redKey;
    [SerializeField] private Image blueGem;
    [SerializeField] private Image redGem;

    [Header("Item Images Gameobject")]
    [SerializeField]
    private GameObject pendantIcon;

    [SerializeField] private GameObject ringIcon;
    [SerializeField] private GameObject pyramidIcon;

    [Header("Portal Images")]
    [SerializeField]
    private GameObject bluePortalImage;

    [SerializeField] private GameObject redPortalImage;

    [Header("HUD Camera")]
    [SerializeField]
    private Camera hudCamera;

    [Header("Save Crystal Code")]
    [SerializeField]
    private TextMeshProUGUI sceneNameText;

    [SerializeField] private Animator levelNamePanelAnimator;

    [Header("Ability Panel")]
    public GameObject abilityPanel;

    private Upgrade upgrade;

    [Header("Door Recall Icon")]
    [SerializeField]
    private GameObject recallDoorObject;

    [SerializeField] private Image    doorFrame;
    [SerializeField] private Animator doorAnimator;

    [Header("UI Upgrade Icon & Text")]
    [SerializeField]
    private Image upgradeImage;

    [SerializeField] private GameObject upgradeText;

    [SerializeField] private GameObject upgradeLight;

    [Header("Dialogue Panel")]
    public GameObject DialoguePanel;

    private void Awake ()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        thisCanvas = GetComponent<Canvas>();
    }

    void OnEnable () { SceneManager.sceneLoaded += OnSceneLoaded; }

    private void Start ()
    {
        thisCanvas.worldCamera = hudCamera;
        player                 = FindObjectOfType<Player>();
        upgrade                = abilityPanel.GetComponent<Upgrade>();

        UpdateHUD();
        UpdateItemsIcons();

        if (SceneManager.GetActiveScene().buildIndex > 4 && SceneManager.GetActiveScene().buildIndex < 21)
            recallDoorObject.SetActive(true);
        else
            recallDoorObject.SetActive(false);

        if (bluePortalImage.activeSelf) bluePortalImage.SetActive(false);

        if (redPortalImage.activeSelf) redPortalImage.SetActive(false);
    }

    void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();
        player.GetComponent<PlayerAttack>().SetPowerIcon(powerShotIcon);

        if (SceneManager.GetActiveScene().buildIndex > 4 && SceneManager.GetActiveScene().buildIndex < 21)
            recallDoorObject.SetActive(true);
        else
            recallDoorObject.SetActive(false);

        if (doorFrame.fillAmount != 1f) doorFrame.fillAmount = 1f;

        if (bluePortalImage.activeSelf) bluePortalImage.SetActive(false);

        if (redPortalImage.activeSelf) redPortalImage.SetActive(false);
    }

    public void UpdateHUD ()
    {
        if (PlayerStats.HasBlueKey)
            blueKey.gameObject.active = true;
        else
            blueKey.gameObject.active = false;

        if (PlayerStats.HasRedKey)
            redKey.gameObject.active = true;
        else
            redKey.gameObject.active = false;

        if (PlayerStats.HasBlueGem)
            blueGem.gameObject.active = true;
        else
            blueGem.gameObject.active = false;

        if (PlayerStats.HasRedGem)
            redGem.gameObject.active = true;
        else
            redGem.gameObject.active = false;
    }

    public void UpdateItemsIcons ()
    {
        if (PlayerStats.IsPendantPurchased)
            pendantIcon.SetActive(true);
        else
            pendantIcon.SetActive(false);

        if (PlayerStats.IsRingPurchased)
            ringIcon.SetActive(true);
        else
            ringIcon.SetActive(false);

        if (PlayerStats.IsEnergyPyramidPurchased)
            pyramidIcon.SetActive(true);
        else
            pyramidIcon.SetActive(false);
    }

    public void UpdateBluePortalImage () { bluePortalImage.SetActive(!bluePortalImage.activeSelf); }

    public void UpdateRedPortalImage () { redPortalImage.SetActive(!redPortalImage.activeSelf); }

    public void DisplaySaveNotification ()
    {
        sceneNameText.text = "Progress Saved";
        levelNamePanelAnimator.Play("expand", -1, 0f);
    }

    public void UpdateDoorFrame (float value)
    {
        doorFrame.fillAmount = value;

        if (doorFrame.fillAmount <= 0f && doorAnimator.GetBool("isOpen") == false)
        {
            doorAnimator.SetBool("isOpen", true);
            AudioController.Instance.DoorOpenSFX();
        }
    }

    public void UpdateDoorAnimation () { doorAnimator.SetBool("isOpen", false); }

    public bool IsDoorOpen () { return doorAnimator.GetBool("isOpen"); }

    public void UpdateUpgradeImage ()
    {
        if (PlayerStats.Strength       == 10
            && PlayerStats.Dexterity   == 10
            && PlayerStats.Vitality    == 10
            && PlayerStats.AttackSpeed == 10
            && PlayerStats.Armor       == 10)
        {
            upgradeImage.material.SetFloat("_OperationBlend_Fade_1", 0f);
            upgradeLight.SetActive(false);
            upgradeText.SetActive(false);
            return;
        }

        if (PlayerStats.Coins    >= upgrade.baseStrengthCost
            || PlayerStats.Coins >= upgrade.baseDexterityCost
            || PlayerStats.Coins >= upgrade.baseVitalityCost
            || PlayerStats.Coins >= upgrade.baseAttackSpeedCost
            || PlayerStats.Coins >= upgrade.baseArmorCost)
        {
            upgradeImage.material.SetFloat("_OperationBlend_Fade_1", 1f);
            upgradeLight.SetActive(true);
            upgradeText.SetActive(true);
        }
        else
        {
            upgradeLight.SetActive(false);
            upgradeText.SetActive(false);
            upgradeImage.material.SetFloat("_OperationBlend_Fade_1", 0f);
        }
    }
}