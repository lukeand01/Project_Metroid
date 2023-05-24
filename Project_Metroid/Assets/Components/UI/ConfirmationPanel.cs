using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmationPanel : MonoBehaviour
{
    //
    [SerializeField] TextMeshProUGUI confirmationText;
    MenuHandler handler;

    //

    public void SetUp(MenuHandler handler)
    {
        this.handler = handler;
    }
   public void CreateConfirm(string confirmationString)
    {
        confirmationText.text = confirmationString;

       if(PlayerHandler.instance != null)PlayerHandler.instance.AddBlock("Confirmation", PlayerHandler.BlockType.Complete);
        
    }

    public void Confirm()
    {
        handler.OnConfirm();
        if (PlayerHandler.instance != null) PlayerHandler.instance.RemoveBlock("Confirmation");
    }
    public void Cancel()
    {
        handler.OnCancel();
        if (PlayerHandler.instance != null) PlayerHandler.instance.RemoveBlock("Confirmation");
    }


}
