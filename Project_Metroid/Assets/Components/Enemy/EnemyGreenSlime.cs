using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGreenSlime : EnemyBase
{


    protected override void SetupAnimId()
    {
        animId = "BlackSlime";
    }

    //there is a cooldown when he is coming. and when he is close attacking.

    bool chargeCooldown;
    int dir;
    protected override void Behavior()
    {
        base.Behavior();
        if (!CanBehave()) return;
        //
        if (AnimationPlaying(GetAnimString("Attack3")))
        {
            Move(0,0);
            return;
        }

        if (AnimationPlaying(GetAnimString("Attack2")))
        {
            //if we get here then we move.
            Move(dir, data.chaseSpeed);
            return;
        }
        
        if (attackCooldown) return;

        if (chasing)
        {
            Debug.Log("green slime is chasing");
            if (CanLose())
            {
                chasing = false;
            }
            else
            {
                Move(GetDir(), data.chaseSpeed);
            }
        }

        if (PlayerInRange())
        {
            //for now we do the third attack.
            
 
           

            int random = Random.Range(0, 10);

            if(random <= 6)
            {
                anim.Play(GetAnimString("Attack3"));
                sword.SetUp("Player", 25);
            }
            if(random > 6)
            {
                anim.Play(GetAnimString("Attack2"));
                sword.SetUp("Player", 5);
                dir = GetDir();
            }
            attackCooldown = false;
            RefreshCooldown(5);
            return;
        }

        if (PlayerSpotted())
        {
            //we jump.

            anim.Play(GetAnimString("Attack2"));
            dir = GetDir();
            attackCooldown = false;
            RefreshCooldown(3);
        }


    }

    void RefreshCooldown(float timer)
    {
        Invoke("RefreshAttackCooldown", timer);
    }
    protected override void Move(int dir, float speed)
    {
        Rotate(-dir);
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);

    }
}



//what does lance do?
    //he has a really long range.

//BOSS. at least 2, maybe if there is time 3.
    //Fire Demon - slow, -gain fire ball.
    //reaper - fast, turns invisible - gain dash.
    //big slime - consumes stuff.  - 

//FIRE DEMON:   
    //walk slowly to player.
    //fire boulders fall down.
    //spit fire in one direction

//Reaper:
    //he quickly attacks from one side to another. then stops somewhere.
    //he has a movement where he goes from one side rotating.
    //

//Mage:
    //shoots a bunch of projectils.
    //creates clones. has to hit the right one.
    //

//Big Slime
    //bounces and falls.


//what the shrine gives?
    //short buff - buff to a stat.
    //long buff:
        //1 - hunting knife
        //2 - 
        //3 -
        //4 -
        //5 -