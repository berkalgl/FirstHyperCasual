using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    // 0: hasnt bought yet
    // 1: bought but not equipped
    // 2: equipped
    public int id, categoryId;
    public int price;
    public Text priceText;
    public Button buyButton, equipButton, unequipButton;
    public bool PlayerHasItem() => PlayerPrefs.GetInt("item" + id.ToString()) != 0;

    public bool PlayerEquippedItem() => PlayerPrefs.GetInt("item" + id.ToString()) == 2;

    public void InitializeItem()
    {
        priceText.text = price.ToString();

        if(PlayerHasItem())
        {
            buyButton.gameObject.SetActive(false);
            if(PlayerEquippedItem())
            {
                EquipItem();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }
    public void BuyButton()
    {
        if(!PlayerHasItem())
        {
            int money = PlayerPrefs.GetInt("money");
            if(money >= price)
            {
                PlayerController.Current.audioSource.PlayOneShot(PlayerController.Current.buyAudioClip,0.1f);
                LevelController.Current.AddMoney(-price);
                PlayerPrefs.SetInt("item" + id.ToString(), 1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }

    }
    public void EquipItem()
    {
    }
    public void UnequipItem()
    {
        
    }
}
