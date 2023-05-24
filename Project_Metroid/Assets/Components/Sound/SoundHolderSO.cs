using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SoundHolderSO : ScriptableObject
{
    //we are just going to hold a bunch of stuff.
    public List<SoundHolderClass> soundHolderList = new List<SoundHolderClass>();


    public AudioClip GetClip(string id)
    {

        for (int i = 0; i < soundHolderList.Count; i++)
        {
            if (soundHolderList[i].id == id) return soundHolderList[i].clip;

        }

        Debug.Log("Couldnt find the id " + id);
        return null;
    }

    [System.Serializable]
   public class SoundHolderClass
    {
        public string id;
        public AudioClip clip;
    }

}

