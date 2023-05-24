using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRespawner : MonoBehaviour
{
    //respawns just one fella. cannot respawn another if it already has one.
    //
    [SerializeField] EnemyBase[] templateEnemy;
    EnemyBase currentEnemy;

    public void Spawn()
    {
        if (currentEnemy != null) return;

        //otherwise we spawn the fella.
        int random = Random.Range(0, templateEnemy.Length - 1);
        EnemyBase chosenTemplate = templateEnemy[random];

        GameObject newObject = Instantiate(chosenTemplate.gameObject, transform.position, Quaternion.identity);
        EnemyBase enemy = newObject.GetComponent<EnemyBase>();
        enemy.CantLose();
        currentEnemy = enemy;
        currentEnemy.EventEnemyDead += RemoveCurrentEnemy;
        
    }

    void RemoveCurrentEnemy(EnemyBase enemy)
    {
        if(currentEnemy == null)
        {
            Debug.LogError("Something wrong with single respawner for " + enemy.data.enemyName);
            return;
        }

        currentEnemy.EventEnemyDead -= RemoveCurrentEnemy;
        currentEnemy = null;
        
    }


}
