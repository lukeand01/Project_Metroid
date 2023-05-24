
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDrag : MonoBehaviour
{
    //this will help the ui talk to each other.
    //the secondinventory will use its ui to tell.



    //we start to drag a fella.
    [SerializeField] InventoryUnitCopy copyInventoryUnit;

    InventoryUnit draggingItem;
    public InventoryUnit targetItem;

    InventoryUI handler;

    public bool isDragging;
    private void Start()
    {
        handler = GetComponent<InventoryUI>();
    }

    private void Update()
    {
        
        //now we look at stacking.

        if (!isDragging) return;

       copyInventoryUnit.transform.position = Input.mousePosition;     

        //you cant stack just one.
        //

        if (Input.GetMouseButtonUp(0))
        {

            if (CanStack())
            {
                Stack(draggingItem.GetItem().quantity);
                return;
            }


            if (CanSwap())
            {
                Swap();
            }
            else
            {

                EndDrag();
            }
        }
    }

    

    public void StartDrag(InventoryUnit unit)
    {
        isDragging = true;
        draggingItem = unit;
        copyInventoryUnit.SetUp(unit.GetItem());
        draggingItem.holder.SetActive(false);
        copyInventoryUnit.gameObject.SetActive(true);
    }

    public void EndDrag()
    {
        if (draggingItem == null) return;
        isDragging = false;
        draggingItem.holder.SetActive(true);
        draggingItem.hover.SetActive(false);
        draggingItem = null;
        copyInventoryUnit.gameObject.SetActive(false);


    }

    bool CanSwap()
    {
       
        
        if (targetItem == null)
        {
            Debug.Log("no target item");
            return false;
        }
        

        if (targetItem.index == draggingItem.index)
        {
            Debug.Log("same index. same item");
            return false;
        }

        return true;
    }

    bool CanStack()
    {

        if (targetItem == null)
        {
            Debug.Log("no target item");
            return false;
        }

        if(targetItem.GetItem() == null)
        {
            return false;
        }

        //if its the same.
        if (targetItem.GetItem().data == draggingItem.GetItem().data)
        {
            return true;
        }
        
        if(targetItem.GetItem().StackUpperDifference() <= 0)
        {
            Debug.Log("its the same but cannot stack as there is no space");
            return false;
        }

        return false;

    }
    bool CanGive()
    {
        if (targetItem == null)
        {
            Debug.Log("no target item");
            return false;
        }

        if (targetItem.GetItem() == null)
        {
            Debug.Log("the place is empty");
            return true;
        }

        return false;
    }

    void Swap()
    {
        

        ItemClass draggingItemClass = draggingItem.GetItem();
        ItemClass targetItemClass = targetItem.GetItem();

        draggingItem.SetUp(targetItemClass, handler, -1, true);
        targetItem.SetUp(draggingItemClass, handler, -1, true);


        PlayerHandler.instance.SwapItens(handler.CreateNewList());

        EndDrag();
    }



    void Give(int quantity)
    {
        //this is when we give to an empty slot.

        ItemClass newItem = new ItemClass(draggingItem.GetItem().data, quantity);

        targetItem.GiveItem(newItem);
        targetItem.UpdateUI();

        draggingItem.GetItem().RemoveQuantity(quantity);
        draggingItem.UpdateUI();

        copyInventoryUnit.SetUp(draggingItem.GetItem());

        if (!draggingItem.GetItem().CanExist())
        {
            EndDrag();
        }
    }
    void Stack(int quantity)
    {
        //we remove a fella if there is not enough.
        int stackableQuantity = targetItem.GetItem().StackUpperDifference();

        Debug.Log("this is the stackble quantity " + stackableQuantity);

        if(quantity >= stackableQuantity)
        {
            //we add the to the max.
            targetItem.GetItem().AddToLimit();
            targetItem.UpdateUI();

            draggingItem.GetItem().RemoveQuantity(stackableQuantity);
            draggingItem.UpdateUI();
            

        }
        else
        {
            //we just add the quantity.
            targetItem.GetItem().AddQuantity(quantity);
            targetItem.UpdateUI();

            draggingItem.GetItem().RemoveQuantity(quantity);
            draggingItem.UpdateUI();
        }

        copyInventoryUnit.SetUp(draggingItem.GetItem());
        EndDrag();

       

    }


    public void SetTarget(InventoryUnit unit)
    {

        targetItem = unit;
    }


    
}
