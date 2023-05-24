using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilDemon : MonoBehaviour
{
    //projectil that looks only for player.

    float damage;
    Vector3 dir;
    float speed;

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    public void SetUp(Vector3 dir, float speed, float damage)
    {
        this.dir = dir;
        this.speed = speed;
        this.damage = damage;

        Invoke("DestroyItself", 10);
    }

    void DestroyItself()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable == null) return;

        damageable.TakeDamage(damage, gameObject, 0);
        Destroy(gameObject);
    }

}
