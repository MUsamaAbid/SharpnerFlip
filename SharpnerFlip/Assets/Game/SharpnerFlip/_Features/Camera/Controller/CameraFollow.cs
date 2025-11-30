using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = true;
    
    [Header("Smooth Follow")]
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float smoothSpeed = 15f;
    
    private Vector3 offset;
    
    private void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
            Debug.Log($"Camera offset from target: {offset}");
            
            Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
            if (targetRigidbody != null && targetRigidbody.interpolation == RigidbodyInterpolation.None)
            {
                Debug.LogError("CAMERA JITTER: Target Rigidbody must have Interpolation set to 'Interpolate'!");
            }
        }
    }
    
    private void LateUpdate()
    {
        if (target == null)
            return;
        
        Vector3 desiredPosition = target.position + offset;
        
        Vector3 currentPos = transform.position;
        if (!followX) desiredPosition.x = currentPos.x;
        if (!followY) desiredPosition.y = currentPos.y;
        if (!followZ) desiredPosition.z = currentPos.z;
        
        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(currentPos, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = desiredPosition;
        }
    }
}
