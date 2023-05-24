using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Chest : MonoBehaviour, IInteractable, ISaveable
{

    public List<ItemClass> inventoryList = new List<ItemClass>();
    [SerializeField] InventorySave inventorySave;

    public int progress = 30;
    int currentProgress;
    public bool open;

    [SerializeField] GameObject interactHolder;
    [SerializeField] TextMeshProUGUI interactText;

  

    public void Interact()
    {
        //open inventory.
        if (open)
        {
            OpenChest();
        }
        else
        {
            StartOpenChest();
        }   
    }
    
    //
    public void InteractUI(bool choice)
    {
        interactHolder.SetActive(choice);
        interactText.text = PlayerHandler.instance.GetKey("Interact").ToString();
    }


    #region INVENTORY
    void OpenChest()
    {
        PlayerHandler.instance.EventPlayerInput += CloseChest;
        PlayerHandler.instance.EventPlayerDamaged += CloseChest;

        PlayerHandler.instance.MouseVisible(true);

        Observer.instance.OnOpenChest(inventoryList, this);
    }
    void CloseChest(bool empty)
    {
        PlayerHandler.instance.EventPlayerInput -= CloseChest;
        PlayerHandler.instance.EventPlayerDamaged -= CloseChest;

        PlayerHandler.instance.MouseVisible(false);
        //we bring the ui information back.
        Observer.instance.OnOpenChest(null, this); ;
    }



    #endregion
    #region OPENING
    //what if i want the chest to have traps?
    void StartOpenChest()
    {
        //when you start opening the chest you begin a process and hear for input or damage.
        PlayerHandler.instance.EventPlayerInput += Cancel;
        PlayerHandler.instance.EventPlayerDamaged += Cancel;
        PlayerHandler.instance.EventActionCompleted += Complete;

        PlayerHandler.instance.ActionStart(currentProgress, progress);
    }

    void Complete()
    {
        PlayerHandler.instance.EventPlayerInput -= Cancel;
        PlayerHandler.instance.EventPlayerDamaged -= Cancel;
        PlayerHandler.instance.EventActionCompleted -= Complete;
        open = true;
        OpenChest();
    }


    void Cancel(bool empty)
    {
        //when we cancel we want the information about the progress and then tell the player to stop progressing.
        PlayerHandler.instance.EventPlayerInput -= Cancel;
        PlayerHandler.instance.EventPlayerDamaged -= Cancel;
        PlayerHandler.instance.EventActionCompleted -= Complete;
 
        currentProgress = PlayerHandler.instance.ActionProgress();
        PlayerHandler.instance.ActionCancel();
    }
    #endregion

    public void ReceiveInventory(List<ItemClass> itemList)
    {
        inventoryList = itemList;
    }

    #region SAVE SYSTEM

    
    public object CaptureState()
    {
        Debug.Log("captured state from chest " + inventoryList[0].quantity);
        if(inventorySave != null)
        {
            inventorySave.SaveInventory(inventoryList);
        }
        else
        {
            Debug.LogError("NO inventory save in " + gameObject.name);
        }

      

        return new SaveData
        {
            currentProgress = currentProgress,
            open = open

        };
    }

    public void RestoreState(object state)
    {       
        inventoryList = inventorySave.LoadInventory();

        var saveData = (SaveData)state;

        open = saveData.open;
        currentProgress = saveData.currentProgress;

    }

    [System.Serializable]
    struct SaveData
    {
        public int currentProgress;
        public bool open;
    }
    
    #endregion
}
