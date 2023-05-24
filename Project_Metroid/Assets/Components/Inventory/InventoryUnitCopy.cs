using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUnitCopy : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] TextMeshProUGUI quantity;

    public void SetUp(ItemClass item)
    {
        portrait.sprite = item.data.sprite;
        quantity.text = item.quantity.ToString();


    }


}
