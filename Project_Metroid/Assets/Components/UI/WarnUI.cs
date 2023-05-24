using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarnUI : MonoBehaviour
{
    //
    [SerializeField] GameObject template;
    public void CreateWarning(string warning, bool gainWarn)
    {
        GameObject newObject = Instantiate(template, gameObject.transform.position, Quaternion.identity);
        newObject.transform.parent = gameObject.transform;
        newObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);

        if (gainWarn)
        {
            //put it a bit above.
            newObject.transform.position += new Vector3(0, 2, 0);
        }

        newObject.SetActive(true);
        newObject.GetComponent<WarnUnit>().SetUp(warning, gainWarn);
    }



}
