using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour, IDamageable
{
    //this takes damage.

    public float maxHealth;
    public float currentHealth;

    public void SetUp()
    {

    }

    public void TakeDamage(float damage, GameObject attacker, float pushModifier = 0)
    {
        Debug.Log("dummy took damage");
    }

    



}
