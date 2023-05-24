using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    //the ui pourpuse is to relate information.
    int currentProgress;
    int targetProgress;

    

    GameObject holder;
    [SerializeField] Image progressBar;
    bool process;

    private void Start()
    {
        holder = transform.GetChild(0).gameObject;
    }

    public void ActionStart(int currentProgress, int targetProgress)
    {
        this.currentProgress = currentProgress;
        this.targetProgress = targetProgress;

        if (process)
        {
            Debug.LogError("there is an action already running");
            return;
        }

        StartCoroutine(ActionProcess());
    }

    IEnumerator ActionProcess()
    {
        process = true;
        holder.SetActive(true);
        while (targetProgress > currentProgress)
        {
            currentProgress += 1;

            progressBar.fillAmount = (float)currentProgress / (float)targetProgress;
            yield return new WaitForSeconds(0.05f);
        }
        process = false;
        progressBar.fillAmount = 1;  
        yield return new WaitForSeconds(0.08f);
        PlayerHandler.instance.OnActionCompleted();
        holder.SetActive(false);

    }

    public void ActionCancel()
    {
        process = false;
        StopAllCoroutines();
        holder.SetActive(false);
    }

    public int GetCurrentProgress()
    {
        return currentProgress;
    }
   
}
