using DG.Tweening;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class BossReaper : BossBase, IDamageable
{
    //the room will be dark.
    //you will have to defeat the dark fire to weaken the creature.
    //in the meantime the reaper will be attacking the player.
    //the dark fire spawns basic skeletons while they are active.


    //TO DO:
        //the first attack is behaving weird.
        //handle the respawns.
        //create the second phase.
        //archer respawns after every idle.
        //lancer respawns once when stance is changed.
        //change firerespawner in some way to show it has been defeated.


    Vector3 originalPos;
    ReaperStates state;
    enum ReaperStates
    {
        Shadow,//attacking from teh shadows.
        Vulnerable, //at player's mercy to attack
        Duel, //it is visible and it fights the player.
        Kite, //this behavior it will teleport to a corner of the room whilte the enemy go after it.
    }

    bool slashing;

    [Separator("RESPAWNERS")]
    [SerializeField] BossReaperFire fire1;
    [SerializeField] BossReaperFire fire2;
    [SerializeField] SingleRespawner archer1;
    [SerializeField] SingleRespawner archer2;
    [SerializeField] SingleRespawner lancer1;
    [SerializeField] SingleRespawner lancer2;



    SpriteRenderer rend;
    bool start;
    //

    private void Start()
    {
        rend = transform.GetChild(0).GetComponent<SpriteRenderer>();
        originalMaterial = rend.material; 
        
    }

    public override void StartBoss()
    {
        base.StartBoss();

        
        if (secondPhase)
        {
            //then we change a couple of stuff.
            //then it starts in the 
            transform.position = vulnerableTransform.position;
            gameObject.layer = 7;
            //then it starts moving.
            state = ReaperStates.Duel;
            Invoke("StartCall", 4);
            //it either moves towards or away from the player. it summons lancer and archer.         
            return;
        }

        fire1.Activate(this);
        fire2.Activate(this);

        originalPos = transform.position;

        Invoke("StartCall", 5);
    
    }

    void StartCall() => start = true;

    public void FireDestroyed()
    {
        if(!fire1.activated && !fire2.activated)
        {
            TurnFragile();
        }


    }

    [ContextMenu("TURN FRAGILE")]
    void TurnFragile()
    {
        //stop any animation or behavior.
        //anim.enabled = false;
        //anim.enabled = true;

        //while vulenrable it waits.
        //it moves to the center of the room.
        VulnerableBehavior();
       
    }


    private void Update()
    {
        if (!start) return;

        if(slashing && state == ReaperStates.Shadow)
        {
            //moves towards the thing
            HandleSlash();
        }

        if (attackCooldown) return;

        if (AnimPlaying("Attack"))
        {

            return;
        }
        if (AnimPlaying("Attack2"))
        {
            Debug.Log("attack2 anim");
            return;
        }
        if (AnimPlaying("Summon"))
        {
            Debug.Log("Summon anim");
            return;
        }


        Debug.Log("current state " + state);
        switch (state)
        {

            case ReaperStates.Shadow:
                ShadowBehavior();
                break;
            case ReaperStates.Vulnerable:
                //VulnerableBehavior();
                break;
            case ReaperStates.Duel:
                DuelBehavior();
                break;
            
        }

        
    }
    

    #region SHADOW
    void ShadowBehavior()
    {
       
        if (slashing) return;



        StartSlash();
    }

    Vector3 currentTarget;
    bool slashSecondStage;
    Vector3 slashLaunchPosition;
    [SerializeField] float slashSpeed;
    void HandleSlash()
    {
        if (!slashing) return;

        if (slashSecondStage)
        {

            transform.position = Vector3.MoveTowards(transform.position, slashLaunchPosition, slashSpeed * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, slashLaunchPosition);

            if (distance <= 0.5f)
            {
                EndSlash();
                return;
            }
        }
        else
        {

            transform.position = Vector3.MoveTowards(transform.position, currentTarget, slashSpeed * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, currentTarget);

            if (distance <= 0.5f)
            {
                slashSecondStage = true;
                return;
            }

        }



        //but he should continue

    }


    void StartSlash()
    {

        //get location
        currentTarget = playerObject.transform.position;
        float random = Random.Range(-20, 20);
        slashLaunchPosition = new Vector3(originalPos.x + (random * -1), originalPos.y - 20, 0);
        transform.position = new Vector3(originalPos.x + random, originalPos.y, 0);
        anim.Play(animId + "_Slashing");

        sword.SetUp("Player", attackList[0].damage);

        slashing = true;
    }
    void EndSlash()
    {
        slashing = false;
        slashSecondStage = false;

        OrderRefreshAttackCooldown(attackList[0].speed);
    }




    #endregion


    [Separator("VULNERABLE")]
    [SerializeField] Transform vulnerableTransform;
    [SerializeField] float vulnerableTiming;

    [ContextMenu("FORCE VULNERABLE")]
    void VulnerableBehavior()
    {
        //it just sits idle while the player attacks him for a duration.
        //goes to the center and change to vulnerable anim.

        transform.position = vulnerableTransform.position;
        transform.position += new Vector3(0, 10, 0);

        //then it comes down.
        transform.DOMove(vulnerableTransform.position, 3);

        state = ReaperStates.Vulnerable;

        StartCoroutine(VulnerableProcess());
    }

    IEnumerator VulnerableProcess()
    {
        TakeDamage(5, null);
        slashing = false;
        slashSecondStage = false;
        anim.enabled = false;
        sword.gameObject.SetActive(false);
        yield return new WaitForSeconds(vulnerableTiming);
        //return to shadow

        anim.enabled = true;

        fire1.Activate(this);
        fire2.Activate(this);



        if (!secondPhase)
        {
            HandleSlash();
            state = ReaperStates.Shadow;
        }
               
    }


    #region DUEL
    void DuelBehavior()
    {
        //teh room is clear.
        //he will use summons.
        //he will attack also.

        //moves towards the player.
        //when close attack.
        //when that is happening there will be a summon moving around.



        if (attackCooldown) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BossReaper_Attack")) 
        {
            Debug.Log("attack anim");
            return;
        }


        if (PlayerInRange())
        {
            Debug.Log("in range");
            //attack.
            anim.Play("BossReaper_Attack");
            attackCooldown = true;
            Move(0, 0);
            sword.SetUp("Player", attackList[1].damage);
            OrderRefreshAttackCooldown(attackList[1].speed);
        }
        else
        {
            //move
            Move(GetDir(), moveSpeed);
        }




    }
    
    bool PlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, PlayerHandler.instance.transform.position);

        return attackRange >= distance;


    }


    protected virtual void Move(int dir, float speed)
    {

        Rotate(dir);
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        
    }

    protected virtual void Rotate(float dir)
    {

        if (dir == 0) return;

        //rotate to where you are facing.
        if (dir > 0)
        {

            body.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        if (dir < 0)
        {
            body.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }
    #endregion


    #region KITE

    [Separator("KITE POSITIONS")]
    [SerializeField] Transform leftPos;
    [SerializeField] Transform rightPos;

    [ContextMenu("TESTE SPAWN")]
    void TesteSpawn()
    {
        archer1.Spawn();
        archer2.Spawn();

        lancer1.Spawn();
        lancer2.Spawn();
    }

    void KiteBehavior()
    {
        //it will choose one of the two sides farthest from the player and spawn fellas.
        
        transform.position = GetFarthestKitePos().position;
        //rotate to the player
        Rotate(GetDir());
        //spawn archer and lancer.
        //spawn archer by his side.
        //spawn lancer in the other side.

        if(GetDir() == 1)
        {
            //this means left
            archer1.Spawn();
            lancer2.Spawn();
        }
        if(GetDir() == -1)
        {
            //this means right.
            archer2.Spawn();
            lancer1.Spawn();
        }

    }

    Transform GetFarthestKitePos()
    {
        float leftDistance = Vector3.Distance(leftPos.position, PlayerHandler.instance.transform.position);
        float rightDistance = Vector3.Distance(rightPos.position, PlayerHandler.instance.transform.position);

        if(leftDistance > rightDistance)
        {
            return leftPos;
        }
        else
        {
            return rightPos;
        }

    }

    #endregion

    bool AnimPlaying(string id)
    {
        if (anim == null) return true;
        return anim.GetCurrentAnimatorStateInfo(0).IsName("BossReaper_" + id);
    }


    float nextStateChange;
    [SerializeField] float healthChange;

    public void TakeDamage(float damage, GameObject attacker, float pushModifier = 0)
    {
        if (state == ReaperStates.Shadow) return;

        currentHealth -= damage;
        StartCoroutine(HitProcess());

        if (currentHealth <= secondPhaseHealth)
        {
            //then we start the process.
            //when we take damage

            slashing = false;
            slashSecondStage = false;
            
            
            if (secondPhase)
            {
                //if it is then that means we are are in either duel or kite.
                //
                if(currentHealth < nextStateChange)
                {
                    //then we change stuff.
                    ChangePhase();
                }

            }
            else
            {
                //if its not we gonna change that.
                secondPhase = true;
                gameObject.layer = 7;
                DecideNextPhase();               
            }            


        }
        if(currentHealth <= 0)
        {
            Die();
        }
        
    }

    void DecideNextPhase()
    {

        int random = Random.Range(0, 2);

        nextStateChange = currentHealth - healthChange;

        StopCoroutine(VulnerableProcess());

        if (random == 0)
        {
            //duel
            state = ReaperStates.Duel;

        }
        if (random == 1)
        {
            //kite
            state = ReaperStates.Kite;
            KiteBehavior();
        }
    }

    void ChangePhase()
    {
        nextStateChange = currentHealth - healthChange;

        StopCoroutine(VulnerableProcess());

        if (state == ReaperStates.Duel)
        {
            state = ReaperStates.Kite;
            KiteBehavior();



            return;
        }

        if(state == ReaperStates.Kite)
        {
            //move to the center.
            transform.position = vulnerableTransform.position;
            Invoke("ChangeToDuel", 1.5f);
            return;
        }
    }

    
    void ChangeToDuel() => state = ReaperStates.Duel;

    [ContextMenu("FORCE DEATH")]
    void Die()
    {
        //play an animation. in the end the player gains the key
        PlayerHandler.instance.GainSoul(soulValue);
        transform.position = originalPos;
        StartCoroutine(DieProcess());
        
    }

  
    IEnumerator DieProcess()
    {
        
        yield return new WaitForSeconds(1);
        Debug.Log("done here");
        OnBossDefeated();

        PlayerHandler.instance.GainKey(0);
        PlayerHandler.instance.Warn("Gained The Black Key");
        Destroy(gameObject);
    }

    IEnumerator SecondPhaseProcess()
    {
        //the room becomes lighter. the fires are disabled.
        //now it will fight the player instead of hiding in the shadow.
        //it will summon dark fires to help him.


        yield return null;
    }


    [SerializeField] Material flashMaterial;
    Material originalMaterial;

    IEnumerator HitProcess()
    {
        //turn it bright for a second.
        rend.material = flashMaterial;
        yield return new WaitForSeconds(0.2f);
        rend.material = originalMaterial;
    }
}
