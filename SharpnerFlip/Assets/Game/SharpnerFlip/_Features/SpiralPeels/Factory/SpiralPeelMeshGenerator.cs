using UnityEngine;

public static class SpiralPeelMeshGenerator
{
    public static Mesh GenerateSpiralPeelMesh(float width, float length, float thickness, int segments)
    {
        Mesh mesh = new Mesh();
        mesh.name = "SpiralPeel";
        
        int vertexCount = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[segments * 6];
        
        float rotations = 2.5f;
        float totalAngle = rotations * 360f;
        
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = t * totalAngle * Mathf.Deg2Rad;
            
            float radius = t * length;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            
            float normalAngle = angle + Mathf.PI * 0.5f;
            float normalX = Mathf.Cos(normalAngle);
            float normalZ = Mathf.Sin(normalAngle);
            
            float yOffset = t * thickness;
            
            vertices[i * 2] = new Vector3(x, yOffset, z);
            vertices[i * 2 + 1] = new Vector3(
                x + normalX * width, 
                yOffset, 
                z + normalZ * width
            );
            
            uvs[i * 2] = new Vector2(0f, t);
            uvs[i * 2 + 1] = new Vector2(1f, t);
        }
        
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = i * 2;
            int triangleIndex = i * 6;
            
            triangles[triangleIndex] = baseIndex;
            triangles[triangleIndex + 1] = baseIndex + 2;
            triangles[triangleIndex + 2] = baseIndex + 1;
            
            triangles[triangleIndex + 3] = baseIndex + 1;
            triangles[triangleIndex + 4] = baseIndex + 2;
            triangles[triangleIndex + 5] = baseIndex + 3;
        }
        
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }
}
