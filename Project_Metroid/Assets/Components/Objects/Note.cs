using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{

    [SerializeField] GameObject interactHolder;

    [SerializeField] string noteTitle;

    [TextArea]
    [SerializeField] List<string> noteContent;


    private void Update()
    {
        //if you interact while this is going on you close it.
        if (Input.GetKeyDown(PlayerHandler.instance.GetKey("Interact")))
        {
            CancelNote(false);
        }
    }

    public void Interact()
    {
        //open ui. block nothing, but if you do any action or take damage the ui vansihes.

        //but you cannot interact any further.
        PlayerHandler.instance.AddBlock("Note", PlayerHandler.BlockType.Interact);
        Observer.instance.OnShowNote(true, noteTitle, noteContent);

        PlayerHandler.instance.EventPlayerDamaged += CancelNote;
        PlayerHandler.instance.EventPlayerInput += CancelNote;
    }

    void CancelNote(bool empty)
    {

        PlayerHandler.instance.EventPlayerDamaged -= CancelNote;
        PlayerHandler.instance.EventPlayerInput -= CancelNote;

        PlayerHandler.instance.RemoveBlock("Note");
        Observer.instance.OnShowNote();
    }


    public void InteractUI(bool choice)
    {
        interactHolder.SetActive(choice);
    }
}
