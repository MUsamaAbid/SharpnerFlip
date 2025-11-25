using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Sharpner : MonoBehaviour
{
    [Header("Flip Settings")]
    [SerializeField] private Vector3 defaultRotation = new Vector3(0f, 0f, -90f);
    [SerializeField] private Vector3 flippingDirection = new Vector3(0f, 0f, 1f);
    [SerializeField] private float flipSpeed = 720f;
    
    [Header("Jump Settings")]
    [SerializeField] private float forwardForce = 5f;
    [SerializeField] private float upwardForce = 8f;
    
    private Rigidbody rigidBody;
    private float targetRotation;
    private float currentRotationProgress;
    private bool isFlipping;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        currentRotationProgress = 0f;
    }
    
    private void Start()
    {
        SetupRigidbody();
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying && rigidBody != null)
        {
            SetupRigidbody();
        }
    }
    
    private void SetupRigidbody()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();
            
        rigidBody.useGravity = true;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.rotation = Quaternion.Euler(defaultRotation);
        
        Debug.Log($"Sharpner Rigidbody setup: Interpolation={rigidBody.interpolation}, UseGravity={rigidBody.useGravity}");
    }
    
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Flip();
        }
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Flip();
        }
        
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Flip();
        }
    }
    
    private void FixedUpdate()
    {
        if (isFlipping)
        {
            UpdateFlip();
        }
    }
    
    void Flip()
    {
        targetRotation += 360f;
        
        if (!isFlipping)
        {
            isFlipping = true;
        }
        
        Vector3 jumpForce = new Vector3(0f, upwardForce, forwardForce);
        rigidBody.AddForce(jumpForce, ForceMode.Impulse);
    }
    
    private void UpdateFlip()
    {
        float rotationStep = flipSpeed * Time.fixedDeltaTime;
        currentRotationProgress += rotationStep;
        
        float normalizedDirection = flippingDirection.normalized.z;
        float rotationAmount = rotationStep * normalizedDirection;
        
        Quaternion currentRotation = rigidBody.rotation;
        Quaternion deltaRotation = Quaternion.Euler(0f, 0f, rotationAmount);
        rigidBody.MoveRotation(currentRotation * deltaRotation);
        
        if (currentRotationProgress >= targetRotation)
        {
            rigidBody.rotation = Quaternion.Euler(defaultRotation);
            currentRotationProgress = 0f;
            targetRotation = 0f;
            isFlipping = false;
        }
    }
}
