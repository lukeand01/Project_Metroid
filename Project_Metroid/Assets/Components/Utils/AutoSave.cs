using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSave : MonoBehaviour, ISaveable
{

    bool onlyOnce;

    public object CaptureState()
    {
        return new SaveData
        {
            onlyOnce = onlyOnce
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        onlyOnce = saveData.onlyOnce;

    }
    [Serializable]
    public struct SaveData
    {
        public bool onlyOnce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onlyOnce) return;

        onlyOnce = true;
        SaveHandler.instance.Save(SaveSlots.third.ToString());
    }
}
