using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadReminder : MonoBehaviour
{
    //just use this because the damn thing is not loading thourhg gamehandler.

    private void Start()
    {
        if (SaveHandler.instance.FileExists(SaveSlots.third.ToString()))
        {
            Debug.Log("load the damn thing");
            SaveHandler.instance.Load(SaveSlots.third.ToString());
        }
    }



}
