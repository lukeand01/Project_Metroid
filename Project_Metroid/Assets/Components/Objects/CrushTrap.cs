using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushTrap : MonoBehaviour
{
    //it goes down and up in certain times.

    [SerializeField] Sword sword;
    [SerializeField] float distanceVariable;
    [SerializeField] float speed;
    [SerializeField] float timerUp;
    [SerializeField] float timerDown;

    private void Start()
    {
        sword.SetUp("Player", 999, 3);

       

        StartCoroutine(DownProcess());

    }

    IEnumerator DownProcess()
    {
        //wait timer.
        //shake a bit
        StartCoroutine(ShakeProcess(timerDown * 0.5f));
        yield return new WaitForSeconds(timerDown);

        Vector3 vec = new Vector3(transform.position.x, transform.position.y - distanceVariable);
        transform.DOMove(vec, speed);
        yield return new WaitForSeconds(3);
        StartCoroutine(UpProcess());
    }

    IEnumerator ShakeProcess(float timer)
    {
        yield return new WaitForSeconds(timer);
        //shake to all sides.

        for (int i = 0; i < 10; i++)
        {
            

            transform.localPosition += new Vector3(0.09f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            transform.localPosition -= new Vector3(0.09f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }


    }

    IEnumerator UpProcess()
    {
        StartCoroutine(ShakeProcess(timerUp * 0.5f));
        yield return new WaitForSeconds(timerUp);

        Vector3 vec = new Vector3(transform.position.x, transform.position.y + distanceVariable);
        transform.DOMove(vec, speed * 1.5f);
        yield return new WaitForSeconds(3);
        StartCoroutine(DownProcess());

    }

}
