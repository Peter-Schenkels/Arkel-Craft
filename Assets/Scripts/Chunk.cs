using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Chunk
    {
        public bool playerInChunk = false;
        bool changed = true;
        Vector3 position;
        GameObject parent;

        public ChunkNeighbours neighbours;

        public int sizeX = 16;
        public int sizeY = 16;
        public int sizeZ = 16;

        public Dictionary<ByteVector3, BlockData> blocks;

        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uvs;

        public void Generate()
        {
            blocks = new Dictionary<ByteVector3, BlockData>();
            int airBorder = 8;
            int stoneBorder = 6;
            for (byte x = 0; x < sizeX; x++)
            {
                for (byte y = 0; y < sizeY; y++)
                {
                    for (byte z = 0; z < sizeZ; z++)
                    {
                        if (y < stoneBorder)
                        {
                            blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.STONE));
                        }
                        else if (y < airBorder)
                        {
                            blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.DIRT));
                        }
                        else if (y == airBorder)
                        {
                            blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.GRASS));
                        }

                    }
                }
            }
        }

        public Chunk(Vector3 position, GameObject parent)
        {
            this.position = position;
            this.parent = parent;
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();
            clear();
            Generate();
            update();
            this.neighbours = new ChunkNeighbours();
        }

        void clear()
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
        }

        bool update()
        {
            return CalculateMesh();
        }

        List<Side> GetVisableSides(BlockData block)
        {
            List<Side> sides = new List<Side>();

            if (!blocks.ContainsKey(new ByteVector3(block.position.x, (byte)(block.position.y + 1), block.position.z))) sides.Add(Side.top);
            if (!blocks.ContainsKey(new ByteVector3(block.position.x, (byte)(block.position.y - 1), block.position.z))) sides.Add(Side.bottom);
            if (!blocks.ContainsKey(new ByteVector3((byte)(block.position.x - 1), block.position.y, block.position.z))) sides.Add(Side.left);
            if (!blocks.ContainsKey(new ByteVector3((byte)(block.position.x + 1), block.position.y, block.position.z))) sides.Add(Side.right);
            if (!blocks.ContainsKey(new ByteVector3(block.position.x, block.position.y, (byte)(block.position.z + 1)))) sides.Add(Side.back);
            if (!blocks.ContainsKey(new ByteVector3(block.position.x, block.position.y, (byte)(block.position.z - 1)))) sides.Add(Side.front);
            return sides;
        }

        TextureIndex GetTextureIndex(BlockType type)
        {
            TextureIndex output = new TextureIndex();
            if (type == BlockType.GRASS)
            {
                output.bottom = TextureLocation.dirt;
                output.side = TextureLocation.grassSide;
                output.top = TextureLocation.grassTop;
            }
            else if (type == BlockType.DIRT)
            {
                output.bottom = TextureLocation.dirt;
                output.side = TextureLocation.dirt;
                output.top = TextureLocation.dirt;
            }
            else if (type == BlockType.STONE)
            {
                output.bottom = TextureLocation.stone;
                output.side = TextureLocation.stone;
                output.top = TextureLocation.stone;
            }
            return output;
        }

        bool CalculateMesh()
        {
            if (changed)
            {
                clear();
                int sideNr = 0;
                foreach (KeyValuePair<ByteVector3, BlockData> block in blocks)
                {
                    List<Side> visableSides = GetVisableSides(block.Value);
                    TextureIndex textureIndex = GetTextureIndex(block.Value.type);
                    Block newBlock = new Block(visableSides, new Vector3(block.Value.position.x, block.Value.position.y, block.Value.position.z), textureIndex, ref sideNr);
                    newBlock.AddBlockToMesh(vertices, triangles, uvs);
                    newBlock = null;
                    visableSides = null;
                    textureIndex = null;
                }
                changed = false;
                return true;
            }
            return false;
        }

        public void AddChunkToMesh(List<Vector3> outputVertices, List<int> outputTriangles, List<Vector2> outputUvs)
        {
            outputVertices.AddRange(vertices);
            outputTriangles.AddRange(triangles);
            outputUvs.AddRange(uvs);
        }
        public void AddBlock(BlockData newBlock)
        {
            if (!blocks.ContainsKey(newBlock.position))
            {
                blocks.Add(newBlock.position, newBlock);
                changed = true;
                update();
            }
        }

        public void DestroyBlock(ByteVector3 position)
        {
            blocks.Remove(position);
            changed = true;
            update();
        }
    }
}
