using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [Tooltip("In ms^-1")] [SerializeField] float xSpeed = 6f;
    [Tooltip("In ms^-1")] [SerializeField] float ySpeed = 6f;
    [Tooltip("In metres")] [SerializeField] float xDispMax = 5f;
    [Tooltip("In metres")] [SerializeField] float yDispMax = 4f;
    [SerializeField] float positionPitchFactor = -5f;
    [SerializeField] float controlPitchFactor = -5f;
    [SerializeField] float controlRollFactor = -5f;
    [SerializeField] float positionYawFactor = 5f;
    static float xThrow, yThrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleTransform();
        HandleRotation();
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
}
