using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlackSlime : EnemyBase
{

    //the black slime shows it hands and moves to you. it behaves almost humna.
    //the green slimes charges at you like a hound.


    //when it first spots the playear then it does the animation.
    //after that is done then it starts following the player.

    bool ready;
    bool process;

    protected override void Behavior()
    {
        base.Behavior();
        if (!CanBehave()) return;
        
        if (attackCooldown) return;




        if (AnimationPlaying(GetAnimString("Ready")))
        {
            return;
        }


        if (AnimationPlaying(GetAnimString("Ready")))
        {
            Debug.Log("ready animation running");
            return;
        }


        if (chasing)
        {
            Debug.Log("chasing");
            if (CanLose())
            {
                Debug.Log("lost");
                chasing = false;
            }
            else
            {
                Debug.Log("move");
                Move(GetDir(), data.chaseSpeed);
            }
        }


        if (PlayerInRange())
        {
            //then it attacks.
            Attack();
            return;
        }

        if (PlayerSpotted())
        {
            //if the player is spotted then he changes his body to the hand stuff.
            if (ready)
            {
                //if he is ready. he chose the player.
                Move(GetDir(), data.chaseSpeed);
            }
            else
            {
              if(!process) StartCoroutine(ReadyProcess());
            }


        }

        
    }

    IEnumerator ReadyProcess()
    {

        process = true;

        anim.Play(GetAnimString("Ready"));

        while (AnimationPlaying(GetAnimString("Ready")))
        {

            
        }

        yield return new WaitForSeconds(1.1f);
        anim.Play(GetAnimString("Walk2"));
        ready = true;
        process = false;

        yield return null;
    }


    protected override void Move(int dir, float speed)
    {
        Rotate(dir);
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        anim.Play(GetAnimString("Walk2"));
    }

    protected override void Rotate(float dir)
    {
        if (dir == 0) return;

        //rotate to where you are facing.
        if (dir < 0)
        {

            body.transform.localPosition = new Vector3(0, 0, 0);
            body.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        if (dir > 0)
        {
            body.transform.rotation = new Quaternion(0, 180, 0, 0);
            body.transform.localPosition = new Vector3(-0.4f, 0, 0);
        }
    }

    void Attack()
    {
        if (!ready) return;
        //then we attack
        


        if (attackCooldown)
        {
            if (!AnimationPlaying(GetAnimString("Attack")))
            {
                anim.Play(GetAnimString("Walk2"));
                
            }
            return;
        }


       

        Move(0, 0);
        anim.Play(GetAnimString("Attack"));
        attackCooldown = true;
        Invoke("RefreshAttackCooldown", data.attackSpeed);

    }

}
