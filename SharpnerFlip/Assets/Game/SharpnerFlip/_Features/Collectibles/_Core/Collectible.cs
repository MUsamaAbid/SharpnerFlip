using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Info")]
    public CollectibleType collectibleType = CollectibleType.Pencil;
    
    [Header("Properties")]
    public bool canBeSharpened = true;
    
    private bool isCollected = false;
    private bool _isBeingSharpened = false;
    private float sharpeningSpeed;
    private Vector3 originalScale;
    
    public bool IsCollected => isCollected;
    public bool IsBeingSharpened => _isBeingSharpened;

    public void Init(float sharpeningSpeed)
    {
        originalScale = transform.localScale;
        this.sharpeningSpeed = sharpeningSpeed;
    }
    
    public void MarkAsCollected()
    {
        isCollected = true;
    }
    
    public void StartSharpening()
    {
        if (!_isBeingSharpened && canBeSharpened)
        {
            _isBeingSharpened = true;
            Debug.Log($"{collectibleType} - Sharpening effect started");
        }
    }
    
    public void StopSharpening()
    {
        if (_isBeingSharpened)
        {
            _isBeingSharpened = false;
            Debug.Log($"{collectibleType} - Sharpening effect stopped");
        }
    }
    
    private void Update()
    {
        if (_isBeingSharpened)
        {
            ApplySharpeningEffect();
        }
    }
    
    private void ApplySharpeningEffect()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.y -= sharpeningSpeed * Time.deltaTime;
        
        if (currentScale.y <= 0f)
        {
            currentScale.y = 0f;
            _isBeingSharpened = false;
            Destroy(gameObject);
        }
        
        transform.localScale = currentScale;
    }
}
