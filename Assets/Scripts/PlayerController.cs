using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{   
    [Header("General")]

    [Tooltip("In ms^-1")] [SerializeField] private float xSpeed = 6f;
    [Tooltip("In ms^-1")] [SerializeField] private float ySpeed = 6f;
    [Tooltip("In metres")] [SerializeField] private float xDispMax = 5f;
    [Tooltip("In metres")] [SerializeField] private float yDispMax = 4f;
    [SerializeField] private GameObject[] guns;

    [Header("Screen Position Params")]
    [SerializeField] private float positionPitchFactor = -5f;
    [SerializeField] private float positionYawFactor = 5f;

    [Header("Control-Throw Params")]
    [SerializeField] private float controlPitchFactor = -5f;
    [SerializeField] private float controlRollFactor = -5f;


    static float xThrow, yThrow;
    static bool firing;
    private List<ParticleSystem> bullets = new List<ParticleSystem>();
    AudioSource laserSounds;

    bool controlsFrozen = false;

    // Start is called before the first frame update

    void Start()
    {
        CacheGuns();
    }

    private void CacheGuns()
    {
        foreach (GameObject gun in guns)
        {
            ParticleSystem bullet = gun.GetComponent<ParticleSystem>();
            bullets.Add(bullet);
        }

        //cache audiosource of guns
        laserSounds = gameObject.GetComponent<AudioSource>();
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

    private void HandleTransform()
    {
        float actualXTransform = CalcXTransform();
        float actualYTransform = CalcYTransform();

        transform.localPosition = new Vector3(actualXTransform, actualYTransform, transform.localPosition.z);
    }

    private float CalcYTransform()
    {
        yThrow = CrossPlatformInputManager.GetAxis("Vertical");
        float yTransformThisFrame = yThrow * ySpeed * Time.deltaTime;
        float rawYTransform = transform.localPosition.y + yTransformThisFrame;
        float actualYTransform = Mathf.Clamp(rawYTransform, -yDispMax, yDispMax);
        return actualYTransform;
    }

    private float CalcXTransform()
    {
        xThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        float xTransformThisFrame = xThrow * xSpeed * Time.deltaTime;
        float rawXTransform = transform.localPosition.x + xTransformThisFrame;
        float actualXTransform = Mathf.Clamp(rawXTransform, -xDispMax, xDispMax);
        return actualXTransform;
    }

    private void HandleGuns()
    {
        /*
        firing = CrossPlatformInputManager.GetButton("Fire");
        foreach (GameObject gun in guns)
        {
            gun.SetActive(firing);
        }   
        */
        firing = CrossPlatformInputManager.GetButton("Fire");
        SetGunsActive(firing);
    }


    private void SetGunsActive(bool firing)
    {
        foreach (ParticleSystem bullet in bullets)
        {
            var emissionModule = bullet.emission;
            emissionModule.enabled = firing;
        }

        if (firing && !laserSounds.isPlaying)
        {
            laserSounds.Play();
        }
        else if (!firing)
        {
            laserSounds.Stop();
        }
    }

    private void OnPlayerDeath()
    {
        print("Controls Frozen");
        controlsFrozen = true;
        SetGunsActive(false);
    }

}
