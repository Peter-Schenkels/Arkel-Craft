using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    public class WorldGeneration : MonoBehaviour
    {
        public Mesh mesh;
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        public Chunk chunk;
        public GameObject chunkSpawner;
        public ChunkManager chunkManager;

        List<Vector3> vertices;
        List<int> triangles;
        List<Vector2> uvs;

        public void Init()
        {
            mesh = new Mesh();
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();
            chunk = new Chunk(this.transform.position, this.gameObject);
            meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            meshCollider.convex = false;
            CreateShape();
        }

        void Start()
        {
            if (mesh == null)
            {
                this.Init();
            }
        }

        void clear()
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();
        }

        public void AddBlock(Vector3 position)
        {
            position -= gameObject.transform.position;
            ByteVector3 converted = new ByteVector3((byte)position.x, (byte)position.y, (byte)position.z);
            BlockData newBlock = new BlockData(converted, BlockType.STONE, new ByteVector3(0,0,0));
            chunk.AddBlock(newBlock);
            clear();
            CreateShape();
        }

        public void DestroyBlock(Vector3 position)
        {
            position -= gameObject.transform.position;
            ByteVector3 converted = new ByteVector3((byte)position.x, (byte)position.y, (byte)position.z);
            chunk.DestroyBlock(converted);
            clear();
            CreateShape();
        }

        void CreateShape()
        {
            meshCollider.convex = true;
            chunk.AddChunkToMesh(vertices, triangles, uvs);
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            meshCollider.convex = false;
        }
    }
}