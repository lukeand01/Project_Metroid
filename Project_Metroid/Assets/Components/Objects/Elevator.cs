using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{
    //it goes down and the player cannot leave 
    //if the player 


    [SerializeField] GameObject interactHolder;
    [SerializeField] ElevatorUI ui;
   public bool interacting;

    //we need a list of allowed floors.
    [SerializeField] List<ElevatorFloor> floorList;
    [SerializeField] List<int> allowedFloorList;
    public int current = 1;

    [SerializeField] GameObject firstDoor;
    [SerializeField] GameObject secondDoor;

    private void Start()
    {
        ui.SetUpUI(this);

        allowedFloorList.Add(1);

        SetUpFloor();
    }
    public void Interact()
    {
        //show a little ui that checks for input up or down.
        //if you move in any other way then you leave the place.
        if (interacting) return;
        StartElevator();        
    }

    void StartElevator()
    {
        PlayerHandler.instance.EventPlayerInput += CancelElevator;
        interacting = true;
        //open ui. cannot normal interact.
        ui.UpdateUI(allowedFloorList, current);


    }

    void CancelElevator(bool choice)
    {
        PlayerHandler.instance.EventPlayerInput -= CancelElevator;
        interacting = false;
        //close ui. return the interact.

    }

    public void ReceiveAllow(int index)
    {
        allowedFloorList.Add(index);

        //we send the elevator to teh floor the player is. the floor can traverse but he will fall.
        ReceiveOrder(index);
        Debug.Log("received allow");
    }

    void SetUpFloor()
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            floorList[i].Activate(true);

            if (i == current) floorList[i].Activate(false);
        }
    }

    public void ReceiveOrder(int order, bool playerInside = false)
    {
        //the order always has 1 more.

        ElevatorFloor target = floorList[order];

        if(target == null)
        {
            Debug.LogError("Somethign went wrong with receive order in elevator " + order);
            return;
        }

        StopAllCoroutines();
        floorList[current].Activate(true);
        
        if (current > order)
        {
            
            StartCoroutine(AscendProcess(target.gameObject, playerInside));
        }
        if(current < order)
        {
            //the current is higher.
           
            StartCoroutine(DescendProcess(target.gameObject, playerInside));
        }
        current = order;


    }

    IEnumerator AscendProcess(GameObject target, bool playerInside)
    {
        firstDoor.SetActive(true);
        secondDoor.SetActive(true);
        interacting = false;

        if (playerInside)
        {
            PlayerHandler.instance.gameObject.transform.parent = gameObject.transform;
        }

        while (target.transform.position.y > transform.position.y)
        {
            transform.position += new Vector3(0, 0.01f, 0);
            yield return new WaitForSeconds(0f);
        }

        if (playerInside)
        {
            PlayerHandler.instance.gameObject.transform.parent = null;
        }

        floorList[current].Activate(false);

        firstDoor.SetActive(false);
        secondDoor.SetActive(false);
        interacting = true;
    }

    IEnumerator DescendProcess(GameObject target, bool playerInside)
    {
        firstDoor.SetActive(true);
        secondDoor.SetActive(true);
        interacting = true;
        if (playerInside)
        {
            PlayerHandler.instance.gameObject.transform.parent = gameObject.transform;
        }


        while (target.transform.position.y < transform.position.y)
        {
            transform.position -= new Vector3(0, 0.01f, 0);
            yield return new WaitForSeconds(0f);
        }

        if (playerInside)
        {
            PlayerHandler.instance.gameObject.transform.parent = null;
        }
        interacting = false;
        firstDoor.SetActive(false);
        secondDoor.SetActive(false);
        floorList[current].Activate(false);
    }

   


    public void InteractUI(bool choice)
    {
        if (interacting)
        {
            interactHolder.SetActive(false);
            return;
        }
        interactHolder.SetActive(choice);
    }
}
