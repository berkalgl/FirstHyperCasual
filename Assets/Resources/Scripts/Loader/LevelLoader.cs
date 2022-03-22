using Assets.Scripts.Data.Model.Database;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Slider slider;
    public static LevelLoader Current;
    private Scene _lastLoadedScene;
    private bool complete = false;
    void Start()
    {
        Current = this;
        GameObject.FindObjectOfType<AdController>().InitializeAds();
        //PlayerPrefs.SetInt("currentLevel", 0);
        Firebase.Database.FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        StartCoroutine(DeviceInitializion());
        //ChangeLevel("Level " + PlayerPrefs.GetInt("currentLevel"));
    }
    void Update()
    {
        slider.value = Mathf.Clamp01(slider.value + Time.deltaTime * 0.5f);
        if (complete)
        {
            if (slider.value >= 1f)
                LoadLevelScene();
        }
    }
    public void LoadLevelScene()
    {
        SceneManager.LoadScene("Level");
    }

    IEnumerator DeviceInitializion()
    {
        var instance = Firebase.Database.FirebaseDatabase.DefaultInstance;
        var deviceCurrentRef = instance.GetReference($"DeviceCurrent/{DeviceHelper.GetDeviceId()}");
        deviceCurrentRef.KeepSynced(true);
        var task = deviceCurrentRef.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result?.GetRawJsonValue() == null)
        {
            var t1 = deviceCurrentRef.SetRawJsonValueAsync(JsonUtility.ToJson(DeviceCurrent.Default()));
            yield return new WaitUntil(() => t1.IsCompleted);
            var t2 = instance.GetReference($"DeviceMain/{DeviceHelper.GetDeviceId()}").SetRawJsonValueAsync(JsonUtility.ToJson(DeviceMain.GetMain()));
            yield return new WaitUntil(() => t2.IsCompleted);
        }
        else
        {
            var deviceCurrent = JsonUtility.FromJson<DeviceCurrent>(task.Result.GetRawJsonValue());
            DeviceCache.SetGameLevel(deviceCurrent.levelNo);
            DeviceCache.SetGameTotalPoint(deviceCurrent.totalPoint);
        }

        complete = true;

        yield return new WaitUntil(() => complete == true);
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
