using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    //takes to a scene.
    //may be locked. can be opened by key or lockpick


    protected GameObject interactionHolder;
    [SerializeField] DoorType doorType;
    public enum DoorType
    {
        Free,
        Key,
        Picklock,
        MultipleKeys
    }

    bool opened;
    public Door targetDoor;
    [ConditionalField(nameof(doorType), false, DoorType.Key)] public int keyId;
    [Separator("MULTIPLE KEYS")]
    public List<int> multipleKeyId = new List<int>();

    private void Start()
    {
        interactionHolder = transform.GetChild(0).gameObject;

        if (doorType == DoorType.Free) opened = true;
    }

    public void Interact()
    {
        HandleInteraction();
       

    }

    public virtual void HandleInteraction()
    {
        if (opened)
        {
            //just open it right away.
            Open();
            return;
        }



        if (doorType == DoorType.Key)
        {
            //just need a key.
            if (PlayerHandler.instance.HasKey(keyId))
            {
                opened = true;
                Open();
            }
            else
            {
                PlayerHandler.instance.Warn("Needs a key");
            }

        }
        if (doorType == DoorType.Picklock)
        {
            //need an item called needle. spend after some action.

        }
        if (doorType == DoorType.MultipleKeys)
        {
            //then we check if you have every single key

            if (HaveEveryKey())
            {
                opened = true;
                Open();
            }
            else
            {
                if (multipleWarnCooldown) return;
                StartCoroutine(MultipleWarnProcess());
                multipleWarnCooldown = true;
                Invoke("RefreshWarnCooldown", 1);
            }
        }
    }


    bool multipleWarnCooldown;
    IEnumerator MultipleWarnProcess()
    {
        PlayerHandler.instance.Warn("Needs The Red Key");
        yield return new WaitForSeconds(1f);
        PlayerHandler.instance.Warn("Needs The Dark Key");
    }
    void RefreshWarnCooldown() => multipleWarnCooldown = false;

    bool HaveEveryKey()
    {
        if (multipleKeyId.Count <= 0) return false;

        for (int i = 0; i < multipleKeyId.Count; i++)
        {
            if (!PlayerHandler.instance.HasKey(multipleKeyId[i]))
            {
                return false;
            }
        }

        return true;
    }

   public virtual void Open()
    {
        //go to the next scene.


        if (targetDoor == null)
        {
            Debug.Log("there is no target door");
            return;
        }

        if(targetDoor.GetComponent<DoorMerchant>() != null)
        {
            targetDoor.GetComponent<DoorMerchant>().SetUpNewDoor(this);
        }

        GameHandler.instance.EnterDoor(targetDoor);
    }

    protected bool dontShowUI;
    public void InteractUI(bool choice)
    {
        if (dontShowUI)
        {
            interactionHolder.SetActive(false);
            return;
        }
        interactionHolder.SetActive(choice);
    }
}
