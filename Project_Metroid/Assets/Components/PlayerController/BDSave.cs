using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Save/BD")]
public class BDSave : ScriptableObject
{
    //
    public List<ConsumableClass> tempList = new List<ConsumableClass>();
    public List<ConsumableClass> permaList = new List<ConsumableClass>();

    public void SaveBD(List<ConsumableClass> tempList, List<ConsumableClass> permaList)
    {
        this.tempList.Clear();
        for (int i = 0; i < tempList.Count; i++)
        {
            //i need to pass the current value 

            ConsumableClass newConsumable = new ConsumableClass(tempList[i].consumableType, tempList[i].value, tempList[i].duration, tempList[i].exception, tempList[i].GetProgress());
            this.tempList.Add(newConsumable);
        }

        this.permaList.Clear();
        for (int i = 0; i < permaList.Count; i++)
        {
            ConsumableClass newConsuamble = new ConsumableClass(permaList[i].consumableType, permaList[i].value, permaList[i].duration, permaList[i].exception, permaList[i].GetProgress());
            this.permaList.Add(newConsuamble);
        }

    }

    public List<ConsumableClass> LoadTempBD()
    {
        List<ConsumableClass> newList = new List<ConsumableClass>();
        for (int i = 0; i < tempList.Count; i++)
        {
            ConsumableClass newConsumable = new ConsumableClass(tempList[i].consumableType, tempList[i].value, tempList[i].duration, tempList[i].exception, tempList[i].GetProgress());
            newList.Add(newConsumable);
        }

        return newList;
    }
    public List<ConsumableClass> LoadPermaBD()
    {
        List<ConsumableClass> newList = new List<ConsumableClass>();
        for (int i = 0; i < permaList.Count; i++)
        {
            ConsumableClass newConsumable = new ConsumableClass(permaList[i].consumableType, permaList[i].value, permaList[i].duration, permaList[i].exception, permaList[i].GetProgress());
            newList.Add(newConsumable);
        }

        return newList;
    }

}
