using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCorpse : MonoBehaviour
{
    //when enemy become a corpse.
    //

    public List<FleshClass> fleshList = new List<FleshClass>();

    public void SetUp(List<FleshClass> fleshList)
    {

        //basic skeleton 

        GetComponent<BoxCollider2D>().isTrigger = true;
        StartCoroutine(LootCorpse());
    }

    float timeToDisperse = 30;

    IEnumerator LootCorpse()
    {
        yield return new WaitForSeconds(timeToDisperse);
        Destroy(gameObject);
    }
}
