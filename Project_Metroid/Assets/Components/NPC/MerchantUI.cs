using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class MerchantUI : MonoBehaviour
{
    InventoryUI handler;

    [Separator("TEMPLATE")]
    [SerializeField] GameObject template;
    [SerializeField] GameObject tradeTemplate;

    [Separator("STORAGE")]
    [SerializeField] GameObject storageHolder;
    [SerializeField] GameObject storageUnitHolder;
    
    [Separator("TRADE")]
    [SerializeField] GameObject tradeHolder;
    [SerializeField] GameObject tradeUnitPlayerHolder;
    [SerializeField] GameObject tradeUnitMerchantHolder;

    [Separator("COMMENTARIES")]
    [SerializeField] Image commentaryHolder;
    [SerializeField] TextMeshProUGUI commentaryText;

    [Separator("BUTTONS")]
    [SerializeField] GameObject tradeButton;
    [SerializeField] GameObject clearButton;

    public void Trade()
    {
        //doesnt go through if the merchant wont accept.
        Debug.Log("trade");
        //
        if (CanTrade())
        {
            //then we destroy           
            Clear(false);
        }
        else
        {
            Debug.Log("cannot trade");
        }

    }
    public void Cancel()
    {
        //close the ui.
        //clear the trade stuff.
        handler.CloseMerchant();
        Clear();
    } 

    public void Clear(bool clear = true)
    {
        //we clear both lists and ui.
        for (int i = 0; i < playerTradeList.Count; i++)
        {
            if (clear)
            {
                PlayerHandler.instance.ReceiveItem(playerTradeList[i].item);
            }
            


            
            Destroy(playerTradeList[i].gameObject);
        }

        //for each fella here we add it to the player.      

        playerTradeList.Clear();

        for (int i = 0; i < merchantTradeList.Count; i++)
        {
            if (!clear)
            {
                PlayerHandler.instance.ReceiveItem(merchantTradeList[i].item);
            }

            Destroy(merchantTradeList[i].gameObject);
        }

        merchantTradeList.Clear();

    }

    public void SetUpMerchant(List<ItemClass> itemList, InventoryUI handler)
    {
        this.handler = handler;

        //items never change so i just need to set it up once. i dont need to update it because it never changes.
        for (int i = 0; i < itemList.Count; i++)
        {
            CreateMerchantUnit(itemList[i]);
        }
    }

    void CreateMerchantUnit(ItemClass item)
    {
        GameObject newObject = Instantiate(template, storageUnitHolder.transform.position, Quaternion.identity);
        newObject.transform.parent = storageUnitHolder.transform;
        newObject.SetActive(true);
        newObject.GetComponent<MerchantUnit>().SetUp(item, this);

    }





    #region TRADE

    public List<TradeUnit> playerTradeList = new List<TradeUnit>();
    public List<TradeUnit> merchantTradeList = new List<TradeUnit>();

    public void AddItemToTrade(ItemClass item, bool isPlayer = false)
    {
        //we dont actually do anything to the item itself.
        //we just get a copy to the trade section.
        if (isPlayer)
        {

            ReceiveItem(playerTradeList, item);
        }
        else
        {

            ReceiveItem(merchantTradeList, item);
        }

        //when we add or remove.


        //then we update the trade ui.

        CalculateBalance();
    }

    public void RemoveItemToTrade(ItemClass item, bool isPlayer = false)
    {
        ItemClass newItem = new ItemClass(item.data);
        if (isPlayer)
        {
            for (int i = 0; i < playerTradeList.Count; i++)
            {
                if (playerTradeList[i].item.data == item.data)
                {
                    //then we remove one.
                    playerTradeList[i].item.RemoveQuantity();
                    playerTradeList[i].UpdateUI();
                    PlayerHandler.instance.ReceiveItem(newItem);

                    if (!playerTradeList[i].item.CanExist())
                    {
                        Debug.Log("player trade item cannot exit");
                        Destroy(playerTradeList[i].gameObject);
                        playerTradeList.RemoveAt(i);
                    }
                }

            }
        }
        else
        {
            for (int i = 0; i < merchantTradeList.Count; i++)
            {
                if (merchantTradeList[i].item.data == item.data)
                {
                    //then we remove one.
                    merchantTradeList[i].item.RemoveQuantity();
                    merchantTradeList[i].UpdateUI();
                    PlayerHandler.instance.ReceiveItem(newItem);

                    if (!merchantTradeList[i].item.CanExist())
                    {
                        Destroy(merchantTradeList[i].gameObject);
                        Debug.Log("merchant trade item cannot exit");
                        merchantTradeList.RemoveAt(i);
                    }
                }
            }

        }


        CalculateBalance();
    }

    int merchantValue = 0;
    int playerValue = 0;


    bool CanTrade()
    {


        if (playerValue >= merchantValue)
        {
            //you can trade.
            return true;

        }
        else
        {
            //you cannot trade.

            return false;
        }

    }

    void CalculateBalance()
    {
        //we get value.
        merchantValue = GetMerchantValue();
        playerValue = GetPlayerValue();


        if(playerValue == 0 || merchantValue == 0)
        {
            if(playerValue == 0 && merchantValue == 0)
            {
                clearButton.SetActive(false);
            }
            else
            {
                clearButton.SetActive(true);
            }

            //nothing happens when both are 0l
            return;
        }

        clearButton.SetActive(true);
        if (CanTrade())
        {
            //he narrates something based on how generous your offer is.

            MerchantReact(GenerateMerchantAdvice());
            tradeButton.SetActive(true);
        }
        else
        {
            //he narrates based on how close your offer is.

            MerchantReact(GenerateMerchantComplain());           
            tradeButton.SetActive(false);
        }


    }

    string GenerateMerchantComplain()
    {
        string text = "";
        int difference =  merchantValue - playerValue;


        if(difference == 0)
        {
            text = "it is... fair...";
        }

        if(difference == 1)
        {
            text = "almost a fair deal...";
        }

        if(difference == 2)
        {
            text = "be a bit more generous... and our friendship shall continue...";
        }

        if(difference == 3)
        {
            text = "generosity must be shared, friend. share it.";
        }

       if(difference > 3)
        {
            text = "it is... far from enough...";
        }


        return text;
    }

    string GenerateMerchantAdvice()
    {
        string text = "";
        int difference =   playerValue - merchantValue;



        if (difference == 0)
        {
            text = "it is... fair...";
        }

        if (difference == 1)
        {
            text = "you can get one more small thing...";
        }

        if (difference == 2)
        {
            text = "you do value your friend after all... i am much pleased";
        }

        if (difference == 3)
        {
            text = "generosity does fit you my friend";
        }

        if (difference > 3)
        {
            text = "generosity of this size... never seen before...";
        }


        return text;

    }



    int GetMerchantValue()
    {
        int value = 0;
        for (int i = 0; i < merchantTradeList.Count; i++)
        {
            ItemClass item = merchantTradeList[i].item;
            value += item.quantity * item.data.itemValue;

        }

        return value;
    }
    int GetPlayerValue()
    {
        int value = 0;
        for (int i = 0; i < playerTradeList.Count; i++)
        {
            ItemClass item = playerTradeList[i].item;
            value += item.quantity * item.data.itemValue;

        }

        return value;
    }

    #endregion

    #region INVENTORY HANDLIND
    //i need a way to find the trade unit.

    void ReceiveItem(List<TradeUnit> targetList, ItemClass item)
    {
        //nothing too fancy. if there are itens we stack, always stack. otherwise we create.
   
        int index = GetIndexList(targetList, item);
        if (index > -1)
        {
            targetList[index].item.AddQuantity();
            targetList[index].UpdateUI();
        }
        else
        {
            CreateTradeUnit(targetList, item);

        }

    }

    int GetIndexList(List<TradeUnit> targetList, ItemClass item)
    {
        for (int i = 0; i < targetList.Count; i++)
        {

            if (targetList[i].item.data == item.data)
            {
                return i;
            }
        }

        return -1;
    }

    void CreateTradeUnit(List<TradeUnit> targetList, ItemClass item)
    {
        ItemClass newItem = new ItemClass(item.data);


        if(merchantTradeList == targetList)
        {
            GameObject newObject = Instantiate(tradeTemplate, tradeUnitMerchantHolder.transform.position, Quaternion.identity);
            newObject.transform.parent = tradeUnitMerchantHolder.transform;
            newObject.SetActive(true);
            TradeUnit trade = newObject.GetComponent<TradeUnit>();
            trade.SetUp(newItem, this, false);
            merchantTradeList.Add(trade);

        }

        if(playerTradeList == targetList)
        {
            GameObject newObject = Instantiate(tradeTemplate, tradeUnitPlayerHolder.transform.position, Quaternion.identity);
            newObject.transform.parent = tradeUnitPlayerHolder.transform;
            newObject.SetActive(true);
            TradeUnit trade = newObject.GetComponent<TradeUnit>();
            trade.SetUp(newItem, this, true);
            playerTradeList.Add(trade);
        }

    }



    #endregion

    public void OpenMerchant()
    {
        //holder.SetActive(true); //open inventotry.
        PlayerHandler.instance.MouseVisible(true);

    }


    public void MerchantClick(ItemClass item)
    {
        //if you click on anything it will react.

        if (item.data.itemType == ItemData.ItemType.Flesh) return;

        Debug.Log("clicked on something not flesh");
        //that junk? 
        int random = Random.Range(0, 2);
        string text = "";

        if(random == 0)
        {
            text = "I want nothing to do with that junk!";
        }
        if(random == 1)
        {
            text = "is our friendship such a joke to you?";
        }
        if(random == 2)
        {
            text = "My hand would slap you";
        }


        if (text == "") Debug.LogError("Something went wrong with merchant click");

        MerchantReact(text);

    }

    public void MerchantHover(ItemClass item)
    {
        //it will just react about exsquisite item and flesh itens.

        if(item.data.itemType == ItemData.ItemType.Flesh)
        {

            MerchantReact(GetFleshString(item.data.itemValue));

        }


    }

    string GetFleshString(int fleshValue)
    {
        string text = "";

        if(fleshValue == 1)
        {
            int random = Random.Range(0, 2);

            if(random == 0)
            {
                text = "That is all i request";
            }
            if(random == 1)
            {
                text = "Its enough for an old creed like myself";
            }
            if(random == 2)
            {
                text = "would you like to trade it?";
            }

        }

        if(fleshValue == 2)
        {
            int random = Random.Range(0, 2);

            if (random == 0)
            {
                text = "Now... Thats interesting";
            }
            if (random == 1)
            {
                text = "I must have something you would like to trade for it.";
            }
            if (random == 2)
            {
                text = "Fresh... i want it...";
            }
        }

        if(fleshValue == 6)
        {
            int random = Random.Range(0, 2);
            if(random == 0)
            {
                text = "believe on what i say. If i could i would give my own body for it.";
            }

            if (random == 1)
            {
                text = "There is much to trade for such a beautiful piece.";
            }

            if(random == 2)
            {
                text = "Can i touch? of course not what im talking about. do point to anything you want";
            }
        }

        if(fleshValue == 10)
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                text = "that... i need it... you must trade with me...";
            }

            if (random == 1)
            {
                text = "I would kill for it- no, no, what im talking about?";
            }

            if (random == 2)
            {
                text = "all... all... that you want you shall have...";
            }
        }




        return text;
    }




    void MerchantReact(string commentary)
    {
        //he reacts to the itens when you click over them.
        //he reacts to only the good itens when you hover over them.

       
        StopAllCoroutines();
        commentaryText.text = commentary;
        commentaryHolder.gameObject.SetActive(true);
        commentaryHolder.color = new Color(commentaryHolder.color.r, commentaryHolder.color.g, commentaryHolder.color.b, 1);
        commentaryText.color = new Color(commentaryText.color.r, commentaryText.color.g, commentaryText.color.b, 1);

        StartCoroutine(MerchantCommentaryProcess());
    }

    IEnumerator MerchantCommentaryProcess()
    {
        yield return new WaitForSeconds(3);

        while(commentaryHolder.color.a > 0)
        {
            commentaryHolder.color -= new Color(0, 0, 0, 0.01f);
            commentaryText.color -= new Color(0, 0, 0, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }

        commentaryHolder.gameObject.SetActive(false);
    }
    


     
}
