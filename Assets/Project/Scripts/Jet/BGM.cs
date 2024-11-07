using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioClip bgmSound;     // BGM
    private static BGM instance;
    private AudioSource audioSource;

    void Awake()
       {
           if (instance == null)
           {
               instance = this;
               DontDestroyOnLoad(gameObject);
           }
           else
           {
               Destroy(gameObject);
           }
       }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (bgmSound != null)
        {
            audioSource.clip = bgmSound;
            audioSource.loop = true; // ループ再生
            audioSource.Play(); // BGM再生
        }
    }
       public void DestroyThisObject()
       {
           Destroy(gameObject);
       }

}
