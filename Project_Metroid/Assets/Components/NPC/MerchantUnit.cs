using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MerchantUnit : ButtonBase
{
    MerchantUI handler;
    ItemClass item;

    [SerializeField] Image portrait;
    [SerializeField] GameObject holder;

   public void SetUp(ItemClass item, MerchantUI handler)
    {
        this.item = item;
        this.handler = handler;
        portrait.sprite = item.data.sprite;
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        //when you click on this we sent it to the trading bar.
        handler.AddItemToTrade(item);
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        holder.SetActive(true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        holder.SetActive(false);
    }

    //they should have some kind of infomration about how rare they are.

}

