using Assets.Scripts.Data.Model.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive = false;
    public AudioSource audioSource;
    public AudioClip gameActiveSound, finishMenuSound, gameOverSound;
    int currentLevel;
    public int score;
    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingMenuMoneyText, gameOverMenuMoneyText, finishMenuMoneyText;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;
    public DailyReward dailyReward;
    public Button rewardedAdButton, soundOnButton, soundOffButton;
    private Firebase.Database.DatabaseReference deviceCurrentRef, deviceLevelsRef;
    // Start is called before the first frame update
    void Start()
    {
        //init
        deviceCurrentRef = Firebase.Database.FirebaseDatabase.DefaultInstance.GetReference($"DeviceCurrent/{DeviceHelper.GetDeviceId()}");
        deviceCurrentRef.KeepSynced(true);

        deviceLevelsRef = Firebase.Database.FirebaseDatabase.DefaultInstance.GetReference($"DeviceLevels/{DeviceHelper.GetDeviceId()}");
        deviceLevelsRef.KeepSynced(true);

        StartCoroutine(SetDatabaseData());

        InitializeLanguageTexts();
        Current = this;
        audioSource = Camera.main.transform.GetComponent<AudioSource>();
        //Get current level from the memory
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        
        PlayerController.Current = GameObject.FindObjectOfType<PlayerController>();

        GameObject.FindObjectOfType<MarketItemController>().InitializeMarketItemController();
        dailyReward.InitializeDailyReward();
        currentLevelText.text = (currentLevel + 1).ToString();
        nextLevelText.text = (currentLevel + 2).ToString();
        UpdateMoneyText();
        
        if(AdController.Current.IsInterstitialAdReady())
        {
            AdController.Current.interstitial.Show();
        }
        AdController.Current.bannerView.Show();
    }
    private IEnumerator SetDatabaseData()
    {
        var t1 = deviceCurrentRef.GetValueAsync();
        yield return new WaitUntil(() => t1.IsCompleted);

        var currentRef = JsonUtility.FromJson<DeviceCurrent>(t1.Result.GetRawJsonValue());
        currentRef.levelNo = currentLevel;
        currentRef.totalPoint += score; 

        var t2 = deviceCurrentRef.SetRawJsonValueAsync(JsonUtility.ToJson(currentRef));
        yield return new WaitUntil(() => t2.IsCompleted);

        var currentLevelRef = deviceLevelsRef.Child(currentLevel.ToString());
        currentLevelRef.KeepSynced(true);

        var t3 = currentLevelRef.GetValueAsync();
        yield return new WaitUntil(() => t3.IsCompleted);

        var deviceLevel = JsonUtility.FromJson<DeviceLevel>(t3.Result.GetRawJsonValue());
        deviceLevel.EndLevel(score);
        var t4 = currentLevelRef.SetRawJsonValueAsync(JsonUtility.ToJson(deviceLevel));
        yield return new WaitUntil(() => t4.IsCompleted);

        var nextLevelRef = deviceLevelsRef.Child(currentRef.levelNo.ToString());
        var t5 = nextLevelRef.SetRawJsonValueAsync(JsonUtility.ToJson(DeviceLevel.Default()));
        yield return new WaitUntil(() => t5.IsCompleted);

    }
    private void InitializeLanguageTexts()
    {
        GameObject[] parentsInScene = this.gameObject.scene.GetRootGameObjects();
        foreach(GameObject parent in parentsInScene)
        {
            TextObject[] textObjectsInParent = parent.GetComponentsInChildren<TextObject>(true);
            foreach(TextObject textObject in textObjectsInParent)
                textObject.InitTextObject();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            //Update progress bar
            PlayerController player = PlayerController.Current;
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;

            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }

    public void StartLevel()
    {
        AdController.Current.bannerView.Hide();
        //Calculate the max distance between character and finish line
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;

        //Change the speed of player
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        audioSource.clip = gameActiveSound;
        audioSource.Play();
        PlayerController.Current.animator.SetBool("Running", true);
        gameActive = true;
    }

    public void RestartLevel()
    {
        //LevelLoader.Current.ChangeLevel(this.gameObject.scene.name);
        LevelLoader.Current.LoadLevelScene();
    }

    public void LoadNextLevel()
    {
        //LevelLoader.Current.ChangeLevel("Level " + (currentLevel + 1).ToString());
        LevelLoader.Current.LoadLevelScene();
        StartCoroutine(SetDatabaseData());
    }

    public void GameOver()
    {
        if(AdController.Current.IsInterstitialAdReady())
        {
            AdController.Current.interstitial.Show();
        }
        AdController.Current.bannerView.Show();
        UpdateMoneyText();
        audioSource.clip = gameOverSound;
        audioSource.Play();
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }

    public void FinishGame()
    {
        if(AdController.Current.rewardedAd.IsLoaded())
            rewardedAdButton.gameObject.SetActive(true);
        else
            rewardedAdButton.gameObject.SetActive(true);

        AdController.Current.bannerView.Show();
        AddMoney(score);
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        audioSource.clip = finishMenuSound;
        audioSource.Play();
        finishScoreText.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }

    public void ChangeScore(int incrementValue)
    {
        score += incrementValue;
        scoreText.text = score.ToString();
    }
    public void AddMoney(int incrementValue)
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + incrementValue);
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyText();
    }
    public void UpdateMoneyText()
    {
        int money = PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text = money.ToString();
        gameOverMenuMoneyText.text = money.ToString();
        finishMenuMoneyText.text = money.ToString();
    }
    public void ShowRewardedAd()
    {
        if(AdController.Current.rewardedAd.IsLoaded())
        {
            AdController.Current.rewardedAd.Show();
        }
    }
    public void TurnTheSoundOnandOff(bool active)
    {
        AudioListener.volume = active ? 1 : 0;
        //Camera.main.GetComponent<AudioListener>().enabled = active;
        soundOnButton.gameObject.SetActive(active);
        soundOffButton.gameObject.SetActive(!active);
    }
}
