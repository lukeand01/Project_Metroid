using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWhiteSkeleton : EnemyBase
{
    //this skeleton is fast and will try to flank 
    //i want to exagerate the flanking


 

    protected override void SetupAnimId()
    {

        animId = "BasicSkeleton";
    }

    bool dashed = false;
      protected override void Behavior()
    {
        if (!CanBehave()) return;

        WhiteSkeletonBehavior();
       

    }


    void NewWhiteBehavior()
    {
        if (PlayerInRange())
        {
            Move(GetDir(), 0);

            if (!dashed)
            {
                //we have a chance of dash then we dash.
                int dashChance = Random.Range(0, 10);
                Debug.Log("inside here");
                Dash();
                if (dashChance > 5)
                {
                    //no dash
                    dashed = true;
                }
                if (dashChance <= 5)
                {
                    //dash. 

                }

            }

            Rotate(GetDir());

            if (attackCooldown && !dashed) return;
            Debug.Log("got here");
            attackCooldown = true;
            Invoke("RefreshAttackCooldown", data.attackSpeed);
            Move(0, 0);
            anim.Play(GetAnimString("Attack"));

            return;
        }

        if (PlayerSpotted())
        {
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

    protected override void RefreshAttackCooldown()
    {
        dashed = false;
        base.RefreshAttackCooldown();
    }


    void Dash()
    {
        dashed = true;
        transform.position += new Vector3(5 * GetDir(), 0, 0);

    }
   
    //when he is in range to attack he dashes first then attacks.

    void WhiteSkeletonBehavior()
    {
        base.Behavior();

        if (AnimationPlaying(GetAnimString("Hit")))
        {
            Debug.Log("White skeleton hit");
            rb.velocity = Vector2.zero;
            return;
        }


        if (!CanBehave()) return;

      
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
                Invoke("RefreshAttackCooldown", data.attackList[0].speed);
                Move(0, 0);
                sword.SetUp(PLAYERTAG, data.attackList[0].damage);
                anim.Play(GetAnimString("Attack"));
            }
            else
            {
                //we walk to him.

                float dir = playerObject.transform.position.x - transform.position.x;

                //but we will walk more 

                chasing = true;
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

        if (patrolling)
        {
            //we check if we go to the end
        }
    }

}
