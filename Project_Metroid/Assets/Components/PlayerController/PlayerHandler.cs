using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerHandler : MonoBehaviour, ISaveable
{
    public static PlayerHandler instance;

    public SoundHolderSO playerSoundHolder;

    [Separator("COMPONENTS")]
    public InventoryUI inventoryUI;
    public ActionUI actionUI;
    public PlayerHUD hud;
    public WarnUI warn;
    PlayerInventory inventory;
    PlayerController controller;
    PlayerConsumption consumption;
    PlayerHS hs;
    [HideInInspector] public PlayerCombat combat;

    [Separator("BODY COMPONENTS")]
    [HideInInspector] public Rigidbody2D rb;
    [SerializeField]public GameObject body;
    [HideInInspector]public Animator anim;
    public Sword sword;

    [Separator("SOUND")]
    [SerializeField] AudioSource runAudio;
    [SerializeField] AudioSource walkAudio;

    [Separator("VALUES")]
    public float panic;
    public bool untargetable;

    public void ChangeTargetability(bool choice) => untargetable = choice;

    public void ControlGravity(bool choice)
    {
        if (choice)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    //i want to activate gravity and take it out when the player touches the ground.

    #region KEYCODES
    KeyCode keyMoveLeft;
    KeyCode keyMoveRight;
    KeyCode keyInventory;
    KeyCode keyPause;
    KeyCode keyInteract;
    KeyCode keyChangeInteract;
    KeyCode keyRun;
    KeyCode keyAttack;
    KeyCode keyRoll;
    KeyCode keySkip;
    KeyCode keyKick;
    KeyCode keyDagger;

    public KeyCode GetKey(string id)
    {
        switch (id)
        {
            case "MoveLeft":
                return keyMoveLeft;
            case "MoveRight":
                return keyMoveRight;
            case "Inventory":
                return keyInventory;
            case "Pause":
                return keyPause;
            case "Interact":
                return keyInteract;
            case "ChangeInteract":
                return keyChangeInteract;
            case "Run":
                return keyRun;
            case "Attack":
                return keyAttack;
            case "Roll":
                return keyRoll;
            case "Skip":
                return keySkip;
            case "Kick":
                return keyKick;
            case "Dagger":
                return keyDagger;
        }



        return KeyCode.None;
    }

    void ChangeKey(string id, KeyCode key)
    {

    }

    void SetUpKeys()
    {
        keyMoveLeft = KeyCode.A;
        keyMoveRight = KeyCode.D;
        keyRun = KeyCode.LeftShift;

        keyInventory = KeyCode.Tab;
        keyPause = KeyCode.Escape;

        keyInteract = KeyCode.E;

        keyAttack = KeyCode.Q;

        keyRoll = KeyCode.R;

        keyKick = KeyCode.G;
        keyDagger = KeyCode.F;
    }
    #endregion

    #region BLOCKS

    //get blocks to stop movement.
    public Dictionary<string, BlockType> blockNary = new Dictionary<string, BlockType>();

    public void AddBlock(string id, BlockType block)
    {
        if (blockNary.ContainsKey(id))
        {
            //if we already have that key then we dont.
            return;
        }
        blockNary.Add(id, block);
    }
    public bool HasBlock(BlockType block)
    {
        return blockNary.ContainsValue(block);
    }
    public void RemoveBlock(string id)
    {
        if (blockNary.ContainsKey(id))
        {
            blockNary.Remove(id);
        }
    }

    public enum BlockType
    {
        Complete,
        Partial,
        Mouse,
        Movement,
        Input,
        Interact

    }

    #endregion

    #region EVENTS
    public event Action<bool> EventPlayerInput; //movement, attack, also when take damage.
    public void OnPlayerInput(bool easyInput) => EventPlayerInput?.Invoke(easyInput);


    public event Action<bool> EventPlayerDamaged; //when is attacked
    public void OnDamaged(bool isPassive) => EventPlayerDamaged?.Invoke(isPassive);


    public event Action EventPlayerDead; //when is dead
    public void OnPlayerDead() => EventPlayerDead?.Invoke();


    public event Action EventActionCompleted; //when an action is completed.
    public void OnActionCompleted() => EventActionCompleted?.Invoke();


    public event Action EventActionCancelled; //when an action is cancelled.
    public void OnActionCancelled() => EventActionCancelled?.Invoke();

    void SetUpEvents()
    {
        EventPlayerInput += PlayerInput;
        EventPlayerDamaged += PlayerReceivedDamage;
    }

    public void PlayerInput(bool easyInput)
    {
        //player has pressed some kind of input. movement, attack.
        //easy input is walking.


        if (!easyInput)
        {
            CancelItemConsumption();
        }

    }
    public void PlayerReceivedDamage(bool passiveDamage)
    {
        //in case its an attack or poision


        CancelItemConsumption();
    }
    #endregion

    public void RespawnPlayer()
    {

        untargetable = false;
        gameObject.tag = "Player";
        RemoveBlock("Death");
        hs.dead = false;
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        anim = body.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        inventory = GetComponent<PlayerInventory>();
        inventory.SetUp(this);
        controller = GetComponent<PlayerController>();
        consumption = GetComponent<PlayerConsumption>();
        hs = GetComponent<PlayerHS>();
        combat = GetComponent<PlayerCombat>();

        SetUpKeys();
        SetUpEvents();
        //MouseVisible(false);
        
    }


    public void MouseVisible(bool choice)
    {
        //the only thing we do here we show the mouse or not.
        Cursor.visible = choice;
        
    }

    public void ControlMoveAudio(int choice)
    {
        if(choice == 0)
        {
            runAudio.enabled = false;
            walkAudio.enabled = false;
        }

        if(choice == 1)
        {
            runAudio.enabled = false;
            walkAudio.enabled = true;
        }

        if(choice == 2)
        {
            runAudio.enabled = true;
            walkAudio.enabled = false;
        }

    }



    #region ACTION

    public void ActionStart(int currentProgress, int targetProcess) => actionUI.ActionStart(currentProgress, targetProcess);

    public void ActionCancel() => actionUI.ActionCancel();

    public int ActionProgress() => actionUI.GetCurrentProgress();



    #endregion

    #region INVENTORY
    public void ReceiveItem(ItemClass item) => inventory.ReceiveItem(item);
    public bool CanAdd(ItemClass item) => inventory.CanAdd(item);

    public bool HasCertainItem(string itemName) => inventory.HasCertainItem(itemName);

    public void ConsumeCertainItem(string itemName) => inventory.ConsumeCertainItem(itemName);

    public void SwapItens(List<ItemClass> itemList) => inventory.SwapItens(itemList);
    #endregion

    #region INTERACTION
    //if you ever get in a collider of two interacts you can choose either.

    public IInteractable interact;
    void AddInteract(IInteractable interact)
    {
        this.interact = interact;
        this.interact.InteractUI(true);
    }
    void RemoveInteract()
    {
        if (interact == null) return;
        interact.InteractUI(false);
        interact = null;
    }

    public int GetSoul() => inventory.souls;
    public void ClearSoul() => inventory.souls = 0;
    #endregion

    #region CONSUMABLES


    //there can be only one 
    //consumables.
    ItemClass itemBeingConsumed;
    public void StartConsuming(ItemClass newItem)
    {
        //we start consuming an item
        if(itemBeingConsumed != null)
        {
            //stop courotine.

        }
        itemBeingConsumed = newItem;
        //we lower the walking speed.
        //can walk but if you run, attack or interact with something. it stops.
        actionUI.ActionStart(0, (int)newItem.data.useDuration);
    }

    void CancelItemConsumption()
    {
        if (itemBeingConsumed == null) return;
        OnActionCancelled();
        actionUI.ActionCancel();

    }

    //there will be stamina system.
    public void Consume(ItemClass item)
    {
        List<ConsumableClass> consumableList = item.data.consumableList.consumableList;

        if(consumableList.Count == 0)
        {
            Debug.LogError("Something went wrong wiht consume items " + item.data.itemName);
            return;
        }

        for (int i = 0; i < consumableList.Count; i++)
        {
            ConsumableClass consumable = new ConsumableClass(consumableList[i].consumableType, consumableList[i].value, consumableList[i].duration, consumableList[i].exception);
            consumption.HandleConsumption(consumable);                    
        }

    }

    public void ApplyDirectBD(ConsumableClass consumable)
    {
        ConsumableClass newConsumable = new ConsumableClass(consumable.consumableType, consumable.value, consumable.duration, consumable.exception);
        consumption.HandleConsumption(newConsumable);
    }

    public void TemporaryItens(ConsumableClass consumable) => consumption.AddTempItem(consumable);



    #endregion


    #region HEALTH AND STAMINA 
    public void SpendStamina(float value) => hs.SpendStamina(value);
    public bool CanAct(float value) => hs.CanAct(value);

    public void RecoverHealth(float value) => hs.RecoverHealth(value);


    public void UpdateStaminaBonus(float staminaBonus) => hs.UpdateStaminaBonus(staminaBonus);
    public void UpdateHealthBonus(float healthBonus) => hs.UpdateHealthBonus(healthBonus);
    public void UpdateStaminaRecovery(float staminaRecoveryBonus) => hs.UpdateStaminaRecoveryBonus(staminaRecoveryBonus);


    #endregion

    #region SWORD

    public float GetSwordState() => sword.swordState;
    public void RepairSword() => sword.RepairSword();

    #endregion


    bool hasKnife;

    public void GainKnife()
    {
        //just sets something true here;
        //now from one you will be able to use knife skill
        hasKnife = true;
        Observer.instance.OnGainedKnife();

    }


    public bool HasKey(int key) => inventory.HasKey(key);
    public void GainKey(int key) => inventory.AddKey(key);

    public void GainSoul(int soul)
    {
        inventory.GainSoul(soul);
        Warn($"Gained {soul} soul", true);
    }

    public void Warn(string warn, bool gainWarn = false) => this.warn.CreateWarning(warn, gainWarn);

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyCorpse corpse = collision.GetComponent<EnemyCorpse>();
        if(corpse != null && hasKnife)
        {
            //use the warn text to describe what you got            
            inventory.AddFlesh(corpse.fleshList);
            Destroy(collision.gameObject);
        }

        IInteractable interact = collision.gameObject.GetComponent<IInteractable>(); 
        if (interact != null)
        {
            AddInteract(interact);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interact = collision.gameObject.GetComponent<IInteractable>();
        if (interact != null)
        {

            RemoveInteract();

        }
    }


    #region SAVE SYSTEM
    
    public object CaptureState()
    {

        return new SaveData
        {
            posX = transform.position.x,
            posY = transform.position.y,

            hasKnife = hasKnife,
        };

    }

    public void RestoreState(object state)
    {

        Debug.Log("restore state");
        var saveData = (SaveData)state;


        //thats why i need to close it till it is all saved.


        anim.Play("Player_Idle_1");

        transform.position = new Vector3(saveData.posX, saveData.posY, 0);
        MouseVisible(false);

        hasKnife = saveData.hasKnife;
        if (hasKnife) Observer.instance.OnGainedKnife();

    }

    //save weapon durability.
    //souls, itens, buffs,
    [System.Serializable]
    struct SaveData
    {
        public float posX;
        public float posY;

        public bool hasKnife;


    }
    
    #endregion
}
