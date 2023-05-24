using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject upInteractHolder;
    [SerializeField] GameObject downInteractHolder;

    [SerializeField] Transform downPos;
    [SerializeField] Transform upPos;

    GameObject stepsHolder;
    bool isClimbing;
    GameObject player;
    List<GameObject> stepList = new List<GameObject>();


    bool climbing;
    private void Start()
    {
        player = PlayerHandler.instance.gameObject;

       
    }

    public void Interact()
    {

        //start climbing. if you are hit you fall down.


        if (CloserToTop())
        {
            //you are closer to the last piece. which means the top ladder.
            upInteractHolder.SetActive(false);

            GameHandler.instance.UseLadder(downPos);
        }
        else
        {

            downInteractHolder.SetActive(false);
            GameHandler.instance.UseLadder(upPos);
        }
       
    }

    //we just fade to black. climbing sound. then to the other place.



    bool CloserToTop()
    {
        float firstDistance = Vector3.Distance(downPos.transform.position, player.transform.position);
        float lastDistance = Vector3.Distance(upPos.transform.position, player.transform.position);


        return firstDistance > lastDistance;
    }

    #region OldClimbing
    IEnumerator ClimbDown()
    {
        PlayerHandler.instance.untargetable = true;
        //we take each step begging by the first in the list.
        climbing = true;
        PlayerHandler.instance.AddBlock("Climb", PlayerHandler.BlockType.Partial);
        for (int i = stepList.Count - 1; i > 0; i--)
        {
            PlayerHandler.instance.transform.position = stepList[i].transform.position;
            yield return new WaitForSeconds(0.3f);
        }

        PlayerHandler.instance.RemoveBlock("Climb");
        // PlayerHandler.instance.ControlGravity(false);

        climbing = false;
        PlayerHandler.instance.untargetable = false;
    }

    //i need to handle gravity. gravity always stop when its feet touch the ground.

    IEnumerator ClimbUp()
    {
        PlayerHandler.instance.untargetable = true;
        //we take each step begging by the first in the list.
        climbing = true;
        PlayerHandler.instance.AddBlock("Climb", PlayerHandler.BlockType.Partial);
        //have to lock position.
        for (int i = 0; i < stepList.Count; i++)
        {
            PlayerHandler.instance.transform.position = stepList[i].transform.position;
            yield return new WaitForSeconds(0.7f);
        }

       // PlayerHandler.instance.ControlGravity(true);
        
        //yield return new WaitForSeconds(0.3f);
        PlayerHandler.instance.RemoveBlock("Climb");
       // PlayerHandler.instance.ControlGravity(false);

        climbing = false;
        PlayerHandler.instance.untargetable = false;
    }

    #endregion

    public void InteractUI(bool choice)
    {
        if (climbing)
        {
            upInteractHolder.SetActive(false);
            downInteractHolder.SetActive(false);
            return;
        }

        if (CloserToTop())
        {
            upInteractHolder.SetActive(choice);
        }
        else
        {
            downInteractHolder.SetActive(choice);
        }

        //interactHolder.SetActive(choice);
    }

   
}
