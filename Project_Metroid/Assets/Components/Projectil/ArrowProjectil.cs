using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectil : MonoBehaviour
{
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

        if (collision.gameObject.tag == "Player")
        {

            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage, gameObject);
                Destroy();
            }



        }

    }
}
