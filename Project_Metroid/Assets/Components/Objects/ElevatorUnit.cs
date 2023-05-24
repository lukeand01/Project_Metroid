using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElevatorUnit : ButtonBase
{
    [SerializeField] GameObject notAllowed;
    [SerializeField] GameObject selected;
    [SerializeField] GameObject hovered;
    [SerializeField] GameObject currentChoice;
    [SerializeField] TextMeshProUGUI buttonText;
    int index;
    ElevatorUI handler;

    public void SetUp(int index, ElevatorUI handler)
    {
        this.index = index;
        this.handler = handler;
        
        buttonText.text = $"Floor {index}";
    }

    public void HandleAllow(bool choice)
    {
        if (choice)
        {
            notAllowed.SetActive(false);
        }
        else
        {
            notAllowed.SetActive(true);
        }
    }

    public void Current(bool choice)
    {
        currentChoice.SetActive(choice);
    }
    

    bool CanInteract()
    {
        if (notAllowed.activeInHierarchy) return false;
        if (currentChoice.activeInHierarchy) return false;

        return true;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //cannot click if moving.
        if (!CanInteract()) return;

        //if we click we send the order.
        handler.ReceiveOrder(index);

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanInteract()) return;
        hovered.SetActive(true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!CanInteract()) return;
        hovered.SetActive(false);
    }



}
