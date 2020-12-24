using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] internal Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        //Invoke("LoadLevel", 5);
        startButton.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
}
