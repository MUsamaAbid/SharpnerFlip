using UnityEngine;

public class SpiralPeelSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private SpiralPeelConfig config;
    
    private bool isSpawning;
    private float spawnTimer;
    private Mesh peelMesh;
    private Transform peelsParent;
    private float colorOffset;
    private Transform spawnTransform;
    
    private void Awake()
    {
        if (config == null)
        {
            Debug.LogError("SpiralPeelSpawner: No config assigned! Assign a SpiralPeelConfig in the Inspector.");
            return;
        }
        
        peelMesh = SpiralPeelMeshGenerator.GenerateSpiralPeelMesh(
            config.peelWidth, 
            config.peelLength, 
            config.peelThickness, 
            config.peelSegments
        );
        
        GameObject parent = new GameObject("Peels");
        peelsParent = parent.transform;
        peelsParent.SetParent(transform);
    }
    
    private void Update()
    {
        if (!isSpawning || config == null)
            return;
        
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= config.spawnRate)
        {
            SpawnPeels();
            spawnTimer = 0f;
        }
    }
    
    public void StartSpawning(Transform sharpenerTransform)
    {
        isSpawning = true;
        spawnTimer = 0f;
        colorOffset = Random.value;
        spawnTransform = sharpenerTransform;
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        spawnTransform = null;
    }
    
    private void SpawnPeels()
    {
        for (int i = 0; i < config.peelsPerSpawn; i++)
        {
            SpawnSinglePeel();
            colorOffset += 0.05f;
            if (colorOffset > 1f)
                colorOffset -= 1f;
        }
    }
    
    private void SpawnSinglePeel()
    {
        if (config.peelMaterial == null)
        {
            Debug.LogError("SpiralPeelSpawner: No material assigned in config! Assign one in the SpiralPeelConfig.");
            return;
        }
        
        if (spawnTransform == null)
        {
            Debug.LogWarning("SpiralPeelSpawner: No spawn transform set! Peels will spawn from collectible.");
            return;
        }
        
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 radialDirection = new Vector3(
            Mathf.Cos(randomAngle), 
            0f, 
            Mathf.Sin(randomAngle)
        );
        
        Vector3 spawnPosition = spawnTransform.position + radialDirection * config.spawnRadius;
        
        GameObject peelObject = new GameObject("SpiralPeel");
        peelObject.transform.position = spawnPosition;
        
        Quaternion rotation = Quaternion.Euler(
            Random.Range(-30f, 30f),
            Random.Range(0f, 360f),
            Random.Range(-30f, 30f)
        );
        peelObject.transform.rotation = rotation;
        peelObject.transform.SetParent(peelsParent);
        
        MeshFilter meshFilter = peelObject.AddComponent<MeshFilter>();
        meshFilter.mesh = peelMesh;
        
        MeshRenderer meshRenderer = peelObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(config.peelMaterial);
        
        Rigidbody rb = peelObject.AddComponent<Rigidbody>();
        rb.mass = config.peelMass;
        rb.linearDamping = config.linearDamping;
        rb.angularDamping = config.angularDamping;
        
        SpiralPeel peel = peelObject.AddComponent<SpiralPeel>();
        
        Vector3 force = radialDirection * config.baseForce.magnitude + 
                       Vector3.up * config.baseForce.y + 
                       Random.insideUnitSphere * config.forceRandomness;
        Vector3 torque = Random.insideUnitSphere * config.torqueStrength;
        Color peelColor = config.rainbowGradient.Evaluate(colorOffset);
        
        peel.Initialize(peelColor, force, torque, config.peelLifetime);
    }
    
    private void OnDestroy()
    {
        if (peelMesh != null)
        {
            Destroy(peelMesh);
        }
    }
}
