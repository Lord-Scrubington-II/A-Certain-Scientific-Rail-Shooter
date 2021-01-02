using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
<<<<<<< Updated upstream
    [Tooltip("In Seconds")][SerializeField] float levelLoadDelay = 1f;
    [Tooltip("Explosion Prefab")][SerializeField] GameObject deathFx;
    [SerializeField] GameObject deathGibs;
    [SerializeField] GameObject jetParticles;
    private readonly float gibIntangibilityTime = 0.1f;
=======
    [Tooltip("In Seconds")] [SerializeField] private float levelLoadDelay = 1f;
    [Tooltip("Jet Particles Prefab")] [SerializeField] private GameObject jetParticles;
    [Tooltip("Explosion Prefab")] [SerializeField] private GameObject deathFx;
    [Tooltip("Gibs Prefab")] [SerializeField] private GameObject deathGibs;
    private readonly float gibIntangibilityTime = 0.3f;
>>>>>>> Stashed changes
    private readonly float explosionStrength = 10f;

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

    //boom
    /**
     * func: PlayGibs
     * Partial invocation of the visual death sequence.
     * The game instantiates a set of gibs while deactivating the player's original model,
     * then applies an impulse to each gib via its rigid body. 
     */
    private void PlayGibs()
    {
        //shut off ship's mesh renderer and collider
        MeshRenderer shipMesh = gameObject.GetComponent<MeshRenderer>();
        shipMesh.enabled = false;
        MeshCollider shipCollider = gameObject.GetComponent<MeshCollider>();
        shipCollider.enabled = false;

        //disable jet particles
        jetParticles.SetActive(false);

        //find rigid body of player ship
        Rigidbody playerRB = gameObject.GetComponent<Rigidbody>();

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
            Vector3 impulseVector = new Vector3(
                transforms[findIndex].position.x, 
                transforms[findIndex].position.y, 
                transforms[findIndex].position.z
            );
            impulseVector -= playerRB.centerOfMass;

            impulseVector *= explosionStrength;
            rigidbody.velocity += impulseVector;

            findIndex++;
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
