using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorFade : Door
{

    //this only responds to the boss and the doorboss.
    //

    SpriteRenderer rend;
    [SerializeField] BossBase boss;
    [SerializeField] UnityEvent unityEvent;
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    //i reset the boss through the same save that i use to restore the player state.
    public void StartBoss()
    {
        //when this happens we just make it 
        dontShowUI = true;
        interactionHolder.SetActive(false);

        //the door disappears. 
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0);

        if (boss != null)
        {
            boss.StartBoss();
            boss.EventBossDefeated += EndBoss;
            PlayerHandler.instance.EventPlayerDead += ResetDoor;
        }
        if (unityEvent != null) unityEvent.Invoke();
    }

    public void EndBoss()
    {
        //we show it again.
        boss.EventBossDefeated -= EndBoss;
        dontShowUI = false;
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 1);
    }

    void ResetDoor()
    {
        PlayerHandler.instance.EventPlayerDead -= ResetDoor;
    }

}
