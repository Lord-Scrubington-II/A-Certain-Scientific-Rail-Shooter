using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{   
    [Header("General")]
    [Tooltip("In ms^-1")] [SerializeField] private readonly float xSpeed = 6f;
    [Tooltip("In ms^-1")] [SerializeField] private readonly float ySpeed = 6f;
    [Tooltip("In metres")] [SerializeField] private readonly float xDispMax = 5f;
    [Tooltip("In metres")] [SerializeField] private readonly float yDispMax = 4f;
    [SerializeField] private GameObject[] guns;

    [Header("Screen Position Params")]
    [SerializeField] private readonly float positionPitchFactor = -5f;
    [SerializeField] private readonly float positionYawFactor = 5f;

    [Header("Control-Throw Params")]
    [SerializeField] private readonly float controlPitchFactor = -5f;
    [SerializeField] private readonly float controlRollFactor = -5f;

    internal static float xThrow, yThrow;
    internal static bool firing;
    private List<ParticleSystem> bullets = new List<ParticleSystem>();
    private AudioSource laserSounds;

    bool controlsFrozen = false;

    // Start is called before the first frame update
    void Start()
    {
        CacheGuns();
    }

    /**
     * func: CacheGuns
     * Caches the gun fx as instance vars to avoid calling
     * "GetComponent()" in the Update() function.
     */
    private void CacheGuns()
    {

        //cache particle systems
        foreach (GameObject gun in guns)
        {
            ParticleSystem bullet = gun.GetComponent<ParticleSystem>();
            bullets.Add(bullet);
        }

        //cache audiosource of guns
        laserSounds = gameObject.GetComponent<AudioSource>();
        laserSounds.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlsFrozen)
        {
            HandleTransform();
            HandleRotation();
            HandleGuns();
        }
        
    }

    /**
     * func: HandleRotation()
     * 
     * Rotates the player ship model to respond to translations with
     * some modicum of avionic realism. Additionally, the ship's yaw
     * is used to provide the player a larger set of firing angles
     * to accomodate the large camera FOV.
     */
    private void HandleRotation()
    {
        float pitch, yaw, roll;

        //Calc Pitch
        float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
        float pitchDueToControlThrow = yThrow * controlPitchFactor;
        pitch = pitchDueToControlThrow + pitchDueToPosition;

        //Calc Yaw
        yaw = transform.localPosition.x * positionYawFactor;

        //Calc Roll
        roll = xThrow * controlRollFactor;

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }

    /**
     * func: HandleTransform()
     * 
     * Moves the ship across the screen in accordance with signals from
     * the cross-platform input manager.
     */
    private void HandleTransform()
    {
        float actualXTransform = CalcXTransform();
        float actualYTransform = CalcYTransform();

        //always handle translation relative to the camera's line of sight
        transform.localPosition = new Vector3(actualXTransform, actualYTransform, transform.localPosition.z);
    }

    private float CalcYTransform()
    {
        //get control throw
        yThrow = CrossPlatformInputManager.GetAxis("Vertical");

        //translate control throw to y-transform with framerate independence
        float yTransformThisFrame = yThrow * ySpeed * Time.deltaTime;
        float rawYTransform = transform.localPosition.y + yTransformThisFrame;

        //stop the player at the edge of the screen
        float actualYTransform = Mathf.Clamp(rawYTransform, -yDispMax, yDispMax);
        return actualYTransform;
    }

    private float CalcXTransform()
    {
        //get control throw
        xThrow = CrossPlatformInputManager.GetAxis("Horizontal");

        //translate control throw to y-transform with framerate independence
        float xTransformThisFrame = xThrow * xSpeed * Time.deltaTime;
        float rawXTransform = transform.localPosition.x + xTransformThisFrame;

        //stop the player at the edge of the screen
        float actualXTransform = Mathf.Clamp(rawXTransform, -xDispMax, xDispMax);
        return actualXTransform;
    }

    /**
     * func: HandleGuns()
     * 
     * Fires the player ship's guns when given instruction from the CPIM.
     */
    private void HandleGuns()
    {
        /*
        firing = CrossPlatformInputManager.GetButton("Fire");
        foreach (GameObject gun in guns)
        {
            gun.SetActive(firing);
        }   
        */
        if (!CollisionHandler.isInvincible)
        {
            firing = CrossPlatformInputManager.GetButton("Fire");
            SetGunsActiveTo(firing);
        }
        else
        {
            SetGunsActiveTo(false);
        }
    }

    /**
     * func: SetGunsActiveTo()
     * 
     * The player's laser guns are actually a pair of particle systems that are
     * controlled via their emission modules. This function toggles them when called
     * and also plays the firing noises.
     * 
     * @param firing: The state of the guns.
     */
    private void SetGunsActiveTo(bool firing)
    {
        foreach (ParticleSystem bullet in bullets)
        {
            var emissionModule = bullet.emission;
            emissionModule.enabled = firing;
        }

        //play firing sounds
        if (firing && !laserSounds.isPlaying)
        {
            laserSounds.Play();
        }
        else if (!firing)
        {
            laserSounds.Stop();
        }
    }

    private void OnPlayerHit()
    {
        print("Player hit, controls temporarily frozen");
        controlsFrozen = true;
        SetGunsActiveTo(false);
    }

    private void OnInvincibilityFramesEnd()
    {
        print("Invincibility frames over. Control resumed");
        controlsFrozen = false;
    }

    private void OnPlayerDeath()
    {
        print("Player died, controls frozen");
        controlsFrozen = true;
        SetGunsActiveTo(false);
    }

}
