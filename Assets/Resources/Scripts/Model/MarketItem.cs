using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    // 0: hasnt bought yet
    // 1: bought but not equipped
    // 2: equipped

    //category id : 0 => hat, 1 =>
    public int id, categoryId;
    public int price;
    public Text priceText;
    public Button buyButton, equipButton, unequipButton;
    public GameObject marketItemPrefab;
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
                PlayerController.Current.audioSource.PlayOneShot(PlayerController.Current.buyItemAudioClip,0.1f);
                LevelController.Current.AddMoney(-price);
                PlayerPrefs.SetInt("item" + id.ToString(), 1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }

    }
    public void EquipItem()
    {
        UnequipItem();
        MarketItemController.Current.equippedItems[categoryId] = Instantiate(marketItemPrefab, PlayerController.Current.wearablePlaces[categoryId].transform).GetComponent<MarketItem3D>();
        MarketItemController.Current.equippedItems[categoryId].marketItemId = id;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + id.ToString(), 2);
    }
    public void UnequipItem()
    {
        //check if there is a item equipped in that category
        MarketItem3D equippedItem = MarketItemController.Current.equippedItems[categoryId];
        if(equippedItem != null)
        {
            MarketItem marketItem = MarketItemController.Current.marketItems[equippedItem.marketItemId];
            PlayerPrefs.SetInt("item" + marketItem.id.ToString(), 1);
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);

        }
    }
    public void EquipItemButton()
    {
        PlayerController.Current.audioSource.PlayOneShot(PlayerController.Current.equipItemAudioClip,0.1f);
        EquipItem();
    }
    public void UnequipItemButton()
    {
        PlayerController.Current.audioSource.PlayOneShot(PlayerController.Current.unequipItemAudioClip,0.1f);
        UnequipItem();
    }
}
