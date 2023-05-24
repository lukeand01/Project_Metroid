using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBasicSkeleton : EnemyBase
{
    //this skeleton is slow and will always stop in front of the player.
    //

    //they should chase you if you have been spotted.


 

    protected override void Behavior()
    {
        base.Behavior();
        if (!CanBehave()) return;

        //they are slower.
        if (attackCooldown) return;

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

    }






    void BasicSkeletonBehavior()
    {

       


        if (PlayerSpotted())
        {
            patrolling = false;
            if (PlayerInRange())
            {
                float dir = playerObject.transform.position.x - transform.position.x;

                Rotate(dir);
                if (attackCooldown) return;

                //if thats every true then we walk to him
                attackCooldown = true;
                Invoke("RefreshAttackCooldown", data.attackSpeed);
                Move(0, 0);
                anim.Play(GetAnimString("Attack"));
            }
            else
            {
                //we walk to him.

                float dir = playerObject.transform.position.x - transform.position.x;



                if (dir > 0)
                {
                    //we walk to the right.
                    Move(1, data.chaseSpeed);
                }
                if (dir < 0)
                {
                    //we walk to the left.
                    Move(-1, data.chaseSpeed);
                }


            }
        }
        else
        {
            //then we walk randomly.
            patrolling = true;

        }


        if (patrolling)
        {
            //we check if we go to the end
        }
    }



}
