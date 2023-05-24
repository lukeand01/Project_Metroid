using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveHandler : MonoBehaviour
{
    ///THIS IS THE THING THAT IS ALWAYS CALLED.
    //IT SHOULD HOLD MULTIPLE SAVES.

    //THE KEYCODES ARENT UNIQUE TO THE SAVE. THEY ARE PERSISTENT THROUGH OUT THE SAVES. ALL CONFIGURATIONS ARE.


    ///IN A NUTSHELL:
    ///WE GET A VAR CALLED STATE WHICH IS A DICTONARY THAT HOLDS TEH STATE AND THE GUIDS TO FIND TO WHO GIVE THOSE STATES.

    public static SaveHandler instance;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [ContextMenu("Delete all files")]
    public void DeleteFiles()
    {
        DeleteFile("first");
        DeleteFile("second");
        DeleteFile("third");
        
    }

    public void Save(string savePath)
    {

        var state = LoadFile(savePath);
        CaptureState(state);
        SaveFile(state, savePath);
    }

    public void Load(string savePath)
    {

        var state = LoadFile(savePath);
        RestoreState(state);
    }

    void SaveFile(object state, string savePath)///HERE WE SERIALIZE TEH DATA, TURNING INTO SOMETHING ONLY THE COMPUTER CAN READ AND CHANGE.
    {   

        using (var stream = File.Open(savePath, FileMode.Create))
        {
       
            var formatter = new BinaryFormatter();          
            formatter.Serialize(stream, state);
        }

    }

    public Dictionary<string, object> LoadFile(string savePath) ///HERE WE READ THE FILE AND TAKE AN OBJECT, WHICH WILL BE READ AS A TYPE OF SAVEDATA LATER.
    {

        if (!File.Exists(savePath))
        {
            return new Dictionary<string, object>();
        }
        using (FileStream stream = File.Open(savePath, FileMode.Open))
        {
            //the problem is here.
            var formatter = new BinaryFormatter();

           
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }

    }

    public bool FileExists(string savePath) ///WE CHECK IF THE FILE EXISTS IN THE SAVE FOLDER.
    {
        return File.Exists(savePath);
    }


    public void DeleteFile(string savePath) ///WE DELETE THE DATA. I KNOW. PRETTY CRAZY EXPLANATION.
    {
        if (FileExists(savePath))
        {
            File.Delete(savePath);
        }
    }

    private void CaptureState(Dictionary<string, object> state) 
    {
        foreach (var saveable in FindObjectsOfType<SaveableEntity>())///WE LOOK FOR OBJECTS IN THE SCENE OF TYPE SAVEABLE ENTITY.
        {

            state[saveable.Id] = saveable.CaptureState();///THEN WE ASK THE SAVEABLE ENTITY TO LOOK FOR ANY OTHER SCRIPT THAT HAS DATA TO SAVE.
            ///WE SAVE THAT DATA WITH A STRING THAT IS A GUID.
        }

    }

    void RestoreState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<SaveableEntity>())///WE ALSO LOOK FOR SAVEABLE ENTITY.
        {

            if (state.TryGetValue(saveable.Id, out object value))
           {
                saveable.RestoreState(value);
            }
            else
            {
                Debug.LogError("me-Save: save data failed to be restored: " + saveable.name);
            }
        }
    }

    
}
