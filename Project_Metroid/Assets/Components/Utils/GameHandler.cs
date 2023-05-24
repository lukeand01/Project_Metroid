using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameHandler : MonoBehaviour
{
    //game handler. fade in and out and change positions. load stuff as well. help set the game and check stats.

    public static GameHandler instance;

     

    [SerializeField] UnityEngine.UI.Image fadeBackground;
    [Separator("Death")]
    [SerializeField] UnityEngine.UI.Image deathBackground;
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] UnityEngine.UI.Image respawnButton;
    [SerializeField] UnityEngine.UI.Image quitGameButton;
    [Separator("END")]
    [SerializeField] UnityEngine.UI.Image endGameBackground;
    [SerializeField] TextMeshProUGUI endGameText;

    private void Awake()
    {
        instance = this;
    }

    #region LOADING SCENES

    //we need to create a load screen otherwise the things wont work.



    public void ChangeScene()
    {

        //we go to the next scene

        //order the loading screen to start.
        LoadingScreen.instance.ToGame();

       
    }

   

    public void ReturnToMenu()
    {
        

        LoadingScreen.instance.ToMenu();
    }

    #endregion

    #region PROCESS
    public void EnterDoor(Door door)
    {
        StartCoroutine(EnterDoorProcess(door));
    }
    IEnumerator EnterDoorProcess(Door door)
    {
        //fade in and out while commands are blocked.
        //appear in the other door.

        //cannot be attacked.
        GameObject playerObject = PlayerHandler.instance.gameObject;
        playerObject.tag = "Invisible";
        PlayerHandler.instance.AddBlock("Transition", PlayerHandler.BlockType.Complete);
        fadeBackground.gameObject.SetActive(true);
        //fade out 
        for (int i = 0; i < 100; i++)
        {

            fadeBackground.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1f);
        playerObject.transform.position = door.transform.position;

        //fade in
        for (int i = 0; i < 100; i++)
        {

            fadeBackground.color -= new Color(0, 0, 0, 0.01f);
            if(i == 65)
            {
                playerObject.tag = "Player";
                PlayerHandler.instance.RemoveBlock("Transition");
                
            }
            yield return new WaitForSeconds(0.01f);
        }
        fadeBackground.gameObject.SetActive(false);

    }

    public void UseLadder(Transform pos)
    {
        Debug.Log("use ladder");
        StartCoroutine(UseLadderProcess(pos));
    }

    IEnumerator UseLadderProcess(Transform pos)
    {

        GameObject playerObject = PlayerHandler.instance.gameObject;
        playerObject.tag = "Invisible";
        PlayerHandler.instance.AddBlock("Transition", PlayerHandler.BlockType.Complete);
        fadeBackground.gameObject.SetActive(true);
        MusicHandler.instance.CreateGenericSfx("Ladder");
        for (int i = 0; i < 100; i++)
        {

            fadeBackground.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.5f);
        playerObject.transform.position = pos.position;

        for (int i = 0; i < 100; i++)
        {

            fadeBackground.color -= new Color(0, 0, 0, 0.01f);
            if(i == 65)
            {
                playerObject.tag = "Player";
                PlayerHandler.instance.RemoveBlock("Transition");
            }
            yield return new WaitForSeconds(0.01f);
        }

        fadeBackground.gameObject.SetActive(false);
    }

    public void Death()
    {



        StartCoroutine(DeathPanelProcess());
    }

    IEnumerator DeathPanelProcess()
    {
        PlayerHandler.instance.MouseVisible(true);
        deathBackground.gameObject.SetActive(true);
        deathText.gameObject.SetActive(true);

        deathBackground.color = new Color(deathBackground.color.r, deathBackground.color.g, deathBackground.color.b, 0);
        deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 0);

        //fade in
        while (deathBackground.color.a < 1)
        {
            deathBackground.color += new Color(0, 0, 0, 0.01f);
            deathText.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(1);

        //make both buttons fade in.

        respawnButton.gameObject.SetActive(true);
        quitGameButton.gameObject.SetActive(true);

        respawnButton.color = new Color(respawnButton.color.r, respawnButton.color.g, respawnButton.color.b, 0);
        quitGameButton.color = new Color(quitGameButton.color.r, quitGameButton.color.g, quitGameButton.color.b, 0);

        while (respawnButton.color.a < 1)
        {
            respawnButton.color += new Color(0, 0, 0, 0.01f);
            quitGameButton.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

    }

    public void Load()
    {
        Debug.Log("load gamehandler");
        StartCoroutine(LoadProcess());
    }

    IEnumerator LoadProcess()
    {
        //also close the thing
        PlayerHandler.instance.AddBlock("Load", PlayerHandler.BlockType.Complete);

        fadeBackground.gameObject.SetActive(true);
        fadeBackground.color = new Color(deathBackground.color.r, deathBackground.color.g, deathBackground.color.b, 0);

        while (fadeBackground.color.a < 1)
        {
            fadeBackground.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
        Observer.instance.OnPauseUI(true);
        SaveHandler.instance.Load(SaveSlots.third.ToString());

        while (fadeBackground.color.a > 0)
        {
            fadeBackground.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        PlayerHandler.instance.RemoveBlock("Load");
        Debug.Log("got to the end of the save");
    }


    public void Respawn()
    {
        //we will load the game.
        //then reset all enemies

        Observer.instance.OnResetEnemy();
        Observer.instance.OnPauseUI(true);
        SaveHandler.instance.Load(SaveSlots.third.ToString());

        PlayerHandler.instance.RespawnPlayer();
        //
        StartCoroutine(RespawnPanelProcess());

    }
    IEnumerator RespawnPanelProcess()
    {
        respawnButton.gameObject.SetActive(false);
        quitGameButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        //fade out.
        while (deathBackground.color.a > 0)
        {
            deathBackground.color -= new Color(0, 0, 0, 0.01f);
            deathText.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.03f);
        }

        deathBackground.gameObject.SetActive(false);
        deathText.gameObject.SetActive(false);

        PlayerHandler.instance.RemoveBlock("Death");
    }
    #endregion

    #region END GAME
    public void EndGame()
    {
        PlayerHandler.instance.AddBlock("EndGame", PlayerHandler.BlockType.Complete);

        //call the credits stuff.
        //then take it
        StartCoroutine(EndGameProcess());
    }

    IEnumerator EndGameProcess()
    {
        endGameBackground.gameObject.SetActive(true);
        endGameBackground.color = new Color(endGameBackground.color.r, endGameBackground.color.g, endGameBackground.color.b, 0);

        endGameText.gameObject.SetActive(true);
        endGameText.color = new Color(endGameText.color.r, endGameText.color.g, endGameText.color.b, 0);

        for (int i = 0; i < 100; i++)
        {
            endGameBackground.color += new Color(0, 0, 0, 0.01f);
            endGameText.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        //show some credits.
        yield return new WaitForSeconds(2);

        endGameText.text = "Thank you for playing this short game.";

        yield return new WaitForSeconds(3);
        ReturnToMenu();

        

        endGameBackground.gameObject.SetActive(false);

        

    }

    #endregion

    //i need a door for handling the boss.
}

public enum SaveSlots
{
    first,
    second,
    third
}
