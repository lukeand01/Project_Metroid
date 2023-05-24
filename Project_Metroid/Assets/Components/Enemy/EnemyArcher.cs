using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArcher : EnemyBase
{

    bool repositioned;
    [SerializeField] GameObject template;
    [SerializeField] GameObject shootHolder;



    protected override void Behavior()
    {
        
        base.Behavior();
        if (!CanBehave()) return;
        //it just shoots towards the player.
        //it backaways from the player if too close.

        if (attackCooldown) return;

        if(chasing)
        {

            if (CanLose())
            {
                chasing = false;
            }
            else
            {
                int shootDir = GetDir();
                StartCoroutine(ShootArrowProcess(shootDir));
            }

        }


        if (PlayerInRange())
        {
            //we dash back. put the dash in cooldown.
            if (!repositioned)
            {
                //


            }
        }

        if (PlayerSpotted())
        {
            //we start shooting
            if (attackCooldown) return;
            Rotate(GetDir());

            int shootDir = GetDir();           
            StartCoroutine(ShootArrowProcess(shootDir));
        }

    }

    void RepositionRefresh() => repositioned = false;

    IEnumerator ShootArrowProcess(int shootDir)
    {

        anim.Play(GetAnimString("Attack"));
        while (AnimationPlaying(GetAnimString("Attack")))
        {
            Debug.Log("attacking");
            yield return null;
        }

        //yield return new WaitForSeconds(0.3f);
        ShootArrow(shootDir);
        attackCooldown = true;
        Invoke("RefreshAttackCooldown", data.attackList[0].speed);
    }



    void ShootArrow(int shootDir)
    {
        GameObject newObject = Instantiate(template, shootHolder.transform.position, Quaternion.identity);
        newObject.GetComponent<ArrowProjectil>().SetUp(shootDir, data.attackList[0].damage);
    }
}
