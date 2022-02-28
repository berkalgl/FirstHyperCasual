using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketItemController : MonoBehaviour
{
    public static MarketItemController Current;
    public List<MarketItem> marketItems;
    public List<MarketItem3D> equippedItems;
    public GameObject marketMenu;

    public void InitializeMarketItemController()
    {
        Current = this;

        foreach(MarketItem marketItem in marketItems)
        {
            marketItem.InitializeItem();
        }
    }

    public void ActivateMarketMenu(bool active) => marketMenu.SetActive(active);
}
