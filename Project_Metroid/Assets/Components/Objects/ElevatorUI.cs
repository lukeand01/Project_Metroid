using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorUI : MonoBehaviour
{
    //it shows which floors are accessible to travel.
    Elevator elevator;
    GameObject holder;
    [SerializeField] GameObject buttonHolder;
    public void SetUpUI(Elevator elevator)
    {
        this.elevator = elevator;
        holder = transform.GetChild(0).gameObject;

        for (int i = 0; i < buttonHolder.transform.childCount; i++)
        {
            buttonHolder.transform.GetChild(i).GetComponent<ElevatorUnit>().SetUp(i, this);
        }
    }

    bool isOpen;
    void Open()
    {
        isOpen = true;
        holder.SetActive(true);
        PlayerHandler.instance.EventPlayerInput += PlayerInput;
    }

    void PlayerInput(bool easyInput)
    {
        Close();
    }

    void Close()
    {
        Debug.Log("got called");
        elevator.interacting = false;
        isOpen = false;
        holder.SetActive(false);
        PlayerHandler.instance.EventPlayerInput -= PlayerInput;
    }


    public void ReceiveOrder(int index)
    {
        //cannot be the same order that they are currently situated.
        elevator.ReceiveOrder(index, true);
        holder.SetActive(false);
    }
    

    public void UpdateUI(List<int> allowedFloorList, int current)
    {
        //we allow buttons or not based on the allowed list.
        for (int i = 0; i < buttonHolder.transform.childCount; i++)
        {
            ElevatorUnit unit = buttonHolder.transform.GetChild(i).GetComponent<ElevatorUnit>();

            if(unit == null)
            {
                Debug.Log("didnt found an elevator unit");
                return;
            }

            if(i == current)
            {
                unit.Current(true);
            }
            else
            {
                unit.Current(false);
            }

            if (allowedFloorList.Contains(i))
            {
                unit.HandleAllow(true);
            }
            else
            {
                Debug.Log("we dont have that number");
                unit.HandleAllow(false);
            }
        }

        Open();

    }



    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(PlayerHandler.instance.GetKey("Interact")))
        {
            Close();
        }

    }
}
