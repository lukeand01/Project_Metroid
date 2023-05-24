using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [HideInInspector]public GameObject holder;
    [HideInInspector] public InventoryDrag drag;
    public MerchantUI merchant;

    [SerializeField] GameObject itemHolder;
    [SerializeField] GameObject template;

    [SerializeField] GameObject secondaryHolder;
    [SerializeField] GameObject secondaryItemHolder;

    List<InventoryUnit> slotsList = new List<InventoryUnit>();

    [Separator("Keys")]
    [SerializeField] KeyUnit blackKey;
    [SerializeField] KeyUnit fireKey;

    #region EVENTS
    public event Action<int> EventSelectedInventoryUnit;
    public void OnSelectedInventoryUnit(int index) => EventSelectedInventoryUnit?.Invoke(index);

    public event Action<ItemClass> EventHoverItem;
    public void OnHoverItem(ItemClass item) => EventHoverItem?.Invoke(item);

    public event Action<ItemClass> EventClickItem;
    public void OnClickItem(ItemClass item) => EventClickItem?.Invoke(item);

    #endregion

    private void Start()
    {

        holder = transform.GetChild(0).gameObject;
        drag = GetComponent<InventoryDrag>();

        Observer.instance.EventOpenChest += ChestOrder;
        ResetAllKeys();
    }

    
   public void ControlUI()
    {
        if (holder.activeInHierarchy)
        {
            holder.SetActive(false);
            //and if the merchant ui is active then we close that too.

            if (merchant.gameObject.activeInHierarchy)
            {
                CloseMerchant();
            }

            Observer.instance.OnShowDescription(Vector2.zero, "", false);
            PlayerHandler.instance.MouseVisible(false);
        }
        else
        {
            holder.SetActive(true);
            PlayerHandler.instance.MouseVisible(true);
        }
    }


    //we want to update the inventory now.

    public void SetUp(int quantity)
    {
        //create a bunch of 

        for (int i = 0; i < quantity; i++)
        {
           InventoryUnit inventory = CreateItem(null, i);
            slotsList.Add(inventory);
        }
    }

    public void UpdateItem(ItemClass item, int index)
    {
        slotsList[index].SetUp(item, this, index, true);
    }
  
    public void UpdateInventory(List<ItemClass> inventoryList)
    {

        //clear all itens. put new itens.
        for (int i = 0; i < slotsList.Count; i++)
        {
            slotsList[i].Clear();
        }

        for (int i = 0; i < inventoryList.Count; i++)
        {
            slotsList[i].SetUp(inventoryList[i], this, i, true);
        }

    }
    //not interested about adding item. rather i want to place them.
    

    InventoryUnit CreateItem(ItemClass item, int index)
    {
        GameObject newObject = Instantiate(template, itemHolder.transform.position, Quaternion.identity);
        newObject.gameObject.SetActive(true);
        newObject.transform.parent = itemHolder.transform;
        InventoryUnit inventory = newObject.GetComponent<InventoryUnit>();
        inventory.SetUp(item, this, index, true);
        return inventory;
    }



    #region CHANGE FUNCTIONS

    public void RemoveItemOrder(int index, int change)
    {
        for (int i = 0; i < change; i++)
        {
            RemoveItem(index);
        }
    }

    public void RemoveItem(int index)
    {
        //remove one entity of item


    }



    #endregion


    #region SECONDARY INVENTORY
    //i dont want player to use chests to store stuff.
    //they will have two to one itens but you cannot place itens inside.
    //wont create empty slots.
    //cannot place itens in the chest.

    //



    Chest currentChest;
    void ChestOrder(List<ItemClass> itemList, Chest chest)
    {
        currentChest = chest;
        if(itemList == null)
        {
            //then we close
            secondaryHolder.SetActive(false);
            List<ItemClass> newInventory = GetItemList();
            chest.ReceiveInventory(newInventory);
        }
        else
        {
            holder.SetActive(true);
            secondaryHolder.SetActive(true);
            CreateChestItens(itemList);
        }
    }


    void CreateChestItens(List<ItemClass> itemList)
    {
        ClearChest();

        for (int i = 0; i < itemList.Count; i++)
        {
            GameObject newObject = Instantiate(template, secondaryItemHolder.transform.position, Quaternion.identity);
            newObject.SetActive(true);
            newObject.transform.parent = secondaryItemHolder.transform;
            newObject.GetComponent<InventoryUnit>().SetUp(itemList[i], this, i, false);

        }
    }

    void ClearChest()
    {
        for (int i = 0; i < secondaryItemHolder.transform.childCount; i++)
        {
            Destroy(secondaryItemHolder.transform.GetChild(i).gameObject);
        }
    }

    //i dont need to read the ui. just mount some information about index and position.

    public void CheckChestEmpty()
    {
        List<ItemClass> itemList = GetItemList();
        if(itemList.Count <= 0)
        {
            if(currentChest == null)
            {
                Debug.LogError("No chest here?");
                return;
            }

            secondaryHolder.SetActive(false);           
            currentChest.ReceiveInventory(itemList);
        }
        
    }

    List<ItemClass> GetItemList()
    {
        //get all items from the 
        List<ItemClass> itemList = new List<ItemClass>();
        for (int i = 0; i < secondaryItemHolder.transform.childCount; i++)
        {
            GameObject child = secondaryItemHolder.transform.GetChild(i).gameObject;
            InventoryUnit unit = child.GetComponent<InventoryUnit>();

            if (unit.GetItem() != null)
            {
                //then we get the item
                itemList.Add(unit.GetItem());
            }
        }

        return itemList;
    }

    public List<ItemClass> CreateNewList()
    {
        List<ItemClass> newList = new List<ItemClass>();

        for (int i = 0; i < itemHolder.transform.childCount; i++)
        {
            GameObject child = itemHolder.transform.GetChild(i).gameObject;
            InventoryUnit unit = child.GetComponent<InventoryUnit>();

            newList.Add(unit.GetItem());


        }

        return newList;
    }

    #endregion


    #region MERCHANT
    public bool tradeMerchant;


    //its about trading.


    public void SetUpMerchant(List<ItemClass> itemList) => merchant.SetUpMerchant(itemList, this);
   
    public void OpenMerchant()
    {
        tradeMerchant = true;
        holder.SetActive(true); //open inventotry.
        PlayerHandler.instance.MouseVisible(true);
        merchant.gameObject.SetActive(true);
        merchant.OpenMerchant();

        EventClickItem += merchant.MerchantClick;
        EventHoverItem += merchant.MerchantHover;

    }

    public void AddItemToTrade(ItemClass item) => merchant.AddItemToTrade(item, true); //everytime this is called is the player calling it.

    public void CloseMerchant()
    {
        tradeMerchant = false;
        merchant.gameObject.SetActive(false);
        
        EventClickItem -= merchant.MerchantClick;
        EventHoverItem -= merchant.MerchantHover;

        holder.gameObject.SetActive(false);
    }


    
    //merchant interact to hover, click and trading.


    void SubscribeMerchant()
    {

    }
    void UnsubscribeMerchant()
    {

    }


    #endregion


    #region KEY

    void ResetAllKeys()
    {
        blackKey.Control(false);
        fireKey.Control(false);
    }

    public void HandleKeyUI(int id)
    {
        if(id == 0)
        {
            blackKey.Control(true);
        }

        if(id == 1)
        {
            fireKey.Control(true);
        }
    }



    #endregion

}
