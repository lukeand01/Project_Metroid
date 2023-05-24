using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBarrier : MonoBehaviour
{
    //these are doors that are moved up or down to allow passage.

    bool open;
    [SerializeField] float distanceVariable;
    [SerializeField] float speedVariable;

    [ContextMenu("OPEN DOOR")]
    public void OpenDoor()
    {
        if (!open)
        {
            
            open = true;
            Vector3 vec = new Vector3(transform.position.x, transform.position.y + distanceVariable, 0);
            gameObject.transform.DOMove(vec, speedVariable);
        }
    }

    

}
