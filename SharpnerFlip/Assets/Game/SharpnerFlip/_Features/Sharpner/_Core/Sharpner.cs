using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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
    
    [Header("Sharpening Settings")]
    [SerializeField] private bool canSharpenOnlyWhileFlipping = true;
    [SerializeField] private float sharpeningGravityMultiplier = 0.3f;
    
    [Header("Events")]
    public UnityEvent<Collectible> OnCollectibleSharpened;
    public UnityEvent<Collectible> OnUnsharpenableHit;
    
    private Rigidbody rigidBody;
    private float targetRotation;
    private float currentRotationProgress;
    private bool isFlipping;
    private bool isSharpening;
    private Collectible currentCollectible;
    
    public bool IsFlipping => isFlipping;
    public bool IsSharpening => isSharpening;
    
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
        
        if (isSharpening)
        {
            ApplySharpeningGravity();
        }
    }
    
    void Flip()
    {
        targetRotation += 360f;
        
        if (!isFlipping)
        {
            isFlipping = true;
        }
        
        if (isSharpening)
        {
            StopSharpening();
        }
        
        Vector3 currentVelocity = rigidBody.linearVelocity;
        currentVelocity.y = 0f;
        rigidBody.linearVelocity = currentVelocity;
        
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
            
            Vector3 currentVelocity = rigidBody.linearVelocity;
            if (!isSharpening)
            {
                currentVelocity.y = 0f;
            }
            currentVelocity.z = 0f;
            rigidBody.linearVelocity = currentVelocity;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Collectible collectible = other.GetComponent<Collectible>();
        
        if (collectible != null)
        {
            HandleCollectibleTrigger(collectible);
        }
    }
    
    private void HandleCollectibleTrigger(Collectible collectible)
    {
        if (collectible.IsCollected)
        {
            return;
        }
        
        if (collectible.canBeSharpened)
        {
            collectible.MarkAsCollected();
            StartSharpening(collectible);
            Debug.Log($"Sharpening {collectible.collectibleType}!");
            OnCollectibleSharpened?.Invoke(collectible);
        }
        else
        {
            Debug.LogWarning($"Hit unsharpenable object: {collectible.collectibleType}! FAIL!");
            OnUnsharpenableHit?.Invoke(collectible);
        }
    }
    
    private void StartSharpening(Collectible collectible)
    {
        if (!isSharpening)
        {
            isSharpening = true;
            currentCollectible = collectible;
            rigidBody.useGravity = false;
            
            Vector3 currentVelocity = rigidBody.linearVelocity;
            currentVelocity.y = 0f;
            rigidBody.linearVelocity = currentVelocity;
            
            collectible.StartSharpening();
            
            Debug.Log($"SHARPENING STARTED - gravity reduced to {sharpeningGravityMultiplier}x, Y velocity reset to 0");
        }
    }
    
    private void StopSharpening()
    {
        if (isSharpening)
        {
            isSharpening = false;
            rigidBody.useGravity = true;
            
            if (currentCollectible != null)
            {
                currentCollectible.StopSharpening();
                currentCollectible = null;
            }
            
            Debug.Log("Stopped sharpening - gravity restored to normal");
        }
    }
    
    private void ApplySharpeningGravity()
    {
        Vector3 customGravity = Physics.gravity * sharpeningGravityMultiplier;
        rigidBody.AddForce(customGravity, ForceMode.Acceleration);
    }
}
