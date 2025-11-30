using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
public class SpiralPeel : MonoBehaviour
{
    private Rigidbody rb;
    private float lifetime;
    private float maxLifetime = 3f;
    
    public void Initialize(Color color, Vector3 force, Vector3 torque, float lifespan = 3f)
    {
        maxLifetime = lifespan;
        lifetime = 0f;
        
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.SetColor("_BaseColor", color);
            renderer.material.SetColor("_Color", color);
        }
        
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.VelocityChange);
            rb.AddTorque(torque, ForceMode.VelocityChange);
        }
    }
    
    private void Update()
    {
        lifetime += Time.deltaTime;
        
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }
}
