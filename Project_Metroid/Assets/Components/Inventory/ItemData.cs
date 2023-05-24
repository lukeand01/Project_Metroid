using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static ItemData;

[CreateAssetMenu(menuName = "Item/Basic")]
public class ItemData : ScriptableObject
{
    //this is the base.
    //resources, medical, tools, weapons
    [Separator("BASE ITEM")]
    public string itemName;
    public ItemType itemType;
    public Sprite sprite;
    public bool canStack;
    public int itemValue;
    [ConditionalField(nameof(canStack), false)] public int stackLimit;
    [TextArea] public string description;
    [Separator("CONSUMABLE")]
    [ConditionalField(nameof(itemType), false, ItemType.Consumable)] public float useDuration;
    [ConditionalField(nameof(itemType), false, ItemType.Consumable)] public ConsumableHolder consumableList;



    //weapon data.
    //tools also deal damage.


    public enum ItemType
    {
        Expandable,
        Consumable,
        Flesh

    }
    public void HandleConsume(ConsumableClass consumable, PlayerHandler handler)
    {

        if (consumable.consumableType == ConsumableClass.BDType.PermaDamage)
        {
            //
        }
        if (consumable.consumableType == ConsumableClass.BDType.TempDamage)
        {
            //
        }
        if (consumable.consumableType == ConsumableClass.BDType.Health)
        {
            //just increase health
        }
        if (consumable.consumableType == ConsumableClass.BDType.TempSpeed)
        {

        }
        if (consumable.consumableType == ConsumableClass.BDType.PermaSpeed)
        {
            //
        }
    }

  
}

//what kind of itens do i want?
//maybe torch isnt equippable. is locked to a button and gained in game.
//consumable: food, potions,sword refinining,  Elder egg(gives power)
//
[System.Serializable]
public class ConsumableHolder
{
    public List<ConsumableClass> consumableList;
}



[System.Serializable]
public class ItemClass
{
    public ItemData data;
    public int quantity;
    public ItemClass(ItemData data, int quantity = 1)
    {
        this.data = data;
        this.quantity = quantity;
    }
    public bool IsEquippable()
    {
        return true;
    }


    



    #region QUANTITY

    public bool IsFull()
    {
        return quantity >= StackLimit();
    }
    public void AddToLimit()
    {
        int difference = StackLimit() - quantity;
        AddQuantity(difference);

    }
    public int StackUpperDifference() //tell how much is left till it gets to full stack.
    {
        return data.stackLimit - quantity;
    }
    public int GetLowerDifference(int targetUpperValue) //tells how much it went past the target stack.
    {
        return targetUpperValue - quantity;
    }
    public int MaxValue()
    {
        if (quantity > StackLimit())
        {
            return StackLimit();
        }
        else
        {
            return quantity;
        }

    }
    public int StackLimit()
    {
        return data.stackLimit;
    }
    public bool CanExist()
    {
        return quantity > 0;
    }
    public void AddQuantity(int change = 1)
    {
        if (change < 0)
        {
            Debug.LogError("something wrong happened. negative number");
        }
        quantity += change;
    }
    public void RemoveQuantity(int change = 1)
    {
        if(change < 0)
        {
            Debug.LogError("something wrong happened. negative number");
        }
        quantity -= change;
    }
    #endregion

}


