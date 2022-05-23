using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChunkNeighbours : MonoBehaviour
    {
        public GameObject top;
        public GameObject down;
        public GameObject left;
        public GameObject right;
        public GameObject front;
        public GameObject back;
        public ChunkManager chunkManager;
        public bool generated = false;

        public bool Generate(GameObject parent, int iterations = 3)
        {
            if (iterations > 0)
            {
                WorldGeneration parentScript = parent.GetComponent<WorldGeneration>();
                chunkManager = parentScript.chunk.neighbours.chunkManager;
                if (left == null && !chunkManager.Chunks.ContainsKey(new Vector3(parentScript.chunk.sizeX, 0, 0) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z)))
                {
                    left = Instantiate(parentScript.chunkSpawner);
                    left.transform.position += new Vector3(parentScript.chunk.sizeX, 0, 0) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z);
                    WorldGeneration worldGenerator = left.GetComponent<WorldGeneration>();
                    worldGenerator.Init();
                    worldGenerator.chunk.neighbours.right = parent;
                    worldGenerator.chunkSpawner = parentScript.chunkSpawner;
                    worldGenerator.chunk.neighbours.Generate(left, iterations - 1);
                }
                if (right == null && !chunkManager.Chunks.ContainsKey(new Vector3(-parentScript.chunk.sizeX, 0, 0) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z)))
                {
                    right = Instantiate(parentScript.chunkSpawner);
                    right.transform.position += new Vector3(-parentScript.chunk.sizeX, 0, 0) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z);
                    WorldGeneration worldGenerator = right.GetComponent<WorldGeneration>();
                    worldGenerator.Init();
                    worldGenerator.chunk.neighbours.left = parent;
                    worldGenerator.chunkSpawner = parentScript.chunkSpawner;
                    worldGenerator.chunk.neighbours.Generate(right, iterations - 1);
                }
                if (front == null && !chunkManager.Chunks.ContainsKey(new Vector3(0, 0, parentScript.chunk.sizeZ) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z)))
                {
                    front = Instantiate(parentScript.chunkSpawner);
                    front.transform.position += new Vector3(0, 0, parentScript.chunk.sizeZ) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z);
                    WorldGeneration worldGenerator = front.GetComponent<WorldGeneration>();
                    worldGenerator.Init();
                    worldGenerator.chunk.neighbours.back = parent;
                    worldGenerator.chunkSpawner = parentScript.chunkSpawner;
                    worldGenerator.chunk.neighbours.Generate(front, iterations - 1);
                }
                if (back == null && !chunkManager.Chunks.ContainsKey(new Vector3(0, 0, -parentScript.chunk.sizeZ) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z)))
                {
                    back = Instantiate(parentScript.chunkSpawner);
                    back.transform.position += new Vector3(0, 0, -parentScript.chunk.sizeZ) + new Vector3(parentScript.gameObject.transform.position.x, 0, parentScript.gameObject.transform.position.z);
                    WorldGeneration worldGenerator = back.GetComponent<WorldGeneration>();
                    worldGenerator.Init();
                    worldGenerator.chunk.neighbours.front = parent;
                    worldGenerator.chunkSpawner = parentScript.chunkSpawner;
                    worldGenerator.chunk.neighbours.Generate(back, iterations - 1);
                }
                return true;
            }
            return false;
        }

        public void setLayer(int layer)
        {
            front.layer = layer;
            back.layer = layer;
            left.layer = layer;
            right.layer = layer;
        }
    }
}
