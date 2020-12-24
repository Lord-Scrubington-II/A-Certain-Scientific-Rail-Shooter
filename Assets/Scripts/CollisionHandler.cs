using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [Tooltip("In Seconds")][SerializeField] float levelLoadDelay = 1f;
    [Tooltip("Explosion Prefab")][SerializeField] GameObject deathFx;
    [SerializeField] GameObject deathGibs;
    [SerializeField] GameObject jetParticles;
    private float gibIntangibilityTime = 0.1f;
    private float explosionStrength = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        StartDeathSequence();
        deathFx.SetActive(true);
        Invoke(nameof(ReloadScene), levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        print("Ship hit a trigger");
        PlayGibs(); //hehe ship go boom
        SendMessageUpwards("OnPlayerDeath");
    }

    private void PlayGibs()
    {
        //shut off ship's mesh renderer and collider
        MeshRenderer shipMesh = gameObject.GetComponentInChildren<MeshRenderer>();
        shipMesh.enabled = false;
        MeshCollider shipCollider = gameObject.GetComponent<MeshCollider>();
        shipCollider.enabled = false;

        //disable jet particles
        jetParticles.SetActive(false);

        //instantiate gibs, parented to the player ship
        GameObject gibs = Instantiate(deathGibs, gameObject.transform.position, gameObject.transform.rotation);
        gibs.transform.parent = gameObject.transform;

        //Invoke(nameof(MakeGibsTangible), gibIntangibilityTime);

        //apply explosive force to gibs
        Rigidbody[] rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        Transform[] transforms = deathGibs.GetComponentsInChildren<Transform>();
        int findIndex = 0;
        foreach(Rigidbody rigidbody in rigidbodies)
        {
            Vector3 forceVector = new Vector3(
                gameObject.transform.position.x - transforms[findIndex].position.x, 
                gameObject.transform.position.y - transforms[findIndex].position.y, 
                gameObject.transform.position.z - transforms[findIndex].position.z
            );
            forceVector *= explosionStrength;
            findIndex++;

            rigidbody.velocity += forceVector;
        }

    }

    private void MakeGibsTangible()
    {
        MeshCollider[] gibColliders = deathGibs.GetComponentsInChildren<MeshCollider>();
        foreach(MeshCollider meshCollider in gibColliders)
        {
            meshCollider.enabled = true;
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }
}
