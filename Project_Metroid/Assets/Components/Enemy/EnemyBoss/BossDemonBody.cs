using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDemonBody : MonoBehaviour
{
    //this script porpouse is to interact with the trgigers from animation.
    [SerializeField] Transform bodyProjectil;


    BossDemon handler;
    AttackClass projectilAttack;
    Transform[] projectilPos;
    [SerializeField]GameObject template;
    [SerializeField]GameObject projectilShooterHolder;
    List<Transform> projectilsShootersList = new List<Transform>();

    public void SetUp(BossDemon handler, AttackClass projectilAttack)
    {
        this.handler = handler;
        this.projectilAttack = projectilAttack;

        for (int i = 0; i < projectilShooterHolder.transform.childCount; i++)
        {
            projectilsShootersList.Add(projectilShooterHolder.transform.GetChild(i).transform);
        }
        
    }

    private void Start()
    {
        
        
    }

    public void ShootProjectilAround()
    {
        //shoot one wave all around.

        CreateProjectil(new Vector3(1, 0, 0), bodyProjectil);
        CreateProjectil(new Vector3(-1, 0, 0), bodyProjectil);
        CreateProjectil(new Vector3(1, 0.5f, 0), bodyProjectil);
        CreateProjectil(new Vector3(-1, 0.5f, 0), bodyProjectil);
    }

    public bool isRain; //it is currently rainning
    int requiredTurns = 3;
    int currentTurns = 0;

    public void ShootRain()
    {
        if(currentTurns >= requiredTurns)
        {
            Debug.Log("rain stopped");
            isRain = false;
            currentTurns = 0;
        }
        else
        {
            isRain = true;
            //then we choose a type of rain process.
            int random = UnityEngine.Random.Range(0,3);

            if(random == 0)
            {
                StartCoroutine(LeftRainProcess());
            }
            if(random == 1)
            {
                StartCoroutine(RightRainProcess());
            }
            if(random == 2)
            {
                StartCoroutine(RandomRainProcess());
            }
            if(random == 3)
            {
                Debug.LogError("problem with rain");
            }

        }

             
    }
    IEnumerator LeftRainProcess()
    {

        //get the first in the list and goes down shooting at random spacincs.
        int index = 0;
        float timer = UnityEngine.Random.Range(0.6f, 1.1f);
        for (int i = 0; i < projectilsShootersList.Count; i++)
        {
            int random = UnityEngine.Random.Range(0, 3);
            index += random;

            if(index > projectilsShootersList.Count - 1)
            {
                //then we are not interested in doing it anymore.

                continue;
            }

            CreateProjectil(new Vector3(0, -1, 0), projectilsShootersList[index]);
            index++;
            yield return new WaitForSeconds(timer);

        }


        currentTurns += 1;
        ShootRain();
    }
    IEnumerator RightRainProcess()
    {
        //get the last in the list and goes up shooting at random spacincs.

        int index = projectilsShootersList.Count - 1;
        float timer = UnityEngine.Random.Range(0.4f, 0.8f);
        for (int i = projectilsShootersList.Count; i > 0; i--)
        {
            int random = UnityEngine.Random.Range(0, 2);
            index -= random;

            if (index < 0)
            {
                //then we are not interested in doing it anymore.
                continue;
            }

            CreateProjectil(new Vector3(0, -1, 0), projectilsShootersList[index]);
            index--;
            yield return new WaitForSeconds(timer);

        }

        currentTurns += 1;
        ShootRain();
    }

    IEnumerator RandomRainProcess()
    {
        //get random transforms. never the same 

        float timer = UnityEngine.Random.Range(0.4f, 0.8f);
        int turns = UnityEngine.Random.Range(5, 10);
        int oldUsed = -1;
        Transform selectedTranform = null;

        for (int i = 0; i < turns; i++)
        {

            while (selectedTranform == null)
            {
                int random = UnityEngine.Random.Range(0, projectilsShootersList.Count);
                if(oldUsed == random)
                {
                    continue;
                }
                oldUsed = random;
                selectedTranform = projectilsShootersList[random];

            }

            CreateProjectil(new Vector3(0, -1, 0), selectedTranform);
            selectedTranform = null;
            yield return new WaitForSeconds(timer);

        }

        currentTurns += 1;
        ShootRain();


    }


    void CreateProjectil(Vector3 dir, Transform transform)
    {
        GameObject newObject = Instantiate(template.gameObject, transform.position, Quaternion.identity);
        newObject.SetActive(true);
        newObject.GetComponent<ProjectilDemon>().SetUp(dir, 5, projectilAttack.damage);

    }
}
