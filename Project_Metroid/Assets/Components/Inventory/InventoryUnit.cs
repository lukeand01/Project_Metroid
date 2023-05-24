using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryUnit : ButtonBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    [HideInInspector]public int index; //this is the inventory index. where it is in the list and the ui.

    [HideInInspector]public GameObject holder;
    ItemClass item;
    InventoryUI handler;
    [SerializeField] GameObject selected;
    public GameObject hover;
    [SerializeField] Sprite emptyPortrait;
    bool isPlayer;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] UnityEngine.UI.Image portrait;
    //[SerializeField] GameObject durabilityHolder;
    //[SerializeField] Image durabilityImage;


    public void Clear()
    {
        item = null;
        UpdateUI();
    }

    public void SetUp(ItemClass item, InventoryUI handler, int index, bool isPlayer)
    {
        holder = transform.GetChild(0).gameObject;

        this.item = item;
        this.handler = handler;

        if(index != -1) this.index = index;

        this.isPlayer = isPlayer;
      
        UpdateUI();

        handler.EventSelectedInventoryUnit += HandleSelection;
    }

    public void GiveItem(ItemClass item)
    {
        this.item = item;
    }

    public ItemClass GetItem()
    {
        return item;
    }

    void HandleSelection(int index)
    {
        if (this.index == index) return;

        selected.SetActive(false);
    }

   public void UpdateUI()
    {
        
       if(item != null)
        {
            if (item.quantity <= 0)
            {
                item = null;
            }
        }

        if (item == null)
        {

            if (!isPlayer)
            {
                handler.CheckChestEmpty();
                Destroy(gameObject);
            }

            //then the ui will be a bit different.
            portrait.sprite = emptyPortrait;
            quantityText.gameObject.SetActive(false);
        }
        else
        {

            quantityText.gameObject.SetActive(true);
            portrait.sprite = item.data.sprite;
            quantityText.text = item.quantity.ToString();

        }

       
    }

    //there are itens that are consumable.
    //others are equippable.
    //some are neither.
 
    //so basically:
        //you click here - it gives the item there.
        //this fakes informations and pretends as if it has been removed. but it hasnt.
        //three scenarios:
            //accept trade:
                //i will take directly from teh inventory. then update ui. no index info will be passed to the trade just what item.
                //
            //clear trade:
                //the item is returned, but not actually, so what we just need to do is call the units that passed information to update its regular ui.
            //you click in an especifc item to be returned:
                //for the merchant that is pretty easy, just deleted the trade version of it.
                //but for the player i need to return the fake information to the right one. but how to find the right one?



    public override void OnPointerClick(PointerEventData eventData)
    {
        //
       


        if(eventData.button == PointerEventData.InputButton.Right)
        {
            //this uses the item.
            //it give an order for the player handler.
            //there are more options as well.
            handler.OnClickItem(item);


            if (handler.tradeMerchant)
            {
                if (item.data.itemType != ItemData.ItemType.Flesh) return;

                item.RemoveQuantity();              
                handler.AddItemToTrade(item);
                UpdateUI();

                PlayerHandler.instance.SwapItens(handler.CreateNewList());
                return;
            }

            //maybe i can remove and give instead of doing fake stuff.



            if (isPlayer)
            {
                Player();
            }
            else
            {
                Chest();
            }
            
        }
    }

   



   
    void Player()
    {


        if (handler.tradeMerchant)
        {
            //we give this item to the trade.
            handler.AddItemToTrade(item);

        }
        else
        {
            //i equip or use the item.                                               
            if (!IsConsumable()) return ;


            PlayerHandler.instance.StartConsuming(item);

            //in here we give the order for the playerhander to start doing stuff.
            PlayerHandler.instance.EventActionCompleted += ConsumeDone;
            PlayerHandler.instance.EventActionCancelled += ConsumeCancelled;
        }

       
    }

    bool IsConsumable()
    {
        if (item.data.itemType == ItemData.ItemType.Consumable) return true;
        return false;
    }


    void ConsumeCancelled()
    {
        PlayerHandler.instance.EventActionCompleted -= ConsumeDone;
        PlayerHandler.instance.EventActionCancelled -= ConsumeCancelled;
    }

    void ConsumeDone()
    {
      
        PlayerHandler.instance.EventActionCompleted -= ConsumeDone;
        PlayerHandler.instance.EventActionCancelled -= ConsumeCancelled;
        PlayerHandler.instance.Consume(item);
        item.RemoveQuantity();
        UpdateUI();


    }


    void Chest()
    {
        if (!PlayerHandler.instance.CanAdd(item))
        {
            Debug.Log("no espace here");
            return;
        }


        //how to update this little bastard..
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //give all.
            PlayerHandler.instance.ReceiveItem(item);
            item = null;
        }
        else
        {
            //give one.
            ItemClass newItem = new ItemClass(item.data);
            PlayerHandler.instance.ReceiveItem(newItem);
            item.RemoveQuantity();
        }

        UpdateUI();
    }



    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!handler.drag.isDragging && item.data != null)
        {
            Debug.Log("got here");
            Observer.instance.OnShowDescription(transform.position, item.data.description, false);
        }
        
        handler.OnHoverItem(item);
        hover.SetActive(true);
        if(handler.drag.isDragging && isPlayer) handler.drag.SetTarget(this);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        Observer.instance.OnShowDescription(transform.position, "", false);
        hover.SetActive(false);
        if (handler.drag.isDragging && isPlayer) handler.drag.SetTarget(null);
        
    }


    #region DRAG
    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        if (!isPlayer) return;

        handler.drag.StartDrag(this);
        Observer.instance.OnShowDescription(transform.position, "", false);
        //when you start dragging you create a copy of this fellla and create 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null) return;
        if (!isPlayer) return;
    }


    #endregion
}
