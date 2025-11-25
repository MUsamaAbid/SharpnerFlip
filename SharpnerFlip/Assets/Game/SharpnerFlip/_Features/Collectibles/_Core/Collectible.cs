using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Info")]
    public CollectibleType collectibleType = CollectibleType.Pencil;
    
    [Header("Properties")]
    public bool canBeSharpened = true;
}
