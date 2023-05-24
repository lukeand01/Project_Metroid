using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLancer : EnemyBase
{
    //
    protected override void Behavior()
    {
        base.Behavior();
        if (!CanBehave()) return;

        //the lance is pretty straightforward how it works. the only problem is that they are so big.
        if (attackCooldown) return;


        if (chasing)
        {

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
            //if player is ever in range is the first thing we will care.
            //then we stop and attack.
            Move(GetDir(), 0);
            // float dir = playerObject.transform.position.x - transform.position.x;

            Rotate(GetDir());
            if (attackCooldown) return;

            //if thats every true then we walk to him
            attackCooldown = true;
            sword.SetUp(PLAYERTAG, data.attackList[0].damage);
            Invoke("RefreshAttackCooldown", data.attackList[0].speed);
            anim.Play(GetAnimString("Attack"));
            return;
        }

        if (PlayerSpotted())
        {

            //we start walking to him.

            chasing = true;
            Move(GetDir(), data.chaseSpeed);
        }

        

    }


}
