using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadSceneAfterVideoFinished : MonoBehaviour
{
    public VideoPlayer VideoPlayer; // Drag & Drop the GameObject holding the VideoPlayer component
    public string SceneName ;
    public VideoPlayer videoPlayer;
    
    void Start() 
    {
        videoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath,"armorSplash2.ogv"); 
        videoPlayer.Prepare();
        videoPlayer.Play();
        
        VideoPlayer.loopPointReached += LoadScene;
    }
    void LoadScene(VideoPlayer vp)
    {
        SceneManager.LoadScene( SceneName );
    }
}
