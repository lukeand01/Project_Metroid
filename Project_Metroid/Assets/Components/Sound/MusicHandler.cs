using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{

    public static MusicHandler instance;

    AudioSource source;
    AudioSource pausedSource; //when paused the game plays something a bit different. paused music needs to be slow to start.
    bool paused;

    [SerializeField] SoundHolderSO genericSoundHolder;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
        source = GetComponent<AudioSource>();
        Observer.instance.EventPauseUI += PauseGame;
    }
    public void StartBackgroundMusic()
    {

    }

    void PauseGame(bool force)
    {
        return;
        if (paused)
        {
            StartCoroutine(PauseProcess());    
        }
        else
        {
            StopAllCoroutines();
            paused = true;
            pausedSource.Stop();
            source.UnPause();
        }


    }

    IEnumerator PauseProcess()
    {
        yield return new WaitForSeconds(0.6f);
        paused = false;
        pausedSource.Play();
        source.Pause();
    }
   
    public void CreateGenericSfx(string id)
    {
        AudioClip clip = genericSoundHolder.GetClip(id);

        if (clip == null)
        {
            Debug.Log("no clip");
            return;
        }

        CreateSFX(clip);

    }

   public void CreateSFX(AudioClip clip)
    {
        GameObject newObject = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
        newObject.name = "SFX";
        newObject.AddComponent<SoundUnit>().PlaySound(clip);

    }



}
