using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [Tooltip("In Seconds")][SerializeField] float levelLoadDelay = 1f;
    [Tooltip("Explosion Prefab")][SerializeField] GameObject deathFx;
    private void OnTriggerEnter(Collider other)
    {
        StartDeathSequence();
        deathFx.SetActive(true);
        Invoke(nameof(ReloadScene), levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        print("Ship hit a trigger");
        SendMessage("OnPlayerDeath");
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }
}
