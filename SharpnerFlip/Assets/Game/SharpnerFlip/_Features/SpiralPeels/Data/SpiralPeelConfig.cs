using UnityEngine;

[CreateAssetMenu(fileName = "SpiralPeelConfig", menuName = "Configs/Spiral Peel Config")]
public class SpiralPeelConfig : ScriptableObject
{
    [Header("Peel Generation")]
    [Tooltip("The material used for rendering peels")]
    public Material peelMaterial;
    
    [Tooltip("Width of the spiral peel ribbon")]
    [Range(0.05f, 0.5f)]
    public float peelWidth = 0.15f;
    
    [Tooltip("Maximum length/radius of the spiral")]
    [Range(0.2f, 2f)]
    public float peelLength = 0.8f;
    
    [Tooltip("Vertical thickness/curl of the peel")]
    [Range(0.05f, 0.5f)]
    public float peelThickness = 0.2f;
    
    [Tooltip("Number of segments for mesh smoothness")]
    [Range(4, 32)]
    public int peelSegments = 16;
    
    [Header("Spawn Settings")]
    [Tooltip("Time between peel spawns (seconds)")]
    [Range(0.01f, 0.5f)]
    public float spawnRate = 0.08f;
    
    [Tooltip("Number of peels spawned each time")]
    [Range(1, 5)]
    public int peelsPerSpawn = 2;
    
    [Tooltip("Radius around the collectible where peels spawn")]
    [Range(0.05f, 1f)]
    public float spawnRadius = 0.2f;
    
    [Header("Physics")]
    [Tooltip("Base force applied to peels (Y is upward, magnitude is outward)")]
    public Vector3 baseForce = new Vector3(0f, 1.5f, 0.5f);
    
    [Tooltip("Random force variation")]
    [Range(0f, 5f)]
    public float forceRandomness = 1.5f;
    
    [Tooltip("Rotational force strength")]
    [Range(0f, 500f)]
    public float torqueStrength = 150f;
    
    [Tooltip("How long peels live before despawning")]
    [Range(1f, 10f)]
    public float peelLifetime = 2.5f;
    
    [Tooltip("Mass of each peel")]
    [Range(0.001f, 0.1f)]
    public float peelMass = 0.01f;
    
    [Tooltip("Linear drag on peels")]
    [Range(0f, 2f)]
    public float linearDamping = 0.5f;
    
    [Tooltip("Angular drag on peels")]
    [Range(0f, 2f)]
    public float angularDamping = 0.5f;
    
    [Header("Rainbow Colors")]
    [Tooltip("Gradient for rainbow colors")]
    public Gradient rainbowGradient;
    
    private void OnValidate()
    {
        if (rainbowGradient == null || rainbowGradient.colorKeys.Length == 0)
        {
            CreateDefaultRainbowGradient();
        }
    }
    
    private void CreateDefaultRainbowGradient()
    {
        rainbowGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[7];
        colorKeys[0] = new GradientColorKey(new Color(1f, 0f, 0f), 0f);
        colorKeys[1] = new GradientColorKey(new Color(1f, 0.5f, 0f), 0.166f);
        colorKeys[2] = new GradientColorKey(new Color(1f, 1f, 0f), 0.333f);
        colorKeys[3] = new GradientColorKey(new Color(0f, 1f, 0f), 0.5f);
        colorKeys[4] = new GradientColorKey(new Color(0f, 0.5f, 1f), 0.666f);
        colorKeys[5] = new GradientColorKey(new Color(0.5f, 0f, 1f), 0.833f);
        colorKeys[6] = new GradientColorKey(new Color(1f, 0f, 0.5f), 1f);
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);
        
        rainbowGradient.SetKeys(colorKeys, alphaKeys);
    }
}
