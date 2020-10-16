using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadLevel", 5);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
}
