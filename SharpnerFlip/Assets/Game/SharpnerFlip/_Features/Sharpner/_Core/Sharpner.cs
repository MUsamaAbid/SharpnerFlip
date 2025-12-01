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
    
    [Header("Fail Detection")]
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Events")]
    public UnityEvent<Collectible> OnCollectibleSharpened;
    public UnityEvent<Collectible> OnUnsharpenableHit;
    public UnityEvent OnGroundHit;
    public UnityEvent OnGameOver;
    public UnityEvent OnSharpeningStarted;
    public UnityEvent OnSharpeningStopped;
    public UnityEvent OnGameStarted;
    
    private Rigidbody rigidBody;
    private float targetRotation;
    private float currentRotationProgress;
    private bool isFlipping;
    private bool isSharpening;
    private Collectible currentCollectible;
    private bool isGameOver;
    private bool hasGameStarted;
    
    public bool IsFlipping => isFlipping;
    public bool IsSharpening => isSharpening;
    public bool IsGameOver => isGameOver;
    public bool HasGameStarted => hasGameStarted;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        currentRotationProgress = 0f;
        hasGameStarted = false;
    }
    
    private void Start()
    {
        SetupRigidbody();
        rigidBody.useGravity = false;
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
            
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.rotation = Quaternion.Euler(defaultRotation);
        
        Debug.Log($"Sharpner Rigidbody setup: Interpolation={rigidBody.interpolation}, UseGravity={rigidBody.useGravity}");
    }
    
    private void Update()
    {
        if (isGameOver)
            return;
        
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
    
    private void StartGame()
    {
        hasGameStarted = true;
        rigidBody.useGravity = true;
        OnGameStarted?.Invoke();
        Debug.Log("Game Started!");
    }
    
    void Flip()
    {
        if (!hasGameStarted)
        {
            StartGame();
        }
        
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
        if (isGameOver)
            return;
        
        Collectible collectible = other.GetComponent<Collectible>();
        
        if (collectible != null)
        {
            HandleCollectibleTrigger(collectible);
        }
        else if (IsInLayerMask(other.gameObject.layer, groundLayer))
        {
            HandleGroundHit();
        }
    }
    
    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return ((1 << layer) & layerMask) != 0;
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
            collectible.OnFullySharpened += HandleCollectibleFullySharpened;
            StartSharpening(collectible);
            Debug.Log($"Sharpening {collectible.collectibleType}!");
        }
        else
        {
            Debug.LogWarning($"Hit unsharpenable object: {collectible.collectibleType}! FAIL!");
            OnUnsharpenableHit?.Invoke(collectible);
            TriggerGameOver($"Hit unsharpenable object: {collectible.collectibleType}");
        }
    }
    
    private void HandleCollectibleFullySharpened(Collectible collectible)
    {
        collectible.OnFullySharpened -= HandleCollectibleFullySharpened;
        OnCollectibleSharpened?.Invoke(collectible);
        Debug.Log($"Collectible {collectible.collectibleType} fully sharpened!");
    }
    
    private void HandleGroundHit()
    {
        Debug.LogWarning("FAIL! Sharpener hit the ground!");
        OnGroundHit?.Invoke();
        TriggerGameOver("Hit the ground");
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
            
            collectible.StartSharpening(transform);
            OnSharpeningStarted?.Invoke();
            
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
            
            OnSharpeningStopped?.Invoke();
            Debug.Log("Stopped sharpening - gravity restored to normal");
        }
    }
    
    private void ApplySharpeningGravity()
    {
        Vector3 customGravity = Physics.gravity * sharpeningGravityMultiplier;
        rigidBody.AddForce(customGravity, ForceMode.Acceleration);
    }
    
    private void TriggerGameOver(string reason)
    {
        if (isGameOver)
            return;
        
        isGameOver = true;
        
        StopSharpening();
        
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.isKinematic = true;
        
        Debug.LogError($"GAME OVER! Reason: {reason}");
        OnGameOver?.Invoke();
    }
}
