using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConsumableClass;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class PlayerConsumption : MonoBehaviour, ISaveable
{

    //put debuffs here.
    PlayerHandler handler;

    [SerializeField] BDSave bdSave;

    public List<ConsumableClass> permaConsumableList = new List<ConsumableClass>();
    public List<BDUnit> bdList = new List<BDUnit>();

    //i will have a bunch of sprites here for the temp stats.

    private void Start()
    {
        handler = GetComponent<PlayerHandler>();
    }

    public void HandleConsumption(ConsumableClass consumable)
    {
        if (consumable.cure)
        {
            HandleCure(consumable);

        }


       if(consumable.consumableType == BDType.Health)
        {

            //this just recoveres health. 
            handler.RecoverHealth(consumable.value);
        }

       if (consumable.consumableType == BDType.PermaMaxHealth)
        {
            if (consumable.exception)
            {
                Debug.Log("this is an excpetion");
                AddTempItem(consumable);
            }
            else
            {
                Debug.Log("this is not an expcetion");
                permaConsumableList.Add(consumable);
                
            }

            float total = GetPermaValue(BDType.PermaMaxHealth) + GetTempValue(BDType.PermaMaxHealth);
            handler.UpdateHealthBonus(total);
        }

       if (consumable.consumableType == BDType.PermaStamina)
        {
            Debug.Log("perma stamina");
            permaConsumableList.Add(consumable);

            float total = GetTempValue(BDType.TempStamina) + GetPermaValue(BDType.PermaStamina);

            handler.UpdateStaminaBonus(total);

        }
       if(consumable.consumableType == BDType.TempStamina)
        {


            AddTempItem(consumable);

            float total = GetTempValue(BDType.TempStamina) + GetPermaValue(BDType.PermaStamina);

            handler.UpdateStaminaBonus(total);
        }


       if(consumable.consumableType == BDType.PolishWeapon)
        {
            //polish the weapon once done.
            PlayerHandler.instance.RepairSword();
        }

    }

    void HandleCure(ConsumableClass consumable)
    {
        if (consumable.cureAll)
        {
            //then we cure any and all negative 
        }
        else
        {
            //then we cure the diseases affecting the type of the consumable.
        }
    }


    public void AddTempItem(ConsumableClass consumable)
    {
        if (!CanConsumeTemp(consumable.consumableType))
        {
            Debug.LogError("has too many buffs of the type");
            return;
        }

        //create unit.

        handler.hud.AddBD(consumable, this, bdList.Count);       
    }

    public void DeleteBD(int index)
    {
        for (int i = 0; i < bdList.Count; i++)
        {            
            Destroy(bdList[i].gameObject);
            bdList.RemoveAt(i);
        }       
    }

    float GetTempValue(BDType consumableType)
    {
        float value = 0;
        for (int i = 0; i < bdList.Count; i++)
        {
            if (bdList[i].consumable.consumableType == consumableType)
            {
                bdList[i].consumable.value += value;
            }

        }

        return value;

    }

    float GetPermaValue(BDType consumableType)
    {
        float value = 0;
        for (int i = 0; i < permaConsumableList.Count; i++)
        {
            if (permaConsumableList[i].consumableType == consumableType)
            {
                value += permaConsumableList[i].value;
               
            }

        }

        return value;
    }

    public bool CanConsumeTemp(ConsumableClass.BDType consumableType)
    {
        int count = 0;

        for (int i = 0; i < bdList.Count; i++)
        {
            if (bdList[i].consumable.consumableType == consumableType)
            {
                count += 1;
            }
        }
        return count < 3;

    }


    #region SAVE SYSTEM
    
    List<ConsumableClass> CreateListFromTemp()
    {
        List<ConsumableClass> consumableList = new List<ConsumableClass>();
        for (int i = 0; i < bdList.Count; i++)
        {
            //ConsumableClass newConsumable = new ConsumableClass();
            consumableList.Add(bdList[i].consumable);
            //need to find where to store the current progress.
        }

        return consumableList;
    }

    //you reset enemies: if you die, if you load it back

    
    public object CaptureState()
    {

        bdSave.SaveBD(CreateListFromTemp(), permaConsumableList);


        return new SaveData
        {
            empty = ""
        };

    }

    public void RestoreState(object state)
    {

        //we maybe have to clear the temp list.
        for (int i = 0; i < bdList.Count; i++)
        {
            Destroy(bdList[i]);
        }

        bdList.Clear();

        List<ConsumableClass> newList = bdSave.LoadTempBD();

        //then we add it normally.
        for (int i = 0; i < newList.Count; i++)
        {
            AddTempItem(newList[i]);
        }


        //hte other one goes directly to the list.
        permaConsumableList = bdSave.LoadPermaBD();

    }

    //we save here the debuffs and buffs we have.
    [System.Serializable]
    struct SaveData
    {
        public string empty;
    }
    
    #endregion
}

[System.Serializable]
public class ConsumableClass
{
    //here we will have types of resources and their values.
    public BDType consumableType;
    public float value;
    public bool cure; //cure the consumabletype;
    [ConditionalField(nameof(cure), false)] public bool cureAll;
    [ConditionalField(nameof(consumableType), false, BDType.TempDamage, BDType.TempSpeed, BDType.TempStamina)] public float duration;
    float progress; //how much it has progressed towards 
    [ConditionalField(nameof(consumableType), false, BDType.PermaDamage, BDType.PermaMaxHealth, BDType.PermaSpeed, BDType.PermaStamina)] public bool exception;
    [HideInInspector] float currentduration;

    public ConsumableClass(BDType consumableType, float value, float duration = 0, bool exception = false, float progress = 0)
    {
        this.consumableType = consumableType;
        this.value = value;
        this.duration = duration;
        this.exception = exception;
        this.progress = progress;
    }

    public void SetProgress(float value) => progress = value;

    public float GetProgress() => progress;

    public enum BDType
    {
        Health,
        PermaMaxHealth,
        TempDamage,
        PermaDamage,
        TempSpeed,
        PermaSpeed,
        TempStamina,
        PermaStamina,
        StaminaCost,
        UseSpeed,
        AbilityCooldown,
        Luck,
        PolishWeapon
    }

 

}