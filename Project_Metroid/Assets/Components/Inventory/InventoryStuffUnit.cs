using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryStuffUnit : ButtonBase
{
    [TextArea][SerializeField] string description;
    [SerializeField] Vector3 offSet;
    public void SetUpSkill()
    {
        //
    }




    public override void OnPointerEnter(PointerEventData eventData)
    {
        Observer.instance.OnShowDescription(transform.position + offSet, description, false);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Observer.instance.OnShowDescription(transform.position + offSet, "", false);
    }
}
