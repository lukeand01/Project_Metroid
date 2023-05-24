using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingSpot : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject interatHolder;
    public void Interact()
    {
        //tell the gamehandler to fade in and out.
        //save
        SaveHandler.instance.Save(SaveSlots.third.ToString()); //just one. its always first.
        PlayerHandler.instance.Warn("Game Saved", true);
        MusicHandler.instance.CreateGenericSfx("Save");
    }

    private void Update()
    {
        if(Vector3.Distance(PlayerHandler.instance.transform.position, transform.position) > 2.5f)
        {
            interatHolder.SetActive(false);
        }
    }

    public void InteractUI(bool choice)
    {
        interatHolder.SetActive(choice);
    }
}
