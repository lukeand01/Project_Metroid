using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Lever : MonoBehaviour, IInteractable, ISaveable
{
    [SerializeField] UnityEvent unityEvent;
    [SerializeField] GameObject interactHolder;
    bool open;
    [SerializeField] GameObject leverPart;
    [SerializeField] float distanceVariable;
    [SerializeField] float speed = 2;



    [ContextMenu("PULL LEVER")]
    public void Interact()
    {

        if (!open)
        {
            open = true;
            interactHolder.SetActive(false);
            //StartCoroutine(OpenProcess());
            Vector3 vec = new Vector3(leverPart.transform.position.x + distanceVariable, leverPart.transform.position.y, 0);
            leverPart.transform.DOMove(vec, speed);
            unityEvent.Invoke();
        }

    }

    public void InteractUI(bool choice)
    {
        Debug.Log("interactui");
        if (!open)
        {
            interactHolder.SetActive(choice);
        }
        else
        {
            interactHolder.SetActive(false);
        }

        
    }


    #region SAVE SYSTEM
    public object CaptureState()
    {
        return new SaveData
        {
            open = open
        };
    }
    public void RestoreState(object state)
    {
        var savedata = (SaveData)state;

        open = savedata.open;

    }

    [System.Serializable]
    public struct SaveData
    {
        public bool open;

       
    }

    #endregion




}
