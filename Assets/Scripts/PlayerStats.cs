using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    private static Pendant Pendant;
    private Pendant pendant;

    public static float Health;
    public static float Stamina;
    public static int DropRate = 1;
    public static int Coins = 0;
    public static int CheckpointPriority;
    public static Vector2 CheckpointLocation;
    public static bool IsRolling = false;
    public static bool IsFirstPlay = true;

    public float health = 100;
    public float stamina = 100;

    public static float TotalHealth;
    public static float TotalStamina;  
    
    //score
    public static int Score;

    //Multipliers
    public static int StrengthMultiplier = 1;
    public static int ArmorMultiplier = 5;
    public static int AttackSpeedMultiplier = 5;

    [Header("Upgradeable Stats")]
    public static int Strength = 0;
    public static int Dexterity = 0;
    public static int Vitality = 0;
    public static int AttackSpeed = 0;
    public static int Armor = 0;

    [Header("Collectables")]
    public static bool HasBlueKey;
    public static bool HasRedKey;
    public static bool HasBlueGem;
    public static bool HasRedGem;

    [Header("Abilities")]
    public static int CurrentAbilitySelctedIndex = 0;
    public static int MaxAbilityIndex = 0;
    public static bool UnlockPowerArrow, UnlockIceArrow, UnlockFireArrow, UnlockSpecialArrow;

    [Header("Coin Sack")]
    public bool disableBagSystem;
    public static bool DisableBagSystem; 
    public static int CoinSackIndex = 0;
    public static int MaxCoinCapacity = 50;

    [Header("Purchasable Items & Affecting Stats")]
    public static bool IsPendantPurchased;
    public static bool IsRingPurchased;
    public static bool IsPyramidPurchased, IsEnergyPyramidPurchased;
    [SerializeField] private float fallingDragLevel = 4f;
    public static float FallingDragLevel;
    [SerializeField] private float pendantRechargeDelay = 180f;
    public static float PendantRechargeDelay;
    public static bool IsPendantReady;

    private int currentSceneIndex;

    [Header("Poki Code")]
    public static bool IsCommercialBreakInitialized;

    public static bool IsFirstAddChestCollected,
                       IsSecondAddChestCollected,
                       IsThirdAddChestCollected,
                       IsFourthAddChestCollected;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        TotalHealth = health + (Vitality * 20f);
        TotalStamina = stamina + ( Dexterity * 20f);

        Health = TotalHealth;
        Stamina = TotalStamina;
        DisableBagSystem = disableBagSystem;
        FallingDragLevel = fallingDragLevel;
        PendantRechargeDelay = pendantRechargeDelay;
    }

    private void Start()
    {
        CheckpointPriority = 0;
        CheckpointLocation = FindObjectOfType<Player>().gameObject.transform.position;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        pendant = FindObjectOfType<Pendant>();
        Pendant = pendant;

        LoadIsFirstPlay();

        UpdateHealth();
        UpdateStamina();
    }

    public static void ResetHealth()
    {
        Health = TotalHealth;
    }

    public static void ResetStamina()
    {
        Stamina = TotalStamina;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {        
        if (currentSceneIndex == SceneManager.GetActiveScene().buildIndex) { return; }
        CheckpointPriority = 0;
        CheckpointLocation = FindObjectOfType<Player>().gameObject.transform.position;

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void UpdateHealth()
    {
        TotalHealth = health + (Vitality * 20f);
        Health = TotalHealth;
    }

    public void UpdateStamina()
    {
        TotalStamina = stamina + (Dexterity * 20f);
        Stamina = TotalStamina;
    }

    public static void AddHealth(int percentageAmount)
    {
        Health += (TotalHealth / 100) * percentageAmount;

        if (Health > TotalHealth)
            Health = TotalHealth;

        HealthBar.ResetHealthBarStatus();
    }

    public static void RestoreHealthFromPendant()
    {
        Health = TotalHealth;
        HealthBar.ResetHealthBarStatus();

        IsPendantReady = false;

        Pendant.InitializeReset(PendantRechargeDelay);
    }

    public static void SavePlayerState()
    {
        PlayerPrefs.SetInt("IsFirstPlay", (IsFirstPlay ? 1 : 0));

        // Collectables
        PlayerPrefs.SetInt("Blue Key", (HasBlueKey ? 1 : 0));
        PlayerPrefs.SetInt("Red Key", (HasRedKey ? 1 : 0));
        PlayerPrefs.SetInt("Blue Gem", (HasBlueGem ? 1 : 0));
        PlayerPrefs.SetInt("Red Gem", (HasRedGem ? 1 : 0));
        PlayerPrefs.SetInt("Coins", Coins);

        //Items
        PlayerPrefs.SetInt("Pendant", (IsPendantPurchased ? 1 : 0));
        PlayerPrefs.SetInt("Ring", (IsRingPurchased ? 1 : 0));
        PlayerPrefs.SetInt("Pyramid", (IsPyramidPurchased ? 1 : 0));
        PlayerPrefs.SetInt("EnergyPyramid", (IsEnergyPyramidPurchased ? 1 : 0));
        

        //Abilities
        PlayerPrefs.SetInt("Strength", Strength);
        PlayerPrefs.SetInt("Dexterity", Dexterity);
        PlayerPrefs.SetInt("Vitality", Vitality);
        PlayerPrefs.SetInt("AttackSpeed", AttackSpeed);
        PlayerPrefs.SetInt("Armor", Armor);

        //Skills
        PlayerPrefs.SetInt("UnlockPowerArrow", (UnlockPowerArrow ? 1 : 0));
        PlayerPrefs.SetInt("UnlockIceArrow", (UnlockIceArrow ? 1 : 0));
        PlayerPrefs.SetInt("UnlockFireArrow", (UnlockFireArrow ? 1 : 0));
        PlayerPrefs.SetInt("UnlockSpecialArrow", (UnlockSpecialArrow ? 1 : 0));
        PlayerPrefs.SetInt("MaxAbilityIndex", MaxAbilityIndex);

        //Last dungeon level
        PlayerPrefs.SetString("FirstDungeonRecallSceneName", DungeonRecall.FirstDungeonRecallSceneName);
        PlayerPrefs.SetString("SecondDungeonRecallSceneName", DungeonRecall.SecondDungeonRecallSceneName);     
        
        //POKI Add Chests
        PlayerPrefs.SetInt("IsFirstAddChestCollected", (IsFirstAddChestCollected ? 1 : 0));
        PlayerPrefs.SetInt("IsSecondAddChestCollected", (IsSecondAddChestCollected ? 1 : 0));
        PlayerPrefs.SetInt("IsThirdAddChestCollected", (IsThirdAddChestCollected ? 1 : 0));
        PlayerPrefs.SetInt("IsFourthAddChestCollected", (IsFourthAddChestCollected ? 1 : 0));
    }

    public static void LoadIsFirstPlay()
    {
        if (PlayerPrefs.HasKey("IsFirstPlay"))
            IsFirstPlay = Convert.ToBoolean(PlayerPrefs.GetInt("IsFirstPlay"));
    }

    public static void LoadPlayerState()
    {
        //Score
        Score = PlayerPrefs.GetInt("Score");
        
        //Collectables
        Coins = PlayerPrefs.GetInt("Coins");
        HasBlueKey = PlayerPrefs.GetInt("Blue Key") == 1;
        HasRedKey = PlayerPrefs.GetInt("Red Key") == 1;
        HasBlueGem = PlayerPrefs.GetInt("Blue Gem") == 1;
        HasRedGem = PlayerPrefs.GetInt("Red Gem") == 1;

        //Items
        IsPendantPurchased = PlayerPrefs.GetInt("Pendant") == 1;
        IsRingPurchased = PlayerPrefs.GetInt("Ring") == 1;
        IsPyramidPurchased = PlayerPrefs.GetInt("Pyramid") == 1;
        IsEnergyPyramidPurchased = PlayerPrefs.GetInt("EnergyPyramid") == 1;

        if (IsPendantPurchased)
            IsPendantReady = true;

        //Abilities
        Strength = PlayerPrefs.GetInt("Strength");
        Dexterity = PlayerPrefs.GetInt("Dexterity");
        Vitality = PlayerPrefs.GetInt("Vitality");
        AttackSpeed = PlayerPrefs.GetInt("AttackSpeed");
        Armor = PlayerPrefs.GetInt("Armor");

        //Skills
        UnlockPowerArrow = PlayerPrefs.GetInt("UnlockPowerArrow") == 1;
        UnlockIceArrow = PlayerPrefs.GetInt("UnlockIceArrow") == 1;
        UnlockFireArrow = PlayerPrefs.GetInt("UnlockFireArrow") == 1;
        UnlockSpecialArrow = PlayerPrefs.GetInt("UnlockSpecialArrow") == 1;
        MaxAbilityIndex = PlayerPrefs.GetInt("MaxAbilityIndex");
        AbilityChooser.Instance.UpdateTextures();
        
        //POKI Add Chests
        IsFirstAddChestCollected = PlayerPrefs.GetInt("IsFirstAddChestCollected") == 1;
        IsSecondAddChestCollected = PlayerPrefs.GetInt("IsSecondAddChestCollected") == 1;
        IsThirdAddChestCollected = PlayerPrefs.GetInt("IsThirdAddChestCollected") == 1;
        IsFourthAddChestCollected = PlayerPrefs.GetInt("IsFourthAddChestCollected") == 1;

        //Last dungeon level
        DungeonRecall.FirstDungeonRecallSceneName = PlayerPrefs.GetString("FirstDungeonRecallSceneName", "none");
        DungeonRecall.SecondDungeonRecallSceneName = PlayerPrefs.GetString("SecondDungeonRecallSceneName", "none");

        if (DungeonRecall.FirstDungeonRecallSceneName == "none" || DungeonRecall.FirstDungeonRecallSceneName == "")
            DungeonRecall.FirstDungeonRecallSceneName = null;
        if (DungeonRecall.SecondDungeonRecallSceneName == "none" || DungeonRecall.SecondDungeonRecallSceneName == "")
            DungeonRecall.SecondDungeonRecallSceneName = null;

        EssentialObjects.UpdateCoinsStatic();
        GameManager.hud.UpdateHUD();
        GameManager.hud.UpdateItemsIcons();
    }

    public static void ResetPlayerState()
    {
        PlayerPrefs.SetInt("IsFirstPlay", 1);
        IsFirstPlay = true;

        //Collectables
        Coins = 0;
        HasBlueKey = false;
        HasRedKey = false;
        HasBlueGem = false;
        HasRedGem = false;

        //Items
        IsPendantPurchased = false;
        IsRingPurchased = false;
        IsPyramidPurchased = false;
        IsEnergyPyramidPurchased = false;

        if (IsPendantPurchased)
            IsPendantReady = true;

        //Abilities
        Strength = 0;
        Dexterity = 0;
        Vitality = 0;
        AttackSpeed = 0;
        Armor = 0;

        //Skills
        UnlockPowerArrow = false;
        UnlockIceArrow = false;
        UnlockFireArrow = false;
        UnlockSpecialArrow = false;
        
        CurrentAbilitySelctedIndex = 0;
        MaxAbilityIndex = 0;
        AbilityChooser.Instance.UpdateTextures();
        
        //POKI Add Chests
        IsFirstAddChestCollected = false;
        IsSecondAddChestCollected = false;
        IsThirdAddChestCollected = false;
        IsFourthAddChestCollected = false;

        //Last dungeon level
        DungeonRecall.FirstDungeonRecallSceneName = null;
        DungeonRecall.SecondDungeonRecallSceneName = null;

        EssentialObjects.UpdateCoinsStatic();
        GameManager.hud.UpdateHUD();
        GameManager.hud.UpdateItemsIcons();
    }

    public static void ReduceStamina (float staminaToReduce)
    {
        Stamina -= (staminaToReduce * AssistController.StaminaDrain / 100);
    }
}
