using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Current;
    private Scene _lastLoadedScene;
    void Start()
    {
        Current = this;
        GameObject.FindObjectOfType<AdController>().InitializeAds();
        //PlayerPrefs.SetInt("currentLevel", 0);
        LoadLevelScene();
        //ChangeLevel("Level " + PlayerPrefs.GetInt("currentLevel"));
    }

    public void LoadLevelScene()
    {
        SceneManager.LoadScene("Level");
    }
    // public void ChangeLevel(string sceneName)
    // {   
    //     StartCoroutine(ChangeScene(sceneName));
    // }

    //for multiple scenes
    // IEnumerator ChangeScene(string sceneName)
    // {
    //     if(_lastLoadedScene.IsValid())
    //     {
    //         SceneManager.UnloadSceneAsync(_lastLoadedScene);
    //         bool sceneUnloaded = false;

    //         while (!sceneUnloaded)
    //         {
    //             sceneUnloaded = !_lastLoadedScene.IsValid();
    //             yield return new WaitForEndOfFrame();
    //         }
    //     }

    //     SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    //     bool sceneLoaded = false;

    //     while(!sceneLoaded)
    //     {
    //         _lastLoadedScene = SceneManager.GetSceneByName(sceneName);
    //         sceneLoaded = _lastLoadedScene != null && _lastLoadedScene.isLoaded;
    //         yield return new WaitForEndOfFrame();
    //     }
    // }
}