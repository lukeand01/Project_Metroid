using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    //dont destroy on load.
    //show fade.
    //show "animation" of loading.
    //

    public static LoadingScreen instance;
    GameObject holder;
    [SerializeField] Image fadeObject;
    [SerializeField] TextMeshProUGUI loadingText;

    private void Awake()
    {
        instance = this;
        holder = transform.GetChild(0).gameObject;
        DontDestroyOnLoad(gameObject);
    }

    //we only move to the next when the current scene is removed.

    AsyncOperation unLoadingScene;
    AsyncOperation loadingScene;


    public void ToMenu()
    {
        //nothing here to worry.
        StartCoroutine(LowerCurtain(0, 1));
    }


    public void ToGame()
    {
        //we will just wait 0.1 so the game can load properly.
        StartCoroutine(LowerCurtain(1, 0));
    }

    IEnumerator LowerCurtain(int loadingSceneIndex, int unloadingSceneIndex)
    {
        holder.SetActive(true);
        fadeObject.color = new Color(fadeObject.color.r, fadeObject.color.g, fadeObject.color.b, 0);
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 0);
        //after we are done we continue.
        for (int i = 0; i < 100; i++)
        {
            fadeObject.color += new Color(0, 0, 0, 0.01f);
            loadingText.color += new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.03f);
        }

        unLoadingScene = SceneManager.UnloadSceneAsync(unloadingSceneIndex);
        loadingScene = SceneManager.LoadSceneAsync(loadingSceneIndex);

        StartCoroutine(LoadGame());

    }

    IEnumerator LoadGame()
    {
        //after all the pieces are in place we tell them to load the game.
        if (loadingScene == null) yield break;

        while (!loadingScene.isDone)
        {
            yield return null;
        }

        if(PlayerHandler.instance != null)
        {
            //we block it.
            PlayerHandler.instance.AddBlock("Load", PlayerHandler.BlockType.Complete);
        }

        if(SaveHandler.instance != null)
        {
            if (SaveHandler.instance.FileExists(SaveSlots.third.ToString()))
            {
                SaveHandler.instance.Load(SaveSlots.third.ToString());
            }
        }

        StartCoroutine(RaiseCurtain());
        
    }

    IEnumerator RaiseCurtain()
    {
        //if there is player we allow movement.

        for (int i = 0; i < 100; i++)
        {

            fadeObject.color -= new Color(0, 0, 0, 0.01f);
            loadingText.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }

        holder.SetActive(false);

        if (PlayerHandler.instance != null)
        {
            //we block it.
            PlayerHandler.instance.RemoveBlock("Load");
        }

    }

}
