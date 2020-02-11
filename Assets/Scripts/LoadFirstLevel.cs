using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstLevel : MonoBehaviour
{
    [SerializeField] private GameObject button;
    
    public void DisplayStartGameButton()
    {
        button.SetActive(true);
    }
    
    public void LoadGameLevel() { SceneManager.LoadScene(1); }
}
