using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossBase : MonoBehaviour, ISaveable
{
    //a couple of function to streamline the boss scripts.
    protected bool dead;
    protected bool attackCooldown;
    protected bool started;
    [SerializeField]protected bool secondPhase;



    [Separator("STATS")]
    [SerializeField] protected float attackRange;
    [SerializeField] protected float tooCloseRange;
    [SerializeField] protected List<AttackClass> attackList = new List<AttackClass>();
    [SerializeField] protected float moveSpeed;
    [Separator("HEALTH")]
    [SerializeField] protected float initialHealth;
    [SerializeField] protected float secondPhaseHealth;
    protected float currentHealth;

    [Separator("COMPONENTS")]
    [SerializeField] protected Sword sword;
    protected GameObject body;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected GameObject playerObject;
    protected string animId;

    [Separator("REWARD")]
    [SerializeField] protected int soulValue;

    public event Action EventBossDefeated;

    Vector3 initialPos;

    public void OnBossDefeated() => EventBossDefeated?.Invoke();

    private void Start()
    {
        initialPos = transform.position;
    }


    protected void ControlSword(bool choice)
    {
        sword.gameObject.SetActive(choice);
    }

    public virtual void StartBoss()
    {
        //when he starts he will move towards the player.
        //
        started = true;
        playerObject = PlayerHandler.instance.gameObject;
        body = transform.GetChild(0).gameObject;
        anim = body.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animId = gameObject.name;
        currentHealth = initialHealth;
        initialPos = transform.position;
    }

    protected void OrderRefreshAttackCooldown(float timer)
    {
        Invoke("RefreshAttackCooldown", timer);
    }
    void RefreshAttackCooldown() => attackCooldown = false;

    protected int GetDir()
    {
        float dir = playerObject.transform.position.x - transform.position.x;

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


    #region SAVE SYSTEM
    public object CaptureState()
    {
        return new SaveData
        {
            dead = dead,
        };
    }
    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        dead = saveData.dead;
        started = false;
        if (!dead)
        {
            currentHealth = initialHealth;

        }

    }

    [System.Serializable]
    struct SaveData
    {
        //if it restores and it isnt dead then we recover death.

        public bool dead;


    }

    #endregion

}

[System.Serializable]
public class AttackClass
{
    public float damage;
    public float speed;

    public void IncreaseStats(float damageModifier, float speedModifier)
    {
        damage *= damageModifier;
        speed *= speedModifier;

    }
}