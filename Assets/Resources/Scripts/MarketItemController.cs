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

    public void ActivateMarketMenu(bool active) 
    {
        marketMenu.SetActive(active);
        ChangeCameraPosition(active);
    }
    private void ChangeCameraPosition(bool marketMenuActive)
    {
        if (marketMenuActive)
        {
            Camera.main.transform.localPosition = new Vector3(0, 1.17f, 1.23f);
            Camera.main.transform.localRotation = Quaternion.Euler(18.2f, -180.4f, 0);
        }
        else
        {
            Camera.main.transform.localPosition = new Vector3(0, 2.3f, -3.712f);
            Camera.main.transform.localRotation = Quaternion.Euler(18f, 0, 0);
        }
    }
}
