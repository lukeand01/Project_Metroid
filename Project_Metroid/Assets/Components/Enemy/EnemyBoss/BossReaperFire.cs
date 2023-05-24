using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossReaperFire : MonoBehaviour, IDamageable
{
    //while this is active it will spawn activated enemies against the player. it does not spawn if the player is close enough.
    [SerializeField] GameObject enemyTemplate;
    public bool activated;
    BossReaper boss;
    GameObject playerObject;
    [SerializeField] float range = 1.5f;
    [SerializeField] float maxHealth;
    float currentHealth;
    [SerializeField] float respawnCooldown;
    [SerializeField] int enemyLimit = 3;
    float currentRespawnCooldown;
    ParticleSystem particle;
    SpriteRenderer bodyRend;
    Color originalColor;

    List<EnemyBase> currentEnemieList = new List<EnemyBase>(); //

    private void Awake()
    {
        bodyRend = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        originalColor = bodyRend.color;
        particle = transform.GetChild(1).GetComponent<ParticleSystem>();
       
    }

    private void Start()
    {
        playerObject = PlayerHandler.instance.gameObject;
    }

    private void Update()
    {
        HandleEnemyRespawn();
    }

    void RemoveEnemy(EnemyBase enemy)
    {
        //
        for (int i = 0; i < currentEnemieList.Count; i++)
        {
            if (currentEnemieList[i].GetInstanceID() == enemy.GetInstanceID())
            {
                Debug.Log("found someone");
                currentEnemieList[i].EventEnemyDead -= RemoveEnemy;
                currentEnemieList.RemoveAt(i);
                return;
            }

        }


    }

    public void Activate(BossReaper boss = null)
    {
        if(boss != null)
        {
            this.boss = boss;
            this.boss.EventBossDefeated -= ResetFire;
        }

        bodyRend.color = originalColor;
        PlayerHandler.instance.EventPlayerDead += ResetFire;

        particle.Play();
        activated = true;
        currentHealth = maxHealth;

       //CreateEnemy();
    }

    void ResetFire()
    {
        //destroy all fellas.

        for (int i = 0; i < currentEnemieList.Count; i++)
        {
            currentEnemieList[i].EventEnemyDead -= RemoveEnemy;
            Destroy(currentEnemieList[i].gameObject);
        }

        Desactivate();
        currentEnemieList.Clear();
    }

   void HandleEnemyRespawn()
    {
        
        
        if (!activated)
        {
            currentRespawnCooldown = 0;
            return;
        }

        if (PlayerInRange())
        {

            currentRespawnCooldown = 0;
            return;
        }


        if (currentRespawnCooldown >= respawnCooldown)
        {

            if (currentEnemieList.Count >= enemyLimit)
            {
                return;
            }

            currentRespawnCooldown = 0;
            CreateEnemy();
        }
        else
        {
            currentRespawnCooldown += Time.deltaTime;

        }
    }

    [ContextMenu("FORCE SPAWN")]
    void CreateEnemy()
    {
        GameObject newObject = Instantiate(enemyTemplate, transform.position, Quaternion.identity);
        EnemyBase enemy =  newObject.GetComponent<EnemyBase>();
        enemy.CantLose();
        currentEnemieList.Add(enemy);
        enemy.EventEnemyDead += RemoveEnemy;
    }

    //if it resets then we destroy all.


    bool PlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        return distance < range;
    }

    void Desactivate()
    {
        //change color to show it has been destroyed.
        //
        bodyRend.color = Color.black;
        //stops spawning.
        particle.Stop();
        activated = false;
        boss.FireDestroyed();
        StopAllCoroutines();
        //when both are destroyed he goes down so the player can hit him.
    }

    public void TakeDamage(float damage, GameObject attacker, float pushModifier = 0)
    {

        currentHealth -= damage;

        //show that it took damage.

        if(currentHealth <= 0)
        {
            Desactivate();
        }
        else
        {
            StartCoroutine(DamageProcess());
        }
    }

    IEnumerator DamageProcess()
    {
        //it quickly flashes.
        bodyRend.color = Color.black;
        yield return new WaitForSeconds(0.5f);
        bodyRend.color = originalColor;
    }

    
}
