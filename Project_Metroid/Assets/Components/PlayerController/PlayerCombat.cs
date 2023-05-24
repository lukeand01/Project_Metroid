using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //i will store combat here instead. now now though.

    //types of attack:
    //basic, heavy, combo.
    //kick.


    PlayerHandler handler;

    public bool attackCooldown;

    private void Awake()
    {
        handler = GetComponent<PlayerHandler>();
    }

    //maybe we can handle throwing skills here and also magic


    //do we need combo?
        //maybe its better to have different attacks for different situations.

    //depending on the attack you move a bit forward.

    public void Attack()
    {
        

        //but we check combo before we check 
        if (attackCooldown) return;
        if (!handler.CanAct(10)) return;

        handler.SpendStamina(10);

        //damage is not being increased.
        handler.sword.SetUp("Enemy", 10);
        handler.anim.Play("Player_Attack");
        attackCooldown = true;
        MusicHandler.instance.CreateSFX(handler.playerSoundHolder.GetClip("Attack1"));
        Invoke("RefreshAttack", 0.6f);
        handler.OnPlayerInput(false);
    }

    public void AttackHeavy()
    {
        //
        if (attackCooldown) return;
        


    }

    public void Kick()
    {
        if (attackCooldown) return;

        handler.sword.SetUp("Enemy", 1, 5000);
        handler.anim.Play("Player_Kick");
        attackCooldown = true;
        Invoke("RefreshAttack", 0.7f);
        handler.OnPlayerInput(false);
    }

    void RefreshAttack() => attackCooldown = false;

    public void Dash()
    {
        //become insivible for a second. dash foward.

    }

    public void ThrowFire()
    {
        //difference between fire and dagger.
            //
            

    }

    public void ThrowDagger()
    {

    }


}
