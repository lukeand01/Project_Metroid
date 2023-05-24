using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarnUnit : MonoBehaviour
{
    TextMeshProUGUI warnText;

    [SerializeField] Color redColor;
    [SerializeField] Color blueColor;


    public void SetUp(string warning, bool gainWarn)
    {

        

        warnText = GetComponent<TextMeshProUGUI>();

        if (gainWarn)
        {
            warnText.color = blueColor;
        }
        else
        {
            warnText.color = redColor;
        }

        warnText.text = warning;

        StartCoroutine(Process());
    }

    IEnumerator Process()
    {

        for (int i = 0; i < 100; i++)
        {
            
            transform.localPosition += new Vector3(0, 0.1f, 0);
            warnText.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.008f);
        }

        Destroy(gameObject);
    }
}
