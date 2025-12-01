using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float gameOverDuration = 0.5f;
    [SerializeField] private float gameOverMagnitude = 0.4f;
    [SerializeField] private float groundHitDuration = 0.3f;
    [SerializeField] private float groundHitMagnitude = 0.25f;
    [SerializeField] private float collectibleHitDuration = 0.2f;
    [SerializeField] private float collectibleHitMagnitude = 0.15f;
    
    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine currentShake;
    
    public Vector3 ShakeOffset => shakeOffset;
    
    public void ShakeOnGameOver()
    {
        TriggerShake(gameOverDuration, gameOverMagnitude);
    }
    
    public void ShakeOnGroundHit()
    {
        TriggerShake(groundHitDuration, groundHitMagnitude);
    }
    
    public void ShakeOnCollectibleHit()
    {
        TriggerShake(collectibleHitDuration, collectibleHitMagnitude);
    }
    
    public void TriggerShake(float duration, float magnitude)
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }
        
        currentShake = StartCoroutine(Shake(duration, magnitude));
    }
    
    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float dampening = 1f - (elapsed / duration);
            float currentMagnitude = magnitude * dampening;
            
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;
            
            shakeOffset = new Vector3(x, y, 0f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        shakeOffset = Vector3.zero;
        currentShake = null;
    }
}
