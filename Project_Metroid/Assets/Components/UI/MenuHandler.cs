using System;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] GameObject pauseHolder;
    [SerializeField] GameObject menuPartsHolder;
    [SerializeField] ConfirmationPanel confirmationPanel;
    [SerializeField] GameObject continueButton;



    #region EVENTS
    public event Action EventConfirm; 
    public void OnConfirm() => EventConfirm?.Invoke();

    public event Action EventCancel;
    public void OnCancel() => EventCancel?.Invoke();
    #endregion

    private void Start()
    {
       if(pauseHolder != null) Observer.instance.EventPauseUI += HandlePause;

       if(confirmationPanel != null) confirmationPanel.SetUp(this);

        HasContinueButton();

    }

    void HasContinueButton()
    {
        if (continueButton == null) return;

        if (SaveHandler.instance.FileExists(SaveSlots.third.ToString()))
        {
            //if it has then we make it active.
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }

    public void LoadLastSave()
    {
        //make the fade and everything.

        Debug.Log("got here");
        if(!SaveHandler.instance.FileExists(SaveSlots.third.ToString()))
        {
            Debug.Log("yo");
            return;
        }
        HandlePause(true);
        GameHandler.instance.Load();
       
    }


    void HandlePause(bool force = false)
    {

        if (pauseHolder == null) return;

        if (force)
        {
            Cursor.visible = true;
           
            pauseHolder.SetActive(false);
            Time.timeScale = 1;
            PlayerHandler.instance.RemoveBlock("Pause");
            return;
        }


        if (pauseHolder.activeInHierarchy)
        {
            Cursor.visible = false;
            pauseHolder.SetActive(false);
            Time.timeScale = 1;
            PlayerHandler.instance.RemoveBlock("Pause");
        }
        else
        {
            Cursor.visible = true;
            pauseHolder.SetActive(true);
            Time.timeScale = 0;
            PlayerHandler.instance.AddBlock("Pause", PlayerHandler.BlockType.Partial);
        }
    }

    public void OpenPart(GameObject target)
    {
        for (int i = 0; i < menuPartsHolder.transform.childCount; i++)
        {
            menuPartsHolder.transform.GetChild(i).gameObject.SetActive(false);
        }
        target.SetActive(true);
    }


    //we ask confirmation for it and for save and load.


    public void Continue()
    {
        Debug.Log("continue button");
        HandlePause();
    }


    #region MAINMENU
    public void StartNewGame()
    {
        //delete if there is a current file.
        if (SaveHandler.instance.FileExists(SaveSlots.third.ToString()))
        {
            //then we ask for permission
            SetupConfirmStartNewGame();

        }
        else
        {
            //then we simply go foward.
            GameHandler.instance.ChangeScene();
        }
        
    }

    void SetupConfirmStartNewGame()
    {
        EventConfirm += ConfirmStartGame;
        EventCancel += CancelStartGame;

        confirmationPanel.gameObject.SetActive(true);
        confirmationPanel.CreateConfirm("if you start a new game the old save will be destryoed. do you still want to continue");
    }

    void ConfirmStartGame()
    {

        EventConfirm -= ConfirmStartGame;
        EventCancel -= CancelStartGame;

        confirmationPanel.gameObject.SetActive(false);
        //and we delete the file.
        SaveHandler.instance.DeleteFiles();
        GameHandler.instance.ChangeScene();
    }
    void CancelStartGame()
    {
        EventConfirm -= ConfirmStartGame;
        EventCancel -= CancelStartGame;

        //just close the thing.
        confirmationPanel.gameObject.SetActive(false);
    }

    


    public void LoadGame()
    {
        //loda the other scene. then we load it.
        if (!SaveHandler.instance.FileExists(SaveSlots.third.ToString()))
        {
            Debug.LogError("Something wrong happened");
        }
        GameHandler.instance.ChangeScene();


    }



    #endregion


    #region QUIT FROM GAME
    public void QuitGame()
    {
        //quit to desktop
        //ask for permission.
        EventConfirm += ConfirmQuitGame;
        EventCancel += CancelQuitGame;
        Debug.Log("quit");

        confirmationPanel.gameObject.SetActive(true);
        confirmationPanel.CreateConfirm("If you quit the game your unsaved progress will be lost. are you sure you want to leave?");
    }
    void ConfirmQuitGame()
    {
        EventConfirm -= ConfirmQuitGame;
        EventCancel -= CancelQuitGame;

        Debug.Log("quit game");
        HandlePause(true);
        confirmationPanel.gameObject.SetActive(false);
        Application.Quit();

    }
    void CancelQuitGame()
    {
        EventConfirm -= ConfirmQuitGame;
        EventCancel -= CancelQuitGame;
        Debug.Log("cancel game");
        confirmationPanel.gameObject.SetActive(false);

    }
    #endregion

    #region QUIT TO MENU
    public void QuitMenu()
    {
        //quit back to the menu.
        //ask for permission.
        EventConfirm += ConfirmQuitMenu;
        EventCancel += CancelQuitMenu;
        

        confirmationPanel.gameObject.SetActive(true);
        confirmationPanel.CreateConfirm("If you quit to main menu your unsaved progress will be lost. are you sure you want to leave?");
    }

    void ConfirmQuitMenu()
    {
        EventConfirm -= ConfirmQuitMenu;
        EventCancel -= CancelQuitMenu;

        confirmationPanel.gameObject.SetActive(false);

        if (pauseHolder != null)
        {
            Debug.Log("there is a pause holder");
            HandlePause(true);
        }

       
        //go back to menu.
        GameHandler.instance.ReturnToMenu();
    }

    void CancelQuitMenu()
    {
        EventConfirm -= ConfirmQuitMenu;
        EventCancel -= CancelQuitMenu;

        //just close the thing.
        confirmationPanel.gameObject.SetActive(false);
    }
    #endregion



}
