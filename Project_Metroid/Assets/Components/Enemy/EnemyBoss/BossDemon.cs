using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDemon : BossBase, IDamageable
{
    BossDemonBody demonBody;
   

    //he has three moves:
    //basic - just attacks
    //second - grinds the ground and creates fireball
    //third - blast around him.

    private void Start()
    {
        demonBody = transform.GetChild(0).GetComponent<BossDemonBody>();
        demonBody.SetUp(this, attackList[3]);
        //StartBoss();

    }
    float chanceOfSecondAttack = 0;
    private void Update()
    {

        if (!started) return;
        if (dead) return;
        if (attackCooldown) return;

        if (Attacking())
        {

            return;
        }
        //

        if (PlayerTooClose())
        {
            //chance of explosion attack.
            Attack3();
            return;
        }

        if (PlayerInRange())
        {

            if (secondPhase && !demonBody.isRain)
            {
                //then there is a chance of doing another attack
                int randomRange = Random.Range(0, 100);

                if (chanceOfSecondAttack > randomRange)
                {
                    //do the thing.
                    Attack2();
                    return;
                }
                else
                {
                    chanceOfSecondAttack += 0.4f;
                }

                Attack1();
                return;
            }

            //we check here which side the player is.
            Attack1();
            return;
        }
        else
        {
            if (secondPhase && !demonBody.isRain)
            {
                //then there is a chance of doing another attack
                int randomRange = Random.Range(40, 100);

                if (chanceOfSecondAttack > randomRange)
                {
                    //do the thing.
                    Attack2();

                    return;
                }
                else
                {
                    Debug.Log("increasing chance");
                    float randomIncrease = Random.Range(0.01f, 0.1f);
                    chanceOfSecondAttack += randomIncrease;
                }


            }
        }

        //if in second phase

        


        Move(GetDir(), moveSpeed);
       
       

    }

    bool Attacking()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Attack1")))
        {
            Debug.Log("was attacking1");
            return true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Attack2")))
        {
            Debug.Log("was attacking2");
            return true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Attack3")))
        {
            Debug.Log("was attacking3");
            return true;
        }

        return false;
    }

    void Attack1()
    {
        //this is the cleave attack. pretty basic just attack in front.
        sword.SetUp("Player", attackList[0].damage);
        anim.Play(GetAnimString("Attack"));
        rb.velocity = new Vector2(0, rb.velocity.y);
        attackCooldown = true;
        OrderRefreshAttackCooldown(attackList[0].speed);
    }
    void Attack2()
    {
        //scracth the ground. shoot a bunch of fireballs sideways and up, which will fall.
        sword.SetUp("Player", attackList[1].damage);
        chanceOfSecondAttack = 0;
        anim.Play(GetAnimString("Attack2"));
        rb.velocity = new Vector2(0, rb.velocity.y);
        attackCooldown = true;
        OrderRefreshAttackCooldown(attackList[1].speed);
    }
    void Attack3()
    {
        //this is the area around attack. attacks when the player is too close.
        //shoots fireballs around.
        sword.SetUp("Player", attackList[2].damage, 2);
        anim.Play(GetAnimString("Attack3"));
        rb.velocity = new Vector2(0, rb.velocity.y);
        attackCooldown = true;
        OrderRefreshAttackCooldown(attackList[2].speed);
    }


    //if the player dies it resets.

    void Move(int dir, float speed)
    {
        Rotate(dir);
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        anim.Play(GetAnimString("Walk"));
    }

     void Rotate(float dir)
    {

        if (dir == 0) return;

        //rotate to where you are facing.
        if (dir > 0)
        {
            //body.transform.localPosition = new Vector3(0, 0, 0);
            body.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        if (dir < 0)
        {
            body.transform.rotation = new Quaternion(0, 0, 0, 0);
           // body.transform.localPosition = new Vector3(-0.4f, 0, 0);
        }
    }

    int GetDir()
    {
        float dir = playerObject.transform.position.x - transform.position.x;

        if (dir > 0)
        {
            return 1;
        }
        if (dir < 0)
        {
            return -1;
        }

        return 0;
    }

    string GetAnimString(string order)
    {
        return animId + "_" + order;

    }

    bool PlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        return attackRange >= distance;
    }
    bool PlayerTooClose()
    {
        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        return tooCloseRange >= distance;
    }

    public void TakeDamage(float damage, GameObject attacker, float pushModifier = 0)
    {
        if (dead) return;
        //it shows damage through fade.
        currentHealth -= damage;
        StartCoroutine(HitProcess());

        if(currentHealth <= 0)
        {
            Die();
            return;
        }

        if(currentHealth <= secondPhaseHealth)
        {
            SecondPhase();
        }
    }

    IEnumerator HitProcess()
    {
        Debug.Log("hit process");
        //fade in and out quickly.
        SpriteRenderer rend = demonBody.gameObject.GetComponent<SpriteRenderer>();
        rend.color = Color.white;
        
        while(rend.color.r > 0)
        {

            rend.color -= new Color(0.1f, 0.1f, 0.1f, 0);
            yield return new WaitForSeconds(0.02f);
        }

        rend.color = Color.white;

        yield return null;
    }

    void Die()
    {
        Debug.Log("dead");
        dead = true;
        anim.Play(GetAnimString("Dead"));


        StartCoroutine(DieProcess());
    }

    IEnumerator AwardProcess()
    {

        PlayerHandler.instance.GainKey(0);
        yield return new WaitForSeconds(3);
        PlayerHandler.instance.GainSoul(soulValue);
    
    }

    IEnumerator DieProcess()
    {
        while (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Dead")))
        {
            Debug.Log("dead running");
            yield return null;
        }

        Debug.Log("award process");
        StartCoroutine(AwardProcess());
        //open the doors.
        OnBossDefeated();
        //tell the door to open
    }

    void SecondPhase()
    {
        secondPhase = true;

        for (int i = 0; i < attackList.Count; i++)
        {
            attackList[i].IncreaseStats(1.3f, 0.7f);
        }


        //put effects.
    }



}
