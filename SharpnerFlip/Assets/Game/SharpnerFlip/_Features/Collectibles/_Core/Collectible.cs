using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Info")]
    public CollectibleType collectibleType = CollectibleType.Pencil;
    
    [Header("Properties")]
    public bool canBeSharpened = true;
    
    private bool isCollected = false;
    
    public bool IsCollected => isCollected;
    
    public void MarkAsCollected()
    {
        isCollected = true;
    }
}
