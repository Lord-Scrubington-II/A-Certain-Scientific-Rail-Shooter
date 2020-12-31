//===============================================================================
//A Certain Scientific Rails Shooter                    @Author: Zane Wang
//-------------------------------------------------------------------------------
//File: CollisionHandler.cs 
//
//Description: This file contains the implementation of a monobehaviour that is 
//          attached to the player's ship. This allows for the detection and 
//          resolution of collision events between the player's avatar, the
//          environment, and enemies.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollisionHandler : MonoBehaviour
{
    [Tooltip("In Seconds")] [SerializeField] private readonly float levelLoadDelay = 1f;
    [Tooltip("Jet Particles Prefab")] [SerializeField] private GameObject jetParticles;
    [Tooltip("Explosion Prefab")] [SerializeField] private GameObject deathFx;
    [Tooltip("Gibs Prefab")] [SerializeField] private GameObject deathGibs;
    private readonly float gibIntangibilityTime = 0.3f;
    private readonly float explosionStrength = 10f;

    [SerializeField] private AudioClip hitSound;
    private AudioSource playerAuS;
    [SerializeField] private float hitSoundVolume;

    [Range(0f, 3f)] [SerializeField] private float playerIntangibilityTime = 1.5f;
    private readonly float invincibilityFlashTime = 0.2f;
    internal static bool isInvincible;
    private int health;
    private Slider healthBar;

    // Start is called before the first frame update
    private void Start()
    {
        healthBar = GameObject.FindObjectOfType<Slider>();
        playerAuS = gameObject.GetComponent<AudioSource>();
        health = (int)healthBar.maxValue;
    }

    //When the player enters a trigger, decrement HP and check for death
    private void OnTriggerEnter(Collider other)
    {
        //if invincible, do nothing
        if (!isInvincible)
        {
            //damage to player
            health--;
            healthBar.value--;
            if (health <= 0)
            {
                //death operations
                deathFx.SetActive(true);
                Invoke(nameof(ReloadScene), levelLoadDelay);
                StartDeathSequence();
            }
            else
            {
                //begin invincibility frames
                StartCoroutine(playInvincibilityFrames());
            }
        }
    }

    /**
     * func: playInvincibilityFrames (Coroutine)
     * Plays the invincibility frame sequence over a period specified by
     * the player's hit-invulnerabilty time. Also signals other components
     * that the player is in the invulnerable state.
     */
    private IEnumerator playInvincibilityFrames()
    {
        SendMessage("OnPlayerHit");

        isInvincible = true;

        /**
         * To play hit sound, inject new audio clip object to the player's audio source,
         * while preserving the old audio clip as a reference.
         * This takes advantage of the fact that the player is not able to fire their
         * guns while invincible, and eliminates the need for multiple audiosources.
         */
        AudioClip oldAudioClip = playerAuS.clip;
        float oldVolume = playerAuS.volume;
        playerAuS.clip = hitSound;
        playerAuS.volume = 0.5f;
        playerAuS.Play();

        //turn on and off the ship's renderable components over time
        MeshRenderer shipMesh = gameObject.GetComponent<MeshRenderer>();
        for (float t = 0.0f; t <= playerIntangibilityTime; t += invincibilityFlashTime)
        {
            shipMesh.enabled = !shipMesh.enabled;
            jetParticles.SetActive(!jetParticles.activeInHierarchy);
            yield return new WaitForSeconds(invincibilityFlashTime);
        }

        //re-inject original audio clip, end invincibility
        playerAuS.clip = oldAudioClip;
        playerAuS.volume = oldVolume;
        jetParticles.SetActive(true);
        shipMesh.enabled = true;
        isInvincible = false;

        SendMessage("OnInvincibilityFramesEnd");
    }

    /**
     * func: StartDeathSequence()
     * Called when the player's HP reaches 0 on a collision event.
     */
    private void StartDeathSequence()
    {
        print("Ship hit a trigger");
        PlayGibs(); //hehe ship go boom
        SendMessageUpwards("OnPlayerDeath");
    }

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

        //instantiate gibs, child to the player ship
        GameObject gibs = Instantiate(deathGibs, gameObject.transform.position, gameObject.transform.rotation);
        gibs.transform.parent = gameObject.transform;

        StartCoroutine(MakeGibsTangible());

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
            rigidbody.velocity += impulseVector; //boom

            findIndex++;
        }
    }

    /**
     * func: MakeGibsTangible (Coroutine)
     * This prevents gibs from being immediately blown out of 
     * camera FOV when colliding with surfaces perpendicular to
     * the player rig's line of sight.
     */
    private IEnumerator MakeGibsTangible()
    {
        MeshCollider[] gibColliders = deathGibs.GetComponentsInChildren<MeshCollider>();
        foreach(MeshCollider meshCollider in gibColliders)
        {
            meshCollider.enabled = false;
        }

        yield return new WaitForSeconds(gibIntangibilityTime);

        foreach (MeshCollider meshCollider in gibColliders)
        {
            meshCollider.enabled = true;
        }

        yield return null;

    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }
}
