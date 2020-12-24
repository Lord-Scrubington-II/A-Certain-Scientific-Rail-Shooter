using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip titleTheme;
    [SerializeField] AudioClip gameTheme;
    private AudioSource musicPlayer;

    private void Awake()
    {
        musicPlayer = gameObject.GetComponent<AudioSource>();
        ChooseMusic(SceneManager.GetActiveScene().buildIndex);
        musicPlayer.Play();

        /*
        //singleton time
        int musicPlayers = FindObjectsOfType<MusicPlayer>().Length;
        if (musicPlayers > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        */
    }

    private void ChooseMusic(int scene)
    {
        if (scene == 0)
        {
            musicPlayer.clip = titleTheme;
        }
        else
        {
            musicPlayer.clip = gameTheme;
        }
    }
}

