using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    GameObject holder;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI timerText;

    private void Start()
    {
        holder = gameObject.transform.GetChild(0).gameObject;
        Observer.instance.EventShowDescription += SetUp;
    }

    public void Close()
    {
        holder.SetActive(false);
    }
    public void SetUp(Vector3 position, string description, bool HasTimer = true)
    {
        if(description == "")
        {
            holder.SetActive(false);
            return;
        }

        Debug.Log("Show description");
        holder.SetActive(true);

        gameObject.transform.position = position;

       
       

        descriptionText.text = description;
        timerText.gameObject.SetActive(HasTimer);
    }
    public void SetUpTimer(int currentTimer, int maxTimer)
    {
        timerText.text = $"Current: {currentTimer}. Limit: {maxTimer}";
    }



}
