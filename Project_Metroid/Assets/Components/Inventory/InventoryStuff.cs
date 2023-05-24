using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryStuff : MonoBehaviour
{
    //we leave it here so it will receive information.
    //it will show teh souls collected, sword state, keys and tools,


    [SerializeField] Image swordStateBar;
    [SerializeField] TextMeshProUGUI soulText;
    [SerializeField] GameObject knife;

    private void Start()
    {
        Observer.instance.EventUpdateSwordState += UpdateSword;
        Observer.instance.EventUpdateSoul += UpdateSoul;
        Observer.instance.EventGainedKnife += ShowKnife;
        Observer.instance.EventGainedKey += UpdateKey;
    }

    void UpdateSword(float value)
    {
        swordStateBar.fillAmount = value / 100;
    }

    void UpdateSoul(int value)
    {
        soulText.text = value.ToString();
    }

    void ShowKnife()
    {
        knife.SetActive(true);
    }

    void UpdateKey(int id)
    {

    }
}
