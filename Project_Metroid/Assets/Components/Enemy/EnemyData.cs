using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemySO")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    //stats here

    public float health;
   
    public float spotRange;

    public float chaseSpeed;
    public float walkSpeed;

    public float attackRange;
    public float attackSpeed;

    public float knockback;
    public float knockbackResistance;

    public List<AttackClass> attackList = new List<AttackClass>();

    [Separator("SPECIAL")]
    public bool projectilImmunity;

    [Separator("FLESH")]
    public List<FleshClass> fleshList = new List<FleshClass>();

    [Separator("SOUL")]
    public int soulValue;
    
}
[System.Serializable]
public class FleshClass
{
    public ItemData flesh;
    public float chance;

}