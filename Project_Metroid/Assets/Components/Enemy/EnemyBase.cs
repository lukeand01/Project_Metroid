using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    //this will hold variables or fucntions that enemies can or need to use.

    [SerializeField] SoundHolderSO enemySoundHolder;
    [SerializeField] GameObject feet;
    //they should fly a bit farther because of kick.



    public EnemyData data;
    protected GameObject playerObject;

    public bool notReset;

    protected float healthMax;
    protected float healthCurrent;

    protected string animId;
    protected GameObject body;
    protected Animator anim;
    protected Rigidbody2D rb;

    protected const string PLAYERTAG = "Player";
    Vector3 originalPos;

   protected Sword sword;


    public event Action EventEnemyHit; //movement, attack, also when take damage.
    public void OnEnemyHit() => EventEnemyHit?.Invoke();

    public event Action<EnemyBase> EventEnemyDead; //the enemy died.
    public void OnEnemyDead(EnemyBase enemy) => EventEnemyDead?.Invoke(enemy);


    float loseRange = 70; //how far he needs to go to lose this enemy.


    Material originalMaterial;
    [SerializeField] Material flashMaterial;
    SpriteRenderer rend;



    public void CantLose()
    {
        chasing = true;
        cantLose = true;
    }

    private void Start()
    {
        EventEnemyHit += Hit;

        SetupAnimId();

        rb = GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).gameObject;
        sword = body.transform.GetChild(0).GetComponent<Sword>();
        rend = body.GetComponent<SpriteRenderer>();
        originalMaterial = rend.material;
       
        anim = body.GetComponent<Animator>();

        healthMax = data.health;
        healthCurrent = healthMax;
        playerObject = PlayerHandler.instance.gameObject;

        originalPos = transform.position;
        Observer.instance.EventResetEnemy += ResetEnemy;
    }


    void ResetEnemy()
    {
        if (notReset) return;

        //otherwise we snap this enemy back to its original position with full health.
        dead = false;
        transform.position = originalPos;
        healthCurrent = healthMax;

    }

    protected virtual void SetupAnimId()
    {
        animId = data.name;
    }

    void Hit()
    {
        //we stop any animation and play something else.
        anim.Play(GetAnimString("Hit"));

    }

    public bool dead;
    protected bool patrolling;
    protected bool attackCooldown;
    protected bool chasing;
    protected bool cantLose;
    protected bool behaveCooldown;
    bool fallingDown;
    public void FallingDown()
    {
        fallingDown = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }


    private void Update()
    {
        //

        if (fallingDown)
        {
           
            if (IsGrounded())
            {
                //
                Debug.Log("touched on ground");
                chasing = true;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                cantLose = true;
                fallingDown = false;
            }
            else
            {
                Debug.Log("falling down");
                return;
            }
            
        }

        Behavior();

    }

   protected virtual void Behavior()
    {
        
   

    }

   protected virtual bool CanBehave()
    {
        if (dead)
        {
            Move(0, 0);
            return false;
        }

        if (PlayerHandler.instance.untargetable)
        {
            //we stop all animation.
            Move(0, 0);         
            return false;
        }

        if (behaveCooldown)
        {
            
            Move(0, 0);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Hit")))
        {
            Move(0, 0);

            return false;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Attack")))
        {
            return false;
        }

        return true;
    }


    protected virtual void RefreshAttackCooldown() => attackCooldown = false;



    protected void MoveRandomly()
    {
        //we choose a random side. we walk to its limit and then we walk back.



    }

    protected virtual void Move(int dir, float speed)
    {
        Rotate(dir);
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        anim.Play(GetAnimString("Walk"));
    }

   protected virtual void Rotate(float dir)
    {

        if (dir == 0) return;

        //rotate to where you are facing.
        if (dir > 0)
        {
            
            body.transform.localPosition = new Vector3(0, 0, 0);
            body.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        if (dir < 0)
        {
            body.transform.rotation = new Quaternion(0, 180, 0, 0);
            body.transform.localPosition = new Vector3(-0.4f, 0, 0);
        }
    }


   protected void HandleAnimation()
    {
        //
        //handler.anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack")

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Hit")))
        {
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName(GetAnimString("Attack")))
        {
            return;
        }




    }

    protected int GetDir()
    {
        float dir = playerObject.transform.position.x - transform.position.x;

        if(dir > 0)
        {
            return 1;
        }
        if(dir < 0)
        {
            return -1;
        }

        return 0;
    }

    protected bool WallAhead()
    {
        return false;
    }
    protected bool GapAhead()
    {
        return false;
    }

    protected bool PlayerSpotted()
    {
        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        float yDifference = transform.position.y - playerObject.transform.position.y;

        if (yDifference > 4 || yDifference < -4)
        {
            return false;
        }

        return data.spotRange >= distance;
    }
    protected bool PlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        float yDifference = transform.position.y - playerObject.transform.position.y;

        if (yDifference > 4 || yDifference < -4)
        {
            return false;
        }


        return data.attackRange >= distance;
    }

    protected bool CanLose()
    {
        //you also lose them by moving up in the y.
        if (cantLose)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, playerObject.transform.position);
        float yDifference = transform.position.y - playerObject.transform.position.y;

        if(yDifference > 5 || yDifference < -5)
        {
            return true;
        }


        return loseRange >= distance;
    }

    //enemies will walk around, will chase and attack.
    public void TakeDamage(float damage, GameObject attacker, float pushModifier = 0)
    {
        if (dead) return;


        OnEnemyHit();
        MusicHandler.instance.CreateGenericSfx("EnemyHit");

        healthCurrent -= damage;

        if (healthCurrent <= 0)
        {
            Die();
        }
        else
        {
            anim.Play(GetAnimString("Hit"));
            StartCoroutine(HitProcess());
            if (pushModifier != 0)
            {
                StartCoroutine(KnockbackProcess(attacker, pushModifier));
            }
            else
            {                           
                Invoke("RefreshBehaveCooldown", 1);
            }
        }

    }


    void RefreshBehaveCooldown() => behaveCooldown = false;

    
    IEnumerator HitProcess()
    {
        rend.material = flashMaterial;
        yield return new WaitForSeconds(0.2f);
        rend.material = originalMaterial;
    }


    IEnumerator KnockbackProcess(GameObject attacker, float pushModifier)
    {
        int dir = GetDir();

        //maybe i have to adjust it a bit for the kick.
        Debug.Log("this is the dir " + GetDir());
        //the problem isnt the dir.

        //if its off by a little then i shouldnt affect.
        
        rb.AddForce(Vector2.right * dir * pushModifier, ForceMode2D.Force);
        rb.bodyType = RigidbodyType2D.Dynamic;
        int brake = 0;
        while (AnimationPlaying("Hit"))
        {
            brake++;
            if(brake > 1000)
            {
                Debug.Log("brake");
                yield break;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        rb.velocity = new Vector2(0, 0);
        rb.bodyType = RigidbodyType2D.Kinematic;
        yield return null;
    }

    void Knockback(GameObject attacker)
    {
        float dir = playerObject.transform.position.x - transform.position.x;
        
        if(dir > 0)
        {

        }
        if(dir < 0)
        {

        }
       rb.AddForce(Vector2.right * -7f, ForceMode2D.Force);
    }

    protected virtual void Die()
    {
        //when you die you become a body.
        //takes a while to disappear.
        //can be harvested if you have knife.
        OnEnemyDead(this);
        dead = true;
        gameObject.layer = 0;
        Move(0, 0);
        anim.Play(GetAnimString("Dead"));
        PlayerHandler.instance.GainSoul(data.soulValue);
        StartCoroutine(DeathProcess());
    }

    IEnumerator DeathProcess()
    {


        while (AnimationPlaying(GetAnimString("Dead")))
        {
            Debug.Log("dead animation");
        }
               
       // yield return new WaitForSeconds(1f);
        //add a script for corpse.
        gameObject.AddComponent<EnemyCorpse>().SetUp(data.fleshList);
        Destroy(this);
        yield return null;
        
    }


    protected string GetAnimString(string order)
    {
        return animId + "_" + order;

    }
    protected bool AnimationPlaying(string id)
    {
      
        return anim.GetCurrentAnimatorStateInfo(0).IsName(id);
       
               
    }

    protected bool IsGrounded()
    {
        //when the enemy touches ground.
        RaycastHit2D hit = Physics2D.Raycast(feet.transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Wall"));

        if (hit.collider) return true;

        return false;
    }
}
