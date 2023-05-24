using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShrineIncreaseUnit : MonoBehaviour
{
    //shows the player what he gained and what level it is.
    ShrineUI handler;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image gainedSprite;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject confirmButton;

    [SerializeField] List<Sprite> spriteList;


    public void SetUp(int currentLevel, ShrineUI handler)
    {
        levelText.text = currentLevel.ToString();
        this.handler = handler;
        //show what you gained.
        HandleGain(currentLevel);

        messageText.text = CreateMessage(currentLevel);

        StartCoroutine(DisplayProcess());
    }

    void HandleGain(int currentLevel)
    {
        gainedSprite.sprite = spriteList[currentLevel - 1];

        if(currentLevel == 1)
        {
            nameText.text = "Hunting Knife";
        }

    }

    IEnumerator DisplayProcess()
    {
        transform.localScale = new Vector3(0.1f, 0.1f);
        confirmButton.SetActive(false);
        while (transform.localScale.x < 1)
        {
            transform.localScale += new Vector3(0.05f, 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
        confirmButton.SetActive(true);

    }


    string CreateMessage(int currentLevel)
    {
        if(currentLevel == 1)
        {
            return "Take this so your enemies may be harvested just as they are reaped";
        }


        return "";
    }

    public void Accept()
    {
        handler.AcceptNewLevel();
        gameObject.SetActive(false);
    }

}
