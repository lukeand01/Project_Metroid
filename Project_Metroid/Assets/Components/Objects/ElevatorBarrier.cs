using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBarrier : MonoBehaviour, IInteractable
{

    [SerializeField] GameObject interactHolder;
    [SerializeField] int index;
    [SerializeField] Elevator elevator;

    BoxCollider2D bodyCollider;

    public bool unlocked;

    private void Start()
    {
        bodyCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    public void Interact()
    {
        if (unlocked)
        {
            CallElevator();
        }
        else
        {
            OpenBarrier();
        }
        
    }

    bool IsSameFloor()
    {
        return elevator.current == index;
    }
    void CallElevator()
    {
        if (IsSameFloor()) return;

        interactHolder.SetActive(false);
        elevator.ReceiveOrder(index);

    }

    //otherwise it works as a caller. it can only call if the elevator is not present in the same thign.

    void OpenBarrier()
    {
        if (!PlayerHandler.instance.HasCertainItem("ElevatorKey"))
        {
            Debug.Log("doesnt have the item");
            return;
        }

        PlayerHandler.instance.Warn("Lost Elevator Key");
        PlayerHandler.instance.ConsumeCertainItem("ElevatorKey");
        unlocked = true;
        elevator.ReceiveAllow(index);
        CallElevator();

        //we destroy the boxcollider
        Destroy(bodyCollider);
    }


    public void InteractUI(bool choice)
    {
        if (IsSameFloor())
        {
            interactHolder.SetActive(false);
        }
        else
        {
            interactHolder.SetActive(choice);
        }
        
    }
    //this cannot be walked through by the player.
    //it checks







}
