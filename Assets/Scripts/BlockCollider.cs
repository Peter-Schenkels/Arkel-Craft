using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class BlockCollider : MonoBehaviour
{
    public List<Vector3> vertices;
    public List<int> triangles;
    Mesh mesh;
    MeshCollider meshCollider;

    public BlockCollider(List<Vector3> vertices, List<int> triangles)
    {
        vertices = vertices;
        triangles = triangles;
    }

    private void Start()
    {
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }


}