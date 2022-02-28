using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;
    public long rewardGivingTimeTicks;
    public GameObject rewardMenu;
    public Text remaningTimeText;
    private long _dailyTicks = 864000000000;

    public void InitializeDailyReward()
    {
        if(PlayerPrefs.HasKey("lastDailyReward"))
        {
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + _dailyTicks;
            long currentTime = DateTime.Now.Ticks;

            if(currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();
            }
        }
        else
        {
            GiveReward();
        }
        initialized = true;
    }
    
    public void GiveReward()
    {
        LevelController.Current.AddMoney(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward", DateTime.Now.Ticks.ToString());
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + _dailyTicks;
    }

    // Update is called once per frame
    void Update()
    {
        if(initialized && LevelController.Current.startMenu.activeInHierarchy)
        {
            long currentTime = DateTime.Now.Ticks;
            long remaningTime = rewardGivingTimeTicks - currentTime;

            if(remaningTime <= 0)
            {
                GiveReward();
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromTicks(remaningTime);
                remaningTimeText.text = timeSpan.ToString(@"hh\:mm\:ss");
            }
        }
    }

    public void TapToReturnButton()
    {
        rewardMenu.SetActive(false);
    }
}
