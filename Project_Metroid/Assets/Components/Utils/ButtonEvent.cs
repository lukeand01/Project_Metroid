using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEvent : ButtonBase
{
    [SerializeField] GameObject hovered;
    [SerializeField] GameObject selected;
    [SerializeField] UnityEvent unityEvent;

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("clicked on it");
        //StartCoroutine(SelectedProcess());
        MusicHandler.instance.CreateGenericSfx("Button");
        unityEvent.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        hovered.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        hovered.SetActive(false);
    }

    IEnumerator SelectedProcess()
    {
        selected.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        selected.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        Color color = hovered.GetComponent<Image>().color;
        while(color.a > 0)
        {
            color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
        hovered.SetActive(false);
    }

    IEnumerator FadeIn()
    {
        hovered.SetActive(true);
        Color color = hovered.GetComponent<Image>().color;
        while (color.a < 0.7f)
        {
            color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
