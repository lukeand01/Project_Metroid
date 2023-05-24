using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundUnit : MonoBehaviour
{

    AudioSource source;

    public void PlaySound(AudioClip audio)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.loop = false;
        source.clip = audio;
        source.volume = 0.4f;
        source.Play();
        Invoke("DestroyOrder", audio.length + 0.5f);
        //then the sound will be adjusted based in gamehandler information.
    }

    void DestroyOrder()
    {

        Destroy(gameObject);
    }

}
