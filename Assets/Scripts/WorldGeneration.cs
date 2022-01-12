using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class WorldGeneration : MonoBehaviour
{

    public enum Side
    {
        top,
        bottom,
        left,
        right,
        front,
        back
    };



    Mesh mesh;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector2 indexSize;
    void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        vertices = new Vector3[] { };
        triangles = new int[] { };
        uvs = new Vector2[] { };
        CreateShape();
        UpdateMesh();
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        meshCollider.convex = false;

    }

    void UpdateMesh()
    {

    }





    class WorldMeshManager
    {



    };

    public static class TextureLocation
    {
        public static int sizeX = 4;
        public static int sizeY = 2;
        public static int textureSizeX = 1;
        public static int textureSizeY = 1;
        public static Vector2 grassSide = new Vector2(0, 1);
        public static Vector2 grassTop = new Vector2(1, 1);
        public static Vector2 dirt = new Vector2(2, 1);
    };

    class TextureIndex
    {
        public Vector2 side;
        public Vector2 top;
        public Vector2 bottom;
    }




    enum BlockType
    {
        GRASS,
        DIRT,
        AIR
    }


    class ByteVector3
    {
        public byte x;
        public byte y;
        public byte z;

        int ID;
        public ByteVector3(byte x, byte y, byte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            ID = x * 31 + y * 37 + z * 41;

        }

        public override int GetHashCode()
        {
            return ID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ByteVector3);
        }

        public bool Equals(ByteVector3 obj)
        {
            return obj != null && obj.x == this.x && obj.y == this.y && obj.z == this.z;
        }


    }


    class BlockData
    {
        public ByteVector3 position;
        public BlockType type;

        public BlockData(ByteVector3 position, BlockType type)
        {
            this.position = position;
            this.type = type;
        }
    }


    class Chunk
    {
        public const int sizeX = 16;
        public const int sizeY = 16;
        public const int sizeZ = 16;
        public Dictionary<ByteVector3, BlockData> blocks;

        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        public void Generate()
        {
            blocks = new Dictionary<ByteVector3, BlockData>();
            int airBorder = 8;;
            for (byte x = 0; x < sizeX; x++)
            {
                for (byte y = 0; y < sizeY; y++)
                {
                    for (byte z = 0; z < sizeZ; z++)
                    {
                        if (y < airBorder)
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

        public Chunk()
        {
            clear();
            Generate();
            update();
        }

        void clear()
        {
            vertices = new Vector3[] { };
            triangles = new int[] { };
            uvs = new Vector2[] { };
        }


        void update()
        {
            CalculateMesh();
        }

        Side[] GetVisableSides(BlockData block)
        {
            List<Side> sides = new List<Side>();

            if (!blocks.ContainsKey(new ByteVector3(block.position.x, (byte)(block.position.y + 1), block.position.z))) sides.Add(Side.top);
            if (!blocks.ContainsKey(new ByteVector3(block.position.x, (byte)(block.position.y - 1), block.position.z))) sides.Add(Side.bottom);
            if (!blocks.ContainsKey(new ByteVector3((byte)(block.position.x - 1), block.position.y, block.position.z))) sides.Add(Side.left);
            if (!blocks.ContainsKey(new ByteVector3((byte)(block.position.x + 1), block.position.y, block.position.z))) sides.Add(Side.right);
            if (!blocks.ContainsKey(new ByteVector3(block.position.x, block.position.y, (byte)(block.position.z + 1)))) sides.Add(Side.back);
            if (!blocks.ContainsKey(new ByteVector3(block.position.x, block.position.y, (byte)(block.position.z - 1)))) sides.Add(Side.front);

            return sides.ToArray();

            //return new Side[] { Side.back, Side.bottom, Side.front, Side.left, Side.right, Side.top };
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
            return output;
        }

        void CalculateMesh()
        {
            clear();
            int sideNr = 0;
            foreach(KeyValuePair<ByteVector3, BlockData> block in blocks)
            {
                Side[] visableSides = GetVisableSides(block.Value);
                TextureIndex textureIndex = GetTextureIndex(block.Value.type);
                Block newBlock = new Block(visableSides, new Vector3(block.Value.position.x, block.Value.position.y, block.Value.position.z), textureIndex, ref sideNr);
                newBlock.AddBlockToMesh(ref vertices, ref triangles, ref uvs);
                newBlock = null;
                visableSides = null;
                textureIndex = null;
            }
        }

        public void AddChunkToMesh(ref Vector3[] outputVertices, ref int[] outputTriangles, ref Vector2[] outputUvs)
        {
            outputVertices = outputVertices.Concat(vertices).ToArray();
            outputTriangles = outputTriangles.Concat(triangles).ToArray();
            outputUvs = outputUvs.Concat(uvs).ToArray();
        }

    }






    class Block
    {
        Vector3 position { get; }
        TextureIndex textureIndex;

        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        public Block(Side[] visableSides, Vector3 position, TextureIndex textureIndex, ref int sideNr)
        {
            this.position = position;
            this.textureIndex = textureIndex;

            vertices = new Vector3[] { };
            triangles = new int[] { };
            uvs = new Vector2[] { };

            int sideIndex = 0;

            BlockSide[] sides = new BlockSide[visableSides.Length];
            foreach (var side in visableSides)
            {
                if (side == Side.back || side == Side.front || side == Side.left || side == Side.right)
                {
                    sides[sideIndex] = new BlockSide(side, position, textureIndex.side, sideNr + sideIndex);
                }
                else if (side == Side.top)
                {
                    sides[sideIndex] = new BlockSide(side, position, textureIndex.top, sideNr + sideIndex);
                }
                else
                {
                    sides[sideIndex] = new BlockSide(side, position, textureIndex.bottom, sideNr + sideIndex);
                }
                sides[sideIndex].AddSideToMesh(ref vertices, ref triangles, ref uvs);
                sideIndex++;
            }
            sideNr += visableSides.Length;
        }

        public void AddBlockToMesh(ref Vector3[] outputVertices, ref int[] outputTriangles, ref Vector2[] outputUvs)
        {
            outputVertices = outputVertices.Concat(vertices).ToArray();
            outputTriangles = outputTriangles.Concat(triangles).ToArray();
            outputUvs = outputUvs.Concat(uvs).ToArray();
        }
    };

    class BlockSide
    {

        public Side side;
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        Vector2 indexSize;

        public Vector2[] getUv(Vector2 index)
        {
            return new Vector2[]
            {
            new Vector2(index.x * indexSize.x, index.y * indexSize.y),
            new Vector2(index.x * indexSize.x, index.y * indexSize.y + indexSize.y),
            new Vector2(index.x * indexSize.x + indexSize.x, index.y * indexSize.y),
            new Vector2(index.x * indexSize.x + indexSize.x, index.y * indexSize.y + indexSize.y)
            };
        }

        public BlockSide(Side side, Vector3 position, Vector2 textureIndex, int SideNr)
        {
            indexSize = new Vector2((float)TextureLocation.textureSizeX / (float)TextureLocation.sizeX, (float)TextureLocation.textureSizeY / (float)TextureLocation.sizeY);
            switch (side)
            {
                case Side.back:
                    vertices = new Vector3[]
                    {
                        new Vector3(1,0,1) + position,
                        new Vector3(1,1,1) + position,
                        new Vector3(0,0,1) + position,
                        new Vector3(0,1,1) + position
                    };
                    break;

                case Side.front:
                    vertices = new Vector3[]
                    {
                        new Vector3(0,0,0) + position,
                        new Vector3(0,1,0) + position,
                        new Vector3(1,0,0) + position,
                        new Vector3(1,1,0) + position
                    };
                    break;

                case Side.top:
                    vertices = new Vector3[]
                    {
                        new Vector3(0,1,0) + position,
                        new Vector3(0,1,1) + position,
                        new Vector3(1,1,0) + position,
                        new Vector3(1,1,1) + position
                    };
                    break;

                case Side.bottom:
                    vertices = new Vector3[]
                    {
                        new Vector3(1,0,0) + position,
                        new Vector3(1,0,1) + position,
                        new Vector3(0,0,0) + position,
                        new Vector3(0,0,1) + position
                    };
                    break;

                case Side.left:
                    vertices = new Vector3[]
                    {
                        new Vector3(0,0,1) + position,
                        new Vector3(0,1,1) + position,
                        new Vector3(0,0,0) + position,
                        new Vector3(0,1,0) + position
                    };
                    break;

                case Side.right:
                    vertices = new Vector3[]
                    {
                        new Vector3(1,0,0) + position,
                        new Vector3(1,1,0) + position,
                        new Vector3(1,0,1) + position,
                        new Vector3(1,1,1) + position
                    };
                    break;
            }
            triangles = new int[]
            {
                0 + SideNr * 4, 
                1 + SideNr * 4, 
                2 + SideNr * 4, 
                1 + SideNr * 4, 
                3 + SideNr * 4, 
                2 + SideNr * 4
            };

            uvs = getUv(textureIndex);
        }

        public void AddSideToMesh(ref Vector3[] outputVertices, ref int[] outputTriangles, ref Vector2[] outputUvs)
        {
            outputVertices = outputVertices.Concat(vertices).ToArray();
            outputTriangles = outputTriangles.Concat(triangles).ToArray();
            outputUvs = outputUvs.Concat(uvs).ToArray();
        }

    };


    void CreateShape()
    {
        Chunk chunk = new Chunk();
        chunk.AddChunkToMesh(ref vertices, ref triangles, ref uvs);
    }

    // Update is called once per frame
    public void Update()
    {
        meshCollider.convex = false;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        meshCollider.convex = true;
    }
}
