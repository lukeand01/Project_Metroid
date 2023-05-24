using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject interactHolder;
    public List<ItemClass> itemList = new List<ItemClass>();

    public bool alreadyMet;

    //merchant never runs out of things to sell. he has all things to sell at the very start.


    private void Start()
    {
        PlayerHandler.instance.inventoryUI.SetUpMerchant(itemList);
    }
    //you can sell nothing but the flesh parts.
    public void Interact()
    {
        //the first time around.
        if (alreadyMet)
        {
            PlayerHandler.instance.inventoryUI.OpenMerchant();
        }
        else
        {
            Observer.instance.OnStartDialogue(MerchantFirstDialogue());
            alreadyMet = true;
        }


    }

    public void InteractUI(bool choice)
    {
        interactHolder.SetActive(choice);
    }

    List<string> MerchantFirstDialogue()
    {
        List<string> newList = new List<string>();

        string first = "oh...$ a stranger approaches...$ ";
        newList.Add(first);

        


        return newList;
    }
}
