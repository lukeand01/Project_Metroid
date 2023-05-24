using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerProjectil : MonoBehaviour
{
    //it goes in a straight line.
    //it stops at any wall or enemy.
    //deal damage.

    //might inflicts status.
    int dir;
    float damage;
    Rigidbody2D rb;

    

    public void SetUp(int dir, float damage)
    {
        rb = GetComponent<Rigidbody2D>();

        this.dir = dir;
        this.damage = damage;

        Invoke("Destroy", 10);
    }

    private void Update()
    {
        rb.velocity = new Vector2(dir * 10, rb.velocity.y);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            if (enemy.data.projectilImmunity)
            {
                Debug.Log("projectil immunity");
                Destroy(gameObject);
                return;
            }

            if(damageable != null)
            {
                damageable.TakeDamage(damage, gameObject);
                Destroy();
            }


           
        }

    }


}
