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
    int score;

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingMenuMoneyText, gameOverMenuMoneyText, finishMenuMoneyText;
    public Slider levelProgressBar;

    public float maxDistance;
    public GameObject finishLine;
    public DailyReward dailyReward;

    // Start is called before the first frame update
    void Start()
    {
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
        LevelLoader.Current.ChangeLevel(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        LevelLoader.Current.ChangeLevel("Level " + (currentLevel + 1).ToString());
    }

    public void GameOver()
    {
        UpdateMoneyText();
        audioSource.clip = gameOverSound;
        audioSource.Play();
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }

    public void FinishGame()
    {
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
}
