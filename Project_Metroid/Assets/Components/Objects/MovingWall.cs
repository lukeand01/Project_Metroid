using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    //it slowly moves to one side. deals damage to the player if it touches the player.]
    //has to save position.
    //has to lock the door after leaving.

    [SerializeField] float distanceVariable;
    [SerializeField] float speedVariable;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Sword>().SetUp("Player", 100);
        }

    }

    public void StartMovingWall()
    {
        StartCoroutine(StartMovingWallProcess());
    }

    IEnumerator StartMovingWallProcess()
    {
        //sound
        yield return new WaitForSeconds(2);
        Vector3 vec = new Vector3(transform.position.x + distanceVariable, transform.position.y, 0);
        transform.DOMove(vec, speedVariable);
    }
}
