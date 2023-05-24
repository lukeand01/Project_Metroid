using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BridgeHolder : MonoBehaviour, ISaveable
{
    [SerializeField] GameObject roseBridgeObject;
    [SerializeField] GameObject lowBridgeObject;
    [SerializeField] GameObject[] colliderObjects;



    bool lowered;


    [ContextMenu("LOWER BRIDGE")]
    public void LowerBridge()
    {
        roseBridgeObject.SetActive(false);
        lowBridgeObject.SetActive(true);

        for (int i = 0; i < colliderObjects.Length; i++)
        {
            colliderObjects[i].SetActive(false);
        }


    }



    [ContextMenu("ROSE BRIDGE")]
    public void RoseBridge()
    {
        roseBridgeObject.SetActive(true);
        lowBridgeObject.SetActive(false);

        for (int i = 0; i < colliderObjects.Length; i++)
        {
            colliderObjects[i].SetActive(true);
        }
    }


    public object CaptureState()
    {
        return new SaveData
        {
            lowered = lowered
        };


    }
    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        lowered = saveData.lowered;

        if (lowered)
        {
            LowerBridge();
        }
        else
        {
            RoseBridge();
        }

    }
    [Serializable]
    public struct SaveData
    {
        public bool lowered;
    }
}
