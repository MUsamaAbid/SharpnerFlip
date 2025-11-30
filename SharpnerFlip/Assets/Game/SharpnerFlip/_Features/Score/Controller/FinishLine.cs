using UnityEngine;
using System;

public class FinishLine : MonoBehaviour
{
    public event Action OnFinishLineCrossed;
    
    [Header("Visual Settings")]
    [SerializeField] private bool enableVisuals = true;
    
    private bool hasBeenTriggered = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered)
            return;
        
        if (other.CompareTag("Player"))
        {
            hasBeenTriggered = true;
            Debug.Log("FINISH LINE CROSSED!");
            OnFinishLineCrossed?.Invoke();
        }
    }
    
    public void Reset()
    {
        hasBeenTriggered = false;
    }
}
