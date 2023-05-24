using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SaveableEntity : MonoBehaviour
{
    ///THIS SCRIPTS IS PLACED IN AN OBJECT WHICH YOU WANT TO SAVE.

    [SerializeField] private string id;

    public string Id => id;

    [ContextMenu("GenerateID")]
    public void GenerateId() => id = Guid.NewGuid().ToString();

    public object CaptureState() ///
    {
        var state = new Dictionary<string, object>();
        foreach (var saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.CaptureState();
        }
        return state;
    }

    public void RestoreState(object state) 
    {

        var stateDictionary = (Dictionary<string, object>)state;
        foreach (var saveable in GetComponents<ISaveable>()) //WE CHECK EACH ISAVEABLE COMPONENT IN THE OBJECT.
        {
            string typeName = saveable.GetType().ToString();

            if (stateDictionary.TryGetValue(typeName, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }

    

}
