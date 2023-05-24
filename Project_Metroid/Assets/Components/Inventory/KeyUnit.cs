using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyUnit : ButtonBase
{
    //
    [TextArea]
    [SerializeField] string text;
    Image baseImage;


    private void Awake()
    {
        baseImage = GetComponent<Image>();
    }

    public void Control(bool has)
    {
       if(baseImage == null)
        {
            baseImage = GetComponent<Image>();
        }
        if (has)
        {
            baseImage.color = new Color(baseImage.color.r, baseImage.color.g, baseImage.color.b, 1);
        }
        else
        {
            baseImage.color = new Color(baseImage.color.r, baseImage.color.g, baseImage.color.b, 0.3f);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Observer.instance.OnShowDescription(transform.position, text, false);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        Observer.instance.OnShowDescription(transform.position, "", false);
    }

}
