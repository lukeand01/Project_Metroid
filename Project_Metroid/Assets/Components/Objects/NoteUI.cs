using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteUI : MonoBehaviour
{
    GameObject holder;
    [SerializeField] TextMeshProUGUI noteTitleText;
    [SerializeField] TextMeshProUGUI noteContentText;

    private void Start()
    {
        holder = transform.GetChild(0).gameObject;
        Observer.instance.EventShowNote += ShowNote;
    }

    void ShowNote(bool order, string title, List<string> contentList)
    {
        if (order)
        {
            //then we open it

        }
        else
        {
            //then we close it.

        }



    }


}
