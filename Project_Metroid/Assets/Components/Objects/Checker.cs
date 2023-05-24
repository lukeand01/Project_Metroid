using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checker : MonoBehaviour
{
    //this checks if the list of enemies are dead. if thats the case 
    //

    [SerializeField] List<EnemyBase> enemyList = new List<EnemyBase>();
    [SerializeField] UnityEvent unityEvent;
    bool done;


    private void Update()
    {
        if (done) return;

        if (AllDead())
        {
            unityEvent.Invoke();
            done = true;
        }

    }

    bool AllDead()
    {

        if(enemyList.Count <= 0)
        {
            Debug.LogError("There is nothing in the list");
            return false;
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (!enemyList[i].dead) return false;
        }

        Debug.Log("all are dead");
        return true;
    }



}
