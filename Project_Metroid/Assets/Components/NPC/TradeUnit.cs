using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeUnit : ButtonBase
{
    bool isPlayer;
    [HideInInspector]public ItemClass item;
    MerchantUI handler;

    [SerializeField] GameObject hover;
    [SerializeField] Image portrait;
    [SerializeField] TextMeshProUGUI quantityText;

    //this is just a representation of the 

    public void SetUp(ItemClass item, MerchantUI handler, bool isPlayer)
    {
        this.item = item;
        this.handler = handler;
        this.isPlayer = isPlayer;
        UpdateUI();

    }

    public void UpdateUI()
    {
        portrait.sprite = item.data.sprite;
        quantityText.text = item.quantity.ToString();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //we return this item to the thing.
        //take one from this and remove it if its 0.
        //and i have to give it back.
        handler.RemoveItemToTrade(item, isPlayer);
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        hover.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        hover.SetActive(false);
    }


    //interacting with this 


}
