using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyCommander : MonoBehaviour, ISaveable
{
    //this controls only gravity.
    //have to take care wave per wave.
    public List<CommanderWaveClass> commanderList = new List<CommanderWaveClass>();
    [SerializeField] DoorFade door;
    //if i want to spawn in places i need another script.


    public UnityEvent unityEvent;
    bool done;
    int current; //current wave. always reset when you reload.
    bool process;

    private void Start()
    {

    }

    private void Update()
    {
        //when all the enmies from the current wave are dead then we call the next one. 
        if (!process) return;

        if (AllDead())
        {
            Debug.Log("all of current are called");
            process = false;
            ContinueWave();
        }
        else
        {
            Debug.Log("they are not dead");
        }
    }

    void ContinueWave()
    {
        //if there is wave here.
        current++;
        if(current >= commanderList.Count)
        {
            //then we have no more waves to summon.
            Debug.Log("end of wave");
            End();
            return;
        }
      
        StartWave();

    }

    [ContextMenu("Start wave")]
    public void StartWave()
    {
        //start wave.
        //they will drop down or appear in respawn places.
        for (int i = 0; i < commanderList[current].enemyList.Count; i++)
        {
            commanderList[current].enemyList[i].FallingDown();
        }
        process = true;
    }

    bool AllDead()
    {

        if (commanderList[current].enemyList.Count <= 0)
        {
            Debug.LogError("There is nothing in the list");
            return false;
        }

        for (int i = 0; i < commanderList[current].enemyList.Count; i++)
        {
            if (!commanderList[current].enemyList[i].dead) return false;
        }

        return true;
    }


    void End()
    {
        unityEvent.Invoke();
        door.EndBoss();
    }
  
    public object CaptureState()
    {
        return new SaveData
        {
            done = done
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        done = saveData.done;
        current = 0;
    }

    [System.Serializable]
    public struct SaveData
    {
        public bool done;
    }
    //release the first wave and only when the first wave is dead do we throw the next one.





}

[System.Serializable]
public class CommanderWaveClass
{
    public List<EnemyBase> enemyList = new List<EnemyBase>();

}