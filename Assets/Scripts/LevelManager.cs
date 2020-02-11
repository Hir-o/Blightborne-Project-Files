using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Level Name TMPro Object")] [SerializeField]
    private TextMeshProUGUI levelTMPro;

    [SerializeField] private Animator levelNamePanelAnimator;

    [Header("Screen Transition Material")] [SerializeField]
    private Material cameraTransitionMaterial;

    private Transform villageEntry;
    private static Transform VillageEntry;

    private Transform traderExitPosition;
    private static Transform TraderExitPosition;

    private static string LastSceneName;

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex > 1)
        {
            #if UNITY_WEBGL 
            StartLevelEvent(scene.buildIndex);
            #endif
        }

        //POKI
//        if (scene.buildIndex != 0)
//        {
//            if (GameManager.menu.gameObject.activeSelf)
//                PokiUnitySDK.Instance.gameplayStop();
//            else
//                PokiUnitySDK.Instance.gameplayStart();
//        }

        if (AudioController.EnableMusic)
        {
            if (scene.buildIndex <= 3)
                AudioController.Instance.PlayRandTheme();
            else if (scene.buildIndex >= 4 && scene.buildIndex <= 11)
                AudioController.Instance.PlayFirstDungeonTheme();
            else if (scene.buildIndex >= 12 && scene.buildIndex <= 17)
                AudioController.Instance.PlaySecondDungeonTheme();
            else if (scene.buildIndex >= 18 && scene.buildIndex <= 19)
                AudioController.Instance.PlayPuzzleTheme();
            else if (scene.buildIndex == 20)
                AudioController.Instance.StopAllMusic();
            else if (scene.buildIndex == 21) AudioController.Instance.PlayEscapeTheme();
        }

        if (scene.buildIndex >= 6 && scene.buildIndex <= 11)
            DungeonRecall.FirstDungeonRecallSceneName = SceneManager.GetActiveScene().name;
        else if (scene.buildIndex >= 13 && scene.buildIndex <= 16)
            DungeonRecall.SecondDungeonRecallSceneName = SceneManager.GetActiveScene().name;

        if (PlayerStats.IsFirstPlay)
        {
            if (scene.buildIndex > 4)
            {
                ChestsController.SaveChestsState();
                PlayerStats.SavePlayerState();
            }
        }
        else
        {
            ChestsController.SaveChestsState();
            PlayerStats.SavePlayerState();
        }

        if (scene.buildIndex > PlayerStats.Score)
        {
            PlayerStats.Score = scene.buildIndex;
            PlayerPrefs.SetInt("Score", PlayerStats.Score);
            //Application.ExternalCall("kongregate.stats.submit", "Score", PlayerStats.Score);
        }

        levelTMPro.text = scene.name;
        levelNamePanelAnimator.Play("expand", -1, 0f);

        GameManager.cameraTransition.StartSwipeOut();

        if (FindObjectOfType<RecallPosition>())
        {
            villageEntry = FindObjectOfType<RecallPosition>().transform;
            VillageEntry = villageEntry;

            traderExitPosition = FindObjectOfType<TraderExitPosition>().transform;
            TraderExitPosition = traderExitPosition;

            if (LastSceneName == "Shop")
                Invoke(nameof(TraderExit), .01f);
            else if (LastSceneName != "Rand's House") Invoke(nameof(Teleport), .01f);
        }
    }

    public static void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex + 1 == SceneManager.sceneCountInBuildSettings)
            return;

        LastSceneName = SceneManager.GetActiveScene().name;

        if (LastSceneName == "Dungeon II-II") Physics2D.IgnoreLayerCollision(12, 13, false);

        if (SceneManager.GetActiveScene().buildIndex >= 6 && SceneManager.GetActiveScene().buildIndex <= 11)
            DungeonRecall.FirstDungeonRecallSceneName = SceneManager.GetActiveScene().name;
        else if (SceneManager.GetActiveScene().buildIndex >= 13 && SceneManager.GetActiveScene().buildIndex <= 16)
            DungeonRecall.SecondDungeonRecallSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentSceneIndex + 1);

        PlayerStats.ResetHealth();
        HealthBar.ResetHealthBarStatus();

        PlayerStats.Stamina = PlayerStats.TotalStamina;
        StaminaBar.ResetStaminaBarStatus();
    }

    public static void LoadSceneWithName(string sceneName, bool isPlayerReset)
    {
        LastSceneName = SceneManager.GetActiveScene().name;

        if (LastSceneName == "Dungeon II-II") Physics2D.IgnoreLayerCollision(12, 13, false);

        if (SceneManager.GetActiveScene().buildIndex >= 6 && SceneManager.GetActiveScene().buildIndex <= 11)
            DungeonRecall.FirstDungeonRecallSceneName = SceneManager.GetActiveScene().name;
        else if (SceneManager.GetActiveScene().buildIndex >= 13 && SceneManager.GetActiveScene().buildIndex <= 16)
            DungeonRecall.SecondDungeonRecallSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneName);

        if (isPlayerReset)
        {
            PlayerStats.Health = PlayerStats.TotalHealth;
            PlayerStats.Stamina = PlayerStats.TotalStamina;

            HealthBar.ResetHealthBarStatus();
            StaminaBar.ResetStaminaBarStatus();
        }
    }

    public static void LoadVillageScene()
    {
        SceneManager.LoadScene("Katun Village");

        PlayerStats.Health = PlayerStats.TotalHealth;
        PlayerStats.Stamina = PlayerStats.TotalStamina;

        HealthBar.ResetHealthBarStatus();
        StaminaBar.ResetStaminaBarStatus();
    }

    private void Teleport() { GameManager.player.transform.position = VillageEntry.position; }

    private void TraderExit() { GameManager.player.transform.position = TraderExitPosition.position; }

    private IEnumerator SceneTransitionOut() { yield return new WaitForSeconds(.1f); }
    
    [DllImport("__Internal") ]
    private static extern void StartLevelEvent (int level);
}