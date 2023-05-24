using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShrineUnit : ButtonBase
{
    ShrineUI handler;
    [SerializeField] Image sprite;
    [SerializeField] GameObject select;
    public ConsumableClass bd;
    [HideInInspector]public int index;


    //i will just throw the sprites somewhere and fuck it.


    public void SetUp(ShrineUI handler, ConsumableClass bd, int index)
    {
        //where to get the sprite?
        this.handler = handler;
        this.bd = bd;
        Debug.Log("bd type " + bd.consumableType);

        this.index = index;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //give the bonus.
        //be it key, buff or etc.
        handler.ChooseBuff(this);
    }

    string CreateShow()
    {
        if(bd.consumableType == ConsumableClass.BDType.PermaMaxHealth)
        {
            return "Increase your max health";
        }
        if (bd.consumableType == ConsumableClass.BDType.PermaStamina)
        {
            return "Increase your max Stamina";
        }
        if(bd.consumableType == ConsumableClass.BDType.PermaSpeed)
        {
            return "Increase your running speed";
        }

        if(bd.consumableType == ConsumableClass.BDType.StaminaCost)
        {
            return "All stamina cost abilities cost less";
        }

        if (bd.consumableType == ConsumableClass.BDType.UseSpeed)
        {
            return "All itens are used faster";
        }

        if (bd.consumableType == ConsumableClass.BDType.PermaDamage)
        {
            return "Your sword deals more damage";
        }

        if (bd.consumableType == ConsumableClass.BDType.StaminaCost)
        {
            return "All stamina cost abilities cost less";
        }
        if (bd.consumableType == ConsumableClass.BDType.AbilityCooldown)
        {
            return "Abilities have a lower cooldown";
        }
        if (bd.consumableType == ConsumableClass.BDType.Luck)
        {
            return "get more stuff from chests";
        }

        return "";
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("enter");
        Observer.instance.OnShowDescription(transform.position, CreateShow(), false);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        Observer.instance.OnShowDescription(transform.position, "", false);
    }

    public void HandleSelected(bool choice)
    {
        select.SetActive(choice);
    }

}
