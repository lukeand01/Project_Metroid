using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public static Observer instance;

    private void Awake()
    {
        instance = this;
    }

    //public event Action<int> EventCraftPotion;
    //public void OnCraftPotion(int empty) => EventCraftPotion?.Invoke(empty);

    public event Action<bool> EventPauseUI;
    public void OnPauseUI(bool force = false) => EventPauseUI?.Invoke(force);


    public event Action EventPlayerDied;
    public void OnPlayerDied() => EventPlayerDied?.Invoke();

    #region INVENTORY

    public event Action EventUIInventory;
    public void OnUIInventory() => EventUIInventory?.Invoke();

    public event Action<List<ItemClass>> EventUpdateInventory;
    public void OnUpdateInventory(List<ItemClass> itemList) => EventUpdateInventory?.Invoke(itemList);

    public event Action<List<ItemClass>, Chest> EventOpenChest;
    public void OnOpenChest(List<ItemClass> itemList, Chest chest) => EventOpenChest?.Invoke(itemList, chest);

    #endregion

    #region INVENTORT STUFF
    public event Action<float> EventUpdateSwordState;
    public void OnUpdateSwordState(float value) => EventUpdateSwordState?.Invoke(value);

    public event Action EventGainedKnife;
    public void OnGainedKnife() => EventGainedKnife?.Invoke(); //show knife

    public event Action<int> EventGainedKey;
    public void OnGainedKey(int id) => EventGainedKey?.Invoke(id);

    public event Action EventAssignedSkill;
    public void OnAssignedSkill() => EventAssignedSkill?.Invoke();

    public event Action<int> EventUpdateSoul;
    public void OnUpdateSoul(int value) => EventUpdateSoul?.Invoke(value);

    #endregion

    #region INTERACT


    #endregion

    public event Action<Vector3, string, bool> EventShowDescription;
    public void OnShowDescription(Vector3 pos, string description, bool hasTimer) => EventShowDescription?.Invoke(pos, description, hasTimer);

    public event Action<int, int> EventShowTimer;
    public void OnShowTimer(int current, int max) => EventShowTimer?.Invoke(current, max);


    public event Action<List<string>> EventStartDialogue;
    public void OnStartDialogue(List<string> dialogueList) => EventStartDialogue?.Invoke(dialogueList);


    #region NOTE


    public event Action<bool, string, List<string>> EventShowNote;
    public void OnShowNote(bool order = false, string title = "", List<string> contentList = null) => EventShowNote?.Invoke(order, title, contentList);

    #endregion


    #region ENEMIES

    public event Action EventResetEnemy;
    public void OnResetEnemy() => EventResetEnemy?.Invoke();


    #endregion
}
