using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventorySave : ScriptableObject
{
    //this is the stuff that we are going to store the itens.

    public List<ItemClass> itemList = new List<ItemClass>();
    
    public void SaveInventory(List<ItemClass> newList)
    {

        itemList.Clear();
        for (int i = 0; i < newList.Count; i++)
        {
            if (newList[i] == null) continue;
            if (newList[i].data == null) continue;

                ItemClass newItem = new ItemClass(newList[i].data, newList[i].quantity);
                itemList.Add(newItem);
            
           

        }
      
    }

    public List<ItemClass> LoadInventory()
    {
        List<ItemClass> newList = new List<ItemClass>();

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == null) continue;
            if (itemList[i].data == null) continue;

            ItemClass newItem = new ItemClass(itemList[i].data, itemList[i].quantity);
            newList.Add(newItem);
        }
        return newList;
    }

}
