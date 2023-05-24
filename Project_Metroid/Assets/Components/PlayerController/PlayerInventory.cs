using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, ISaveable
{

    //i wont have multiple slots. it wont be used.
    [SerializeField] InventorySave inventorySave;


    public List<ItemClass> inventoryList = new List<ItemClass>();
    public List<ItemClass> initialItensList = new List<ItemClass>();

    public int inventoryLimit = 15;


    [Separator("FLESH EXAMPLES")]
    [SerializeField] ItemData rottenFlesh;
    [SerializeField] ItemData healthyFlesh;
    [SerializeField] ItemData brainFlesh;
    [SerializeField] ItemData heartFlesh;

     public int souls;

    //the merchant seeks flesh 
    //itens:heart = 10, brain = 6, flesh = 2, putrid flesh = 1 



    PlayerHandler handler;
    public void SetUp(PlayerHandler handler)
    {
        this.handler = handler;
        InitializeInventory();
        Observer.instance.OnUpdateSoul(souls);
        //i will tell the inventory to add empty slots.
    }

 
    public void GainSoul(int value)
    {

        souls += value;
        Observer.instance.OnUpdateSoul(souls);
    }

    public void AddFlesh(List<FleshClass> fleshList)
    {
        int random = UnityEngine.Random.Range(0, 100);

        //the first is always the most rare.


        for (int i = 0; i < fleshList.Count; i++)
        {
            //we check each one starting by the first of lower value.

            if (random > fleshList[i].chance)
            {
                ItemClass item = new ItemClass(fleshList[i].flesh);
                AddItem(item);
                handler.Warn("Got a " + fleshList[i].flesh.itemName, true);
                return;
            }

        }

        handler.Warn("Nothing");

    }

    void InitializeInventory()
    {
        handler.inventoryUI.SetUp(inventoryLimit);
        for (int i = 0; i < inventoryLimit; i++)
        {
            inventoryList.Add(null);
        }

        for (int i = 0; i < initialItensList.Count; i++)
        {
            if (CanAdd(initialItensList[i]))
            {
                ReceiveItem(initialItensList[i]);
            }
            else
            {
               UnityEngine.Debug.LogError("Cant add initial itens?");
            }
        }

    }

    #region HELPERS
    public bool CanAdd(ItemClass item) //this is the most basic questioning.
    {
        //need to know if i can stack it somewhere.
        //if i have space.
        if (!ThereIsSpace())
        {
            if (CanStack(item) == -1) return false;
            else return true;
        }
        return true;

    }
    //get as many as you can. as long as you can stack anywhere its fine.
    public int CanStack(ItemClass item)
    {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i] == null) continue;
            if (inventoryList[i].data == item.data)
            {
                if (!inventoryList[i].IsFull()) return i;

            }

        }

        return -1;
    }

    public bool ThereIsSpace()
    {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i] == null) return true;
        }
        return false;
    }

    public int GetFreeSpace()
    {

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i] == null)
            {

                return i;
            }
            if (inventoryList[i].data == null)
            {
                return i;
            }

           
        }

        return -1;
    }

    #endregion


    //how to tell if i have certain itens?


    public bool HasCertainItem(string itemName)
    {

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i] == null) continue;

            if (inventoryList[i].data.name == itemName)
            {
                return true;
            }
        }
        return false;

    }


    public void ConsumeCertainItem(string itemName)
    {

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i] == null) continue;

            if (inventoryList[i].data.name == itemName)
            {
                inventoryList[i].RemoveQuantity();
                handler.inventoryUI.UpdateItem(inventoryList[i], i);
                if (!inventoryList[i].CanExist())
                {
                    inventoryList.RemoveAt(i);
                    return;
                }

            }
            
        }



    }


    public void ReceiveItem(ItemClass item)
    {
        //we go to the first stack.
        //look if i can stack.
        int stackIndex = CanStack(item);
        if (stackIndex == -1)
        {
            //then we add the item
            AddItem(item);         
        }
        else
        {
            //CAN STACK
            SecondStep(item, stackIndex);
        }


    }


    void SecondStep(ItemClass item, int stackIndex)//we have a stack if we got here
    {
       
        int upperDifference = (inventoryList[stackIndex].quantity + item.quantity) - item.StackLimit();

        if(upperDifference > 0)
        {
            //if there is more than we can stack we will look if there are other places to stack 
            //or if we can create.
            item.quantity = upperDifference;
            inventoryList[stackIndex].AddToLimit();
            handler.inventoryUI.UpdateItem(inventoryList[stackIndex], stackIndex);
            ThirdStep(item);
        }
        else
        {
            //here we simply add it and we are done.
            inventoryList[stackIndex].AddQuantity(item.quantity);
            handler.inventoryUI.UpdateItem(inventoryList[stackIndex], stackIndex);
        }
        

    }

    void ThirdStep(ItemClass item)
    {
        int brake = 0;
        while(item.quantity > 0)
        {

            brake++;
            if(brake > 1000)
            {
                UnityEngine.Debug.LogError("broke third step inventory");
                return;
            }

            int canStack = CanStack(item);
            if (canStack != -1)
            {
                SecondStep(item, canStack);
                continue;
            }

            int freeSpace = GetFreeSpace();
            if (freeSpace != -1)
            {
                //if there is no more free space then we cant add this. if thats the case then we send the remaining back.
                //we need to add and then check if there is more
                item.RemoveQuantity(item.MaxValue());
                AddItem(item);

                continue;
            }

            UnityEngine.Debug.Log("there is quantity left");

        }

    }


    void AddItem(ItemClass item)
    {
        int freeSpace = GetFreeSpace();

        if(freeSpace == -1)
        {
            UnityEngine.Debug.LogError("something went wrong adding item: " + item.data.itemName);
            return;
        }

        if(item.quantity > item.StackLimit())
        {
            //then we might have to create another places for this fella.
            //then we remove all the crust and allow it foward.
            int newNumber = item.quantity - item.StackLimit();
            ItemClass newItem = new ItemClass(item.data, newNumber);
            

            item.quantity = item.StackLimit();
            inventoryList[freeSpace] = item;
            handler.inventoryUI.UpdateItem(item, freeSpace);
            ReceiveItem(newItem);
            
            return;
        }


        inventoryList[freeSpace] = item;
        handler.inventoryUI.UpdateItem(item, freeSpace);
        //handler.inventoryUI.

    }


    public void SwapItens(List<ItemClass> newList)
    {
        //

        inventoryList = newList;     



    }

    #region KEY
    List<int> keyList = new List<int>();

    public void AddKey(int key)
    {
        keyList.Add(key);
        if(key == 0)
        {
            handler.Warn("Got the Red Key", true);
        }
        if(key == 1)
        {
            handler.Warn("Got the Black Key", true);
        }
        handler.inventoryUI.HandleKeyUI(key);
        //there are only two keys. 0 - fire. 1 - black.
    }

    public bool HasKey(int keyId)
    {
        for (int i = 0; i < keyList.Count; i++)
        {
            if (keyList[i] == keyId) return true;
        }
        return false;
    }




    #endregion

    #region SAVE SYSTEM
    
    public object CaptureState()
    {

        //we dont save the thing id. we just read it from the restore state.
        //what we do is that we passe the list to the current thing.


        inventorySave.SaveInventory(inventoryList);
        UnityEngine.Debug.Log("capture");
        return new SaveData
        {
            souls = souls,
            keys = keyList,
        };
    }

    public void RestoreState(object state)
    {

        inventoryList = inventorySave.LoadInventory();
        handler.inventoryUI.UpdateInventory(inventoryList);

        var saveData = (SaveData)state;

        //we give hte itens from the inventory save to the actual inventory.

        keyList = saveData.keys;

        if (HasKey(0))
        {
            handler.inventoryUI.HandleKeyUI(0);
        }
        if (HasKey(1))
        {
            handler.inventoryUI.HandleKeyUI(1);
        }

        souls = saveData.souls;
        Observer.instance.OnUpdateSoul(souls);
    }

    [System.Serializable]
    struct SaveData
    {
        //we need a way to tell who it is.
        public int souls;
        public List<int> keys;
    }
    
    #endregion

}
public class KeyClass
{
    public string keyName;
    [TextArea]
    public string keyDescription;
    public int keyId;

}