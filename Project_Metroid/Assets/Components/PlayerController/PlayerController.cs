using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerHandler handler;
    [SerializeField] GameObject feet;
    bool running;
    float currentSpeed;
    [SerializeField]float walkingSpeed;
    [SerializeField]float runningSpeed;
    [SerializeField] float rollSpeed;

    int constDir = 1;
    private void Start()
    {
        handler = GetComponent<PlayerHandler>();
    }

    private void Update()
    {
        HandleMoveSound();
        if (handler.HasBlock(PlayerHandler.BlockType.Complete)) return;
        UIInput();

        if (handler.HasBlock(PlayerHandler.BlockType.Partial)) return;
        
        RunInput();
        if(!handler.HasBlock(PlayerHandler.BlockType.Interact))InteractInput();
        MoveInput();        
        AttackInput();
        DodgeInput();
        DaggerInput();
    }


    void UIInput()
    {
        if (Input.GetKeyDown(handler.GetKey("Inventory")))
        {
            handler.inventoryUI.ControlUI();
        }
        if (Input.GetKeyDown(handler.GetKey("Pause")))
        {
            Observer.instance.OnPauseUI();
        }
    }
    bool interactMoveStop;


    #region MOVEMENT INPUT
    void MoveInput()
    {
        float dir = 0;

        if (AnimationRunning("Hit"))
        {
            handler.rb.velocity = new Vector2(dir * currentSpeed, handler.rb.velocity.y);
            handler.combat.attackCooldown = false;
            
            return;
        }

        if (AnimationRunning("Attack"))
        {
            handler.rb.velocity = new Vector2(dir * currentSpeed, handler.rb.velocity.y);
            return;
        }

        if (AnimationRunning("Roll"))
        {
            return;
        }
        if (AnimationRunning("Kick"))
        {
            handler.rb.velocity = new Vector2(dir * currentSpeed, handler.rb.velocity.y);
            return;
        }

        if (Input.GetKeyUp(handler.GetKey("MoveLeft")))
        {

            interactMoveStop = false;
        }
        if (Input.GetKeyUp(handler.GetKey("MoveRight")))
        {
            interactMoveStop = false;
        }

        if (interactMoveStop)
        {
            handler.rb.velocity = new Vector2(0 * currentSpeed, handler.rb.velocity.y);
            MoveAnimation(0);
            return;
        }

        

        if (Input.GetKey(handler.GetKey("MoveLeft")))
        {
           
            if (Input.GetKey(handler.GetKey("Run")))
            {
                handler.SpendStamina(0.01f);
                handler.OnPlayerInput(false);
            }
            else
            {
                handler.OnPlayerInput(true);
            }

            dir = -1;
            constDir = -1;
        }
        if (Input.GetKey(handler.GetKey("MoveRight")))
        {
                    
            if (Input.GetKey(handler.GetKey("Run")))
            {
                handler.SpendStamina(0.01f);
                handler.OnPlayerInput(false);
            }
            else
            {
                handler.OnPlayerInput(true);
            }
            dir = 1;
            constDir = 1;
        }

        #region DETECT WALL AHEAD


        if (!CanMoveAhead((int)dir) && !GetEnemyStuck())
        {
            handler.rb.velocity = new Vector2(0, handler.rb.velocity.y);
            MoveAnimation(0);
            return;
        }

        #endregion
        handler.rb.velocity = new Vector2(dir * currentSpeed, handler.rb.velocity.y);
        MoveAnimation(dir);
        Rotate(dir);
    }


    void HandleMoveSound()
    {
        

        if (Input.GetKey(handler.GetKey("MoveRight")))
        {

            if (WallAhead(1) || EnemyWallAhead(1))
            {
                handler.ControlMoveAudio(0);
                return;
            }

            if (Input.GetKey(handler.GetKey("Run")))
            {
                //animation 
                handler.ControlMoveAudio(2);
            }
            else
            {
                handler.ControlMoveAudio(1);
            }

            return;


        }

        if (Input.GetKey(handler.GetKey("MoveLeft")))
        {
            if(WallAhead(-1) || EnemyWallAhead(-1))
            {
                handler.ControlMoveAudio(0);
                return;
            }

            if (Input.GetKey(handler.GetKey("Run")))
            {
                //animation 
                handler.ControlMoveAudio(2);
            }
            else
            {
                handler.ControlMoveAudio(1);
            }

            return;
        }

        handler.ControlMoveAudio(0);

    }

    int rollDir;
    void DodgeInput()
    {
        //if at any pont 

        if (AnimationRunning("Roll"))
        {
            //
            //maybe if you stop your roll inside the wall thing does not count.

            //if we find a wall we block any movement.
           if(WallAhead(rollDir)) handler.rb.velocity = new Vector3(0, 0, 0);
            else
            {
                //how to make the roll not get stuck in the player.

                handler.rb.velocity = new Vector2(rollDir * rollSpeed, handler.rb.velocity.y);
            }
            return;
        }

        if (AnimationRunning("Hit"))
        {
            return;
        }
        if (AnimationRunning("Attack"))
        {
            return;
        }
        if (AnimationRunning("Roll"))
        {
            return;
        }


        //roll and dash.
        if (Input.GetKeyDown(handler.GetKey("Roll")))
        {
            Roll();
            handler.OnPlayerInput(false);
        }
        if (Input.GetKeyDown(handler.GetKey("Skip")))
        {

        }

    }

    void Roll()
    {

        if (Input.GetKey(handler.GetKey("MoveLeft")))
        {
            StartCoroutine(RollProcess(-1));
            return;
        }
        if (Input.GetKey(handler.GetKey("MoveRight")))
        {
            StartCoroutine(RollProcess(1));
            return;
        }


    }

    bool rollCooldown;

    void RefreshRoll() => rollCooldown = false;

    IEnumerator RollProcess(int dir)
    {

        if (rollCooldown)
        {
            yield break;
        }

        if (!handler.CanAct(10))
        {
            //show on ui that you can take that action
            handler.Warn("No Stamina");
            yield break;
        }

        rollDir = dir;
        handler.SpendStamina(10);
        handler.anim.Play("Player_Roll");
        gameObject.tag = "Invisible";

        rollCooldown = true;

        //how is it moving?


        handler.body.GetComponent<SpriteRenderer>().color = Color.gray;
        Invoke("RefreshRoll", 1.2f);
        Invoke("RefreshImmune", 0.8f);
       
    }

    void RefreshImmune()
    {

        handler.body.GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.tag = "Player";
    }


    bool runWarnCooldown;
    void RunInput()
    {




        if (Input.GetKey(handler.GetKey("Run")))
        {
            if (handler.CanAct(0.01f))
            {
                currentSpeed = runningSpeed;
                running = true;
            }
            else
            {

                currentSpeed = walkingSpeed;
                running = false;

                if (!runWarnCooldown)
                {
                    runWarnCooldown = true;
                    Invoke("RefreshRunWarn", 3);
                    handler.Warn("No Stamina to run");
                }


            }


        }
        else
        {
            currentSpeed = walkingSpeed;
            running = false;
        }

    }
    #endregion

    bool daggerCooldown;
    void DaggerInput()
    {

        if (AnimationRunning("Attack"))
        {
            return;
        }

        if (AnimationRunning("Roll"))
        {
            return;
        }
        if (AnimationRunning("Kick"))
        {
            return;
        }
        if (AnimationRunning("Hit"))
        {
            return;
        }

        if (Input.GetKeyDown(handler.GetKey("Dagger")))
        {

            //can only throw if there are daggers in inventory.
            if (!handler.HasCertainItem("Dagger"))
            {
                return;
            }

            if (daggerCooldown) return;

            Debug.Log("Dagger thrown");
            //then consume dagger.
            daggerCooldown = true;
            Invoke("RefreshDagger", 2);
            handler.ConsumeCertainItem("Dagger");
            CreateDagger();

            //otherwise we throw a dagger towards teh direction we are facing.
        }


    }

    

    [SerializeField] GameObject daggerTemplate;
    [SerializeField] GameObject shooter;
    void CreateDagger()
    {
        //we can take from our rotation.
        GameObject newObject = Instantiate(daggerTemplate, shooter.transform.position, Quaternion.identity);
        newObject.GetComponent<DaggerProjectil>().SetUp(constDir, 5);
        
    }



    #region ANIMATION
    void MoveAnimation(float dir)
    {
        if (dir == 0)
        {
            IdleAnimation();
            return;
        }

        //there is a cooldown 

        if (running)
        {
            handler.anim.Play("Player_Run");
        }
        else
        {
            handler.anim.Play("Player_Walk");
        }     
    }

    void IdleAnimation()
    {
        if(handler.panic >= 0 && handler.panic <= 35)
        {
            handler.anim.Play("Player_Idle_1");
        }
        if (handler.panic > 35 && handler.panic <= 80)
        {
            handler.anim.Play("Player_Idle_2");
        }
        if (handler.panic > 80 && handler.panic <= 100)
        {
            handler.anim.Play("Player_Idle3");
        }
    }

    #endregion

    void Rotate(float dir)
    {
        if (dir == 0) return;
        //rotate to where you are facing.
        if(dir == 1)
        {
            handler.body.transform.localPosition = new Vector3(0, 0, 0);
            handler.body.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        if(dir == -1)
        {
            handler.body.transform.rotation = new Quaternion(0, 180, 0, 0);
            handler.body.transform.localPosition = new Vector3(-0.4f, 0, 0);
        }
    }


  

    void InteractInput()
    {
       

        if (Input.GetKeyDown(PlayerHandler.instance.GetKey("Interact")))
        {
            if (PlayerHandler.instance.interact == null) return;

            Invoke("RefreshInteractMoveStop", 0.2f);
            interactMoveStop = true;
            PlayerHandler.instance.interact.Interact();

        }

        if (Input.GetKeyDown(PlayerHandler.instance.GetKey("ChangeInteract")))
        {
           
        }

    }

  

    bool AnimationRunning(string id)
    {      
        return handler.anim.GetCurrentAnimatorStateInfo(0).IsName("Player_" + id);
    }

    void AttackInput()
    {
        if (AnimationRunning("Hit"))
        {
            return;
        }
        if (AnimationRunning("Roll"))
        {
            return;
        }

        if (AnimationRunning("Attack"))
        {
            return;
        }

        if (Input.GetKeyDown(handler.GetKey("Attack")))
        {
            handler.combat.Attack();
        }
        if (Input.GetKeyDown(handler.GetKey("AttackHeavy")))
        {
            handler.combat.AttackHeavy();
        }
        if (Input.GetKeyDown(handler.GetKey("Kick")))
        {
            handler.combat.Kick();
        }
    }


    #region REFRESH

    void RefreshInteractMoveStop()
    {
        interactMoveStop = false;
    }
    void RefreshRunWarn()
    {

    }

    void RefreshDagger() => daggerCooldown = false;
    #endregion

    int GetDir()
    {
        if(transform.localRotation.y == 180)
        {
            return -1;
        }
        if (transform.localRotation.y == 0)
        {
            return 1;
        }

        Debug.LogError("something went wrong in getting dir");
        return 0;

    }


    bool CanMoveAhead(int dir)
    {
        if (EnemyWallAhead(dir)) return false;
        if (WallAhead(dir)) return false;

        return true;

    }


    bool EnemyWallAhead(int dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(feet.transform.position, Vector2.right * dir, 0.5f, LayerMask.GetMask("Enemy"));
        return hit.collider;

    }
    bool WallAhead(int dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(feet.transform.position, Vector2.right * dir, 0.6f, LayerMask.GetMask("Wall"));
        return hit.collider;

    }

    bool GetEnemyStuck()
    {
        //if there is someone in both sides. then it must means i am inside the enemy. thats why we cant leave because both dirs are stuck.

        RaycastHit2D hitRight = Physics2D.Raycast(feet.transform.position, Vector2.right, 0.4f, LayerMask.GetMask("Enemy"));
        if (!hitRight.collider) return false;

        RaycastHit2D hitLeft = Physics2D.Raycast(feet.transform.position, Vector2.left, 0.4f, LayerMask.GetMask("Enemy"));
        if (!hitLeft.collider) return false;

        //if both hit something then it must only mean that it is stuck inside. if that happens i will allow the player to move till is no longer stuck.
        return true;

    }

    bool GetWallStuck()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(feet.transform.position, Vector2.right, 0.4f, LayerMask.GetMask("Wall"));
        if (!hitRight.collider) return false;

        RaycastHit2D hitLeft = Physics2D.Raycast(feet.transform.position, Vector2.left, 0.4f, LayerMask.GetMask("Wall"));
        if (!hitLeft.collider) return false;

        //if both hit something then it must only mean that it is stuck inside. if that happens i will allow the player to move till is no longer stuck.
        return true;
    }

}
