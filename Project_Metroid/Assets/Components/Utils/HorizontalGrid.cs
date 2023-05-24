using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalGrid : LayoutGroup
{

    public float spacingX;
    public float spacingY;
    public bool followScale;
    [ConditionalField(nameof(followScale), false)] public Vector3 scaleSize = new Vector3(1,1,1);

    public void CorrectIt()
    {
        padding.left += 1;
        padding.left -= 1;
    }
    public override void CalculateLayoutInputVertical()
    {
        float posX = 0;
        float posY = 0;
        //i get the widght of the bastard.
        float widht = GetComponent<RectTransform>().sizeDelta.x;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var item = rectChildren[i];
            float actualSizeX = widht * item.localScale.x;


            if (i != 0)
            {
                posX += spacingX;
            }

            //but if i have enough in one line i should go to the next.

            //also i would like to force it into a certain size, decided from the editor.

            // item.sizeDelta = unitSize;
            if(followScale)item.localScale = scaleSize;



            if(posX + actualSizeX > widht)
            {
                //this means we go down.
                posY += item.sizeDelta.y + spacingY;
                posX = 0;
            }


            SetChildAlongAxis(item, 0, posX + padding.left - padding.right); //placing in the x axis.
            SetChildAlongAxis(item, 1, posY + padding.top - padding.bottom); //placing in the y axis.

            posX += actualSizeX;
            
        }
    }

    


    public override void SetLayoutHorizontal()
    {
       
    }

    public override void SetLayoutVertical()
    {
       
    }
}
