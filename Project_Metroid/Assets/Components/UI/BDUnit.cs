using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BDUnit : ButtonBase
{
    [SerializeField] Image portrait;
    [SerializeField] Image progressBar;
    public float duration;
    public float progress;
    public int index;
    bool hovering;

   public ConsumableClass consumable;
    PlayerConsumption handler;

    //we will do everyhthing here.

    //certain units are perma but i want to show in ui.


    public void SetUp(ConsumableClass consumable, PlayerConsumption handler, int index)
    {
        this.consumable = consumable;
        this.handler = handler;
        this.index = index;

        progress = consumable.GetProgress();

        if(consumable.value < 0)
        {
            //then its red.
            portrait.color = Color.red;
        }

        duration = consumable.duration;

        if(consumable.exception)
        {
            //its a perma condition taht shows here.
            progress = 0;
        }
        else
        {          
            StartCoroutine(ProgressProcess());
        }



        UpdateUI();
        UpdateProgress();
       
    }

    void UpdateUI()
    {
        
    }

    public void UpdateProgress()
    {
        progressBar.fillAmount = progress / duration;
    }


    IEnumerator ProgressProcess()
    {
        yield return new WaitForSeconds(1);
        while(progress < duration)
        {
            
            progress += 1;
            consumable.SetProgress(progress);
            UpdateProgress();
            yield return new WaitForSeconds(1);
           
        }

        Debug.Log("Done with this fella");
        handler.DeleteBD(index);

    }



    public override void OnPointerEnter(PointerEventData eventData)
    {
        //but only when you have the inventory?

        //show ui.

        if (!PlayerHandler.instance.inventoryUI.holder.activeInHierarchy) return;
        //Observer.instance.OnShowDescription(transform.localPosition)


    }

    

    public override void OnPointerExit(PointerEventData eventData)
    {
        
    }


    //the unit shouldnt handle it. because i want to show this ui in two difference places.
   
}
