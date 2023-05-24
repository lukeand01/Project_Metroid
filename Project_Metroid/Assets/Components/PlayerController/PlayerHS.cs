using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHS : MonoBehaviour, IDamageable, ISaveable
{
    //control player health and stamina.

    PlayerHandler handler;





    private void Start()
    {
        handler = GetComponent<PlayerHandler>();

        healthMax = healthInitial;
        healthCurrent = healthMax;

        staminaMax = staminaInitial;
        staminaCurrent = staminaMax;

    }

    public float staminaInitial;
    float staminaMax;
    float staminaCurrent;
    float staminaBonus;
    
 
    public float healthInitial;
    float healthMax;
    float healthCurrent;
    float healthBonus;
    float healthResult;

    public float staminaCooldown;
    float staminaCooldownCurrent = 0;
    public float staminaRecoveryRate;
    float staminaRecoveryBonus;

    bool recoveryStaminaProcess;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            //
            TakeDamage(100, gameObject, 0);
        }

        if (staminaCurrent >= staminaMax) return;
        if (recoveryStaminaProcess) return;

        if(staminaCooldownCurrent >= staminaCooldown)
        {
            //we start recovering stamina.
            StartCoroutine(RecoveryStaminaProcess());
        }
        else
        {
            staminaCooldownCurrent += Time.deltaTime;
        }
    }


    

    public void UpdateStaminaBonus(float staminaBonus)
    {
        //current stamina is not affected by its
        //max.
        this.staminaBonus = staminaBonus;



        if(staminaCurrent > staminaMax + staminaBonus)
        {
            staminaCurrent = staminaMax + staminaBonus;
        }

        handler.hud.UpdateStamina(staminaCurrent, staminaCurrent, staminaMax + staminaBonus);
    }

    public void UpdateHealthBonus(float healthBonus)
    {     
        this.healthBonus = healthBonus;
        Debug.Log("health bonus " + healthBonus);
        handler.hud.UpdateHealth(healthCurrent, healthCurrent, healthMax + healthBonus);
    }

    public void UpdateStaminaRecoveryBonus(float staminaRecoveryBonus)
    {
        this.staminaRecoveryBonus = staminaRecoveryBonus;
    }

    public void SpendStamina(float value)
    {
        recoveryStaminaProcess = false;
        staminaCooldownCurrent = 0;
        staminaCurrent -= value;
        StopAllCoroutines();
        Mathf.Clamp(staminaCurrent, 0, staminaMax);
        handler.hud.UpdateStamina(staminaCurrent + value, staminaCurrent, staminaMax + staminaBonus);
    }

    public bool CanAct(float value)
    {
        return staminaCurrent >= value;
    }

    IEnumerator RecoveryStaminaProcess()
    {

        recoveryStaminaProcess = true;
        while(staminaCurrent < staminaMax + staminaBonus)
        {

            handler.hud.UpdateStamina(staminaCurrent, staminaCurrent + staminaRecoveryRate, staminaMax);
            staminaCurrent += staminaRecoveryRate + staminaRecoveryBonus;
            staminaCurrent = Mathf.Clamp(staminaCurrent, 0, staminaMax);
            
            yield return new WaitForSeconds(0.1f);
        }

        recoveryStaminaProcess = false;
    }

    public void RecoverHealth(float value)
    {
        handler.hud.UpdateHealth(healthCurrent, healthCurrent + value, healthMax + healthBonus);
        healthCurrent += value;
        healthCurrent = Mathf.Clamp(healthCurrent, 0, healthMax);
       
    }


    bool hitCooldown;
    void RefreshHit() => hitCooldown = false;

    public void TakeDamage(float damage, GameObject attacker, float pushModifier = 0)
    {
        if (hitCooldown) return;
        if (dead) return;


        handler.hud.UpdateHealth(healthCurrent, healthCurrent - damage, healthMax);
        healthCurrent -= damage;
        hitCooldown = true;
        Invoke("RefreshHit", 0.5f);
        
        if (attacker == null)
        {
            handler.OnDamaged(true);
        }
        else
        {
            handler.OnDamaged(false);
            handler.anim.Play("Player_Hit");
        }

        if (healthCurrent <= 0)
        {
            Die();
            return;
        }

        MusicHandler.instance.CreateSFX(handler.playerSoundHolder.GetClip("Hit"));

        if (pushModifier != 0)
        {
            //thenw e turn this dynamic and push it while the hit animation is running.

            StartCoroutine(KnockbackProcess(GetPushDir(attacker), pushModifier));
            //we have to activate the rigidbody then push it away.

        }


        
    }

    protected int GetPushDir(GameObject attacker)
    {
        float dir = attacker.transform.position.x - transform.position.x;

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

    IEnumerator KnockbackProcess(int dir, float pushModifier)
    {
        Debug.Log("knockback");
        handler.rb.bodyType = RigidbodyType2D.Dynamic;
        handler.rb.AddForce(Vector2.right * dir * 1500 * pushModifier, ForceMode2D.Force);

        int brake = 0;
        while (handler.anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Hit"))
        {
            Debug.Log("hit");
            brake++;
            if(brake > 10000)
            {
                Debug.Log("broke");
                yield break;
            }
        }

       
        yield return new WaitForSeconds(1f);
        handler.rb.velocity = new Vector2(0, 0);
        handler.rb.bodyType = RigidbodyType2D.Kinematic;

    }


    public bool dead;
    void Die()
    {
        dead = true;

        handler.OnPlayerDead();

        handler.untargetable = true;
        handler.gameObject.tag = "Invisible";
        handler.anim.Play("Player_Dead");
        PlayerHandler.instance.AddBlock("Death", PlayerHandler.BlockType.Complete);
        MusicHandler.instance.CreateSFX(handler.playerSoundHolder.GetClip("Dead"));
        //move the player a bit up
        transform.position += new Vector3(0, 0.5f, 0);
        handler.rb.velocity = new Vector2(0, 0);
        StartCoroutine(DeathProcess());
    }

    IEnumerator DeathProcess()
    {
        while (handler.anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Dead"))
        {

        }

        yield return new WaitForSeconds(2);
        //then we tell gamehandler to fade in.
        GameHandler.instance.Death();
        yield return null;

    }

    #region SAVE SYSTEM
    
    public object CaptureState()
    {
        return new SaveData
        {
            healthMax = healthMax,
            healthCurrent = healthCurrent,

            staminaMax = staminaMax,
            staminaCurrent = staminaCurrent,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        healthMax = saveData.healthMax;
        healthCurrent = saveData.healthCurrent;
        handler.hud.UpdateHealth(healthCurrent, healthCurrent, healthMax);


        staminaMax = saveData.staminaMax;
        staminaCurrent = saveData.staminaCurrent;
        handler.hud.UpdateStamina(staminaCurrent, staminaCurrent, staminaMax);

    }

    [System.Serializable]
    struct SaveData
    {
        public float healthMax;
        public float healthCurrent;

        public float staminaMax;
        public float staminaCurrent;
    }
    
    #endregion
}
