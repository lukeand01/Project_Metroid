using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shrine : MonoBehaviour, IInteractable, ISaveable
{
    //you get souls to give to the shrine.
    //each soul given progresses to a big gft.
    //each soul give awards some kind of bless.

    [SerializeField] GameObject interactHolder;
    [SerializeField] ShrineUI ui;

    [SerializeField] int minRequired; //min required from one give to trigger a buff giving.

    int storedSouls; //thi
    int currentLevel;

    List<ConsumableClass.BDType> typeList;
    private void Start()
    {
        typeList = GetTypeList();
    }

 
    public void Interact()
    {
        int gainedSouls = 0;
        ShrineBuffClass buffClass = null;
        if (currentLevel >= 5)
        {
            //it will say nothing.
            ui.ReceiveOrder(gainedSouls, storedSouls, currentLevel, buffClass);
            return;
        }

        gainedSouls = PlayerHandler.instance.GetSoul();
        buffClass = GetBuffClass(gainedSouls);
        ui.ReceiveOrder(gainedSouls, storedSouls, currentLevel, buffClass);

        storedSouls += gainedSouls;
        //we will do teh process here.
        if (storedSouls > GetMaxSoul())
        {
            storedSouls = 0;
            currentLevel += 1;
            AwardPrize();
        }
        
        PlayerHandler.instance.ClearSoul();
        Observer.instance.OnUpdateSoul(0);
    }

    //
    //


    void AwardPrize()
    {
        //this is the big prize.
        //the first one is knife.
        //second is 
        if(currentLevel == 1)
        {
            PlayerHandler.instance.GainKnife();
        }
        if(currentLevel == 2)
        {

        }

    }

    List<ConsumableClass.BDType> GetTypeList()
    {
        List<ConsumableClass.BDType> newList = new List<ConsumableClass.BDType>();

        newList.Add(ConsumableClass.BDType.PermaMaxHealth);
        newList.Add(ConsumableClass.BDType.PermaDamage);
        newList.Add(ConsumableClass.BDType.PermaSpeed);
        newList.Add(ConsumableClass.BDType.PermaStamina);
        newList.Add(ConsumableClass.BDType.UseSpeed);
        newList.Add(ConsumableClass.BDType.AbilityCooldown);
        newList.Add(ConsumableClass.BDType.Luck);

        return newList;
    }


    ShrineBuffClass GetBuffClass(float souls)
    {
        //we create all the random fellas.
        //these are always random and dont change over the course of the game.
        //we will generate random consumable class.

        //based on level. cannot have the same buff.
        ShrineBuffClass newClass = new ShrineBuffClass(null, null); 

        if(souls >= 10)
        {
            List<ConsumableClass> newList = new List<ConsumableClass>();

            while(newList.Count < 3)
            {
                //we create options till we have 3 options, but it can never be the same one.
                bool cannot = false;
                int random = Random.Range(0, typeList.Count - 1);
                ConsumableClass.BDType chosenType = typeList[random];

                for (int i = 0; i < newList.Count; i++)
                {
                    if (newList[i].consumableType == chosenType)
                    {
                        cannot = true;
                    }
                }

                if (!cannot)
                {
                    ConsumableClass newConsumable = new ConsumableClass(chosenType, GetValue(chosenType));
                    newList.Add(newConsumable);
                }
                    
             
                
            }

            newClass.firstList = newList;
        }

        if(souls >= 20)
        {
            List<ConsumableClass> newList = new List<ConsumableClass>();

            while (newList.Count < 3)
            {
                //we create options till we have 3 options, but it can never be the same one.
                bool cannot = false;
                int random = Random.Range(0, typeList.Count - 1);
                ConsumableClass.BDType chosenType = typeList[random];

                for (int i = 0; i < newList.Count; i++)
                {
                    if (newList[i].consumableType == chosenType)
                    {
                        cannot = true;
                    }
                }

                if (!cannot)
                {
                    ConsumableClass newConsumable = new ConsumableClass(chosenType, GetValue(chosenType));
                    newList.Add(newConsumable);
                }



            }

            newClass.secondList = newList;
        }


        return newClass;
    }

    int GetValue(ConsumableClass.BDType type)
    {
        return 5;
    }

    public void InteractUI(bool choice)
    {
        interactHolder.SetActive(choice);
    }


    int GetMaxSoul()
    {
        return 10 + (5 * currentLevel);
    }

    #region SAVE SYSTEM
    
    public object CaptureState()
    {

        return new SaveData
        {
            storedSouls = storedSouls,
            currentLevel = currentLevel,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        storedSouls = saveData.storedSouls;
        currentLevel = saveData.currentLevel;


    }

    [System.Serializable]
    struct SaveData
    {
        public int storedSouls;
        public int currentLevel;
    }
    
    #endregion
}

public class ShrineBuffClass
{
    public ShrineBuffClass(List<ConsumableClass> firstList, List<ConsumableClass> secondList)
    {
        this.firstList = firstList;
        this.secondList = secondList;
    }

    public List<ConsumableClass> firstList = new List<ConsumableClass>();
    public List<ConsumableClass> secondList = new List<ConsumableClass>();
}
