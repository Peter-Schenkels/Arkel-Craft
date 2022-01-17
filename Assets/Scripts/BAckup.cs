//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//using BlockCollision;

//namespace ChunkLoading
//{


//    [RequireComponent(typeof(MeshFilter))]
//    public class WorldGeneration : MonoBehaviour
//    {

//        public enum Side
//        {
//            top,
//            bottom,
//            left,
//            right,
//            front,
//            back
//        };



//        Mesh mesh;
//        MeshFilter meshFilter;
//        MeshCollider meshCollider;
//        Chunk chunk;
//        public GameObject BlockCollisionPrefab;

//        List<Vector3> vertices;
//        List<int> triangles;
//        List<Vector2> uvs;
//        Vector2 indexSize;
//        void Start()
//        {
//            mesh = new Mesh();
//            meshFilter = GetComponent<MeshFilter>();
//            meshFilter.mesh = mesh;
//            vertices = new List<Vector3>();
//            triangles = new List<int>();
//            uvs = new List<Vector2>();
//            chunk = new Chunk(this, BlockCollisionPrefab, transform.position);
//            CreateShape();
//            meshCollider = GetComponent<MeshCollider>();
//            meshCollider.sharedMesh = mesh;
//        }


//        public static class TextureLocation
//        {
//            public static int sizeX = 4;
//            public static int sizeY = 2;
//            public static int textureSizeX = 1;
//            public static int textureSizeY = 1;
//            public static Vector2 grassSide = new Vector2(0, 1);
//            public static Vector2 grassTop = new Vector2(1, 1);
//            public static Vector2 dirt = new Vector2(2, 1);
//            public static Vector2 stone = new Vector2(3, 1);
//        };

//        class TextureIndex
//        {
//            public Vector2 side;
//            public Vector2 top;
//            public Vector2 bottom;
//        }




//        public enum BlockType
//        {
//            GRASS,
//            DIRT,
//            AIR,
//            STONE
//        }


//        public class ByteVector3
//        {
//            public byte x;
//            public byte y;
//            public byte z;

//            int ID;
//            public ByteVector3(byte x, byte y, byte z)
//            {
//                this.x = x;
//                this.y = y;
//                this.z = z;

//                ID = x * 31 + y * 37 + z * 41;

//            }

//            public override int GetHashCode()
//            {
//                return ID;
//            }

//            public override bool Equals(object obj)
//            {
//                return Equals(obj as ByteVector3);
//            }

//            public bool Equals(ByteVector3 obj)
//            {
//                return obj != null && obj.x == this.x && obj.y == this.y && obj.z == this.z;
//            }


//        }


//        public class BlockData
//        {
//            public ByteVector3 position;
//            public BlockType type;

//            public BlockData(ByteVector3 position, BlockType type)
//            {
//                this.position = position;
//                this.type = type;
//            }
//        }

//        void clear()
//        {
//            vertices = new List<Vector3>();
//            triangles = new List<int>();
//            uvs = new List<Vector2>();
//        }


//        public void DestroyBlock(ByteVector3 position)
//        {
//            chunk.DestroyBlock(position);
//            clear();
//            CreateShape();
//            reloadMeshCollider();
//        }

//        public class Chunk
//        {
//            bool changed = true;
//            public Vector3 position;
//            public WorldGeneration parent;

//            public const int sizeX = 16;
//            public const int sizeY = 16;
//            public const int sizeZ = 16;
//            public Dictionary<ByteVector3, BlockData> blocks;

//            public List<Vector3> vertices;
//            public List<int> triangles;
//            public List<Vector2> uvs;

//            public void Generate()
//            {
//                blocks = new Dictionary<ByteVector3, BlockData>();

//                int airBorder = 8;
//                int stoneBorder = 6;
//                for (byte x = 0; x < sizeX; x++)
//                {
//                    for (byte y = 0; y < sizeY; y++)
//                    {
//                        for (byte z = 0; z < sizeZ; z++)
//                        {
//                            if (y == 8 && x == 8 && z == 8)
//                            {
//                                continue;
//                            }
//                            else if (y < stoneBorder)
//                            {
//                                blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.STONE));
//                            }
//                            else if (y < airBorder)
//                            {
//                                blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.DIRT));
//                            }
//                            else if (y == airBorder)
//                            {
//                                blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.GRASS));
//                            }

//                        }
//                    }
//                }
//            }

//            public Chunk(WorldGeneration parent, GameObject BlockCollisionPrefab, Vector3 position)
//            {
//                this.parent = parent;
//                this.position = position;
//                vertices = new List<Vector3>();
//                triangles = new List<int>();
//                uvs = new List<Vector2>();
//                clear();
//                Generate();
//                update();
//            }

//            void clear()
//            {
//                vertices.Clear();
//                triangles.Clear();
//                uvs.Clear();
//            }


//            bool update()
//            {
//                return CalculateMesh();
//            }

//            List<Side> GetVisableSides(BlockData block)
//            {
//                List<Side> sides = new List<Side>();

//                if (!blocks.ContainsKey(new ByteVector3(block.position.x, (byte)(block.position.y + 1), block.position.z))) sides.Add(Side.top);
//                if (!blocks.ContainsKey(new ByteVector3(block.position.x, (byte)(block.position.y - 1), block.position.z))) sides.Add(Side.bottom);
//                if (!blocks.ContainsKey(new ByteVector3((byte)(block.position.x - 1), block.position.y, block.position.z))) sides.Add(Side.left);
//                if (!blocks.ContainsKey(new ByteVector3((byte)(block.position.x + 1), block.position.y, block.position.z))) sides.Add(Side.right);
//                if (!blocks.ContainsKey(new ByteVector3(block.position.x, block.position.y, (byte)(block.position.z + 1)))) sides.Add(Side.back);
//                if (!blocks.ContainsKey(new ByteVector3(block.position.x, block.position.y, (byte)(block.position.z - 1)))) sides.Add(Side.front);

//                return sides;
//            }

//            TextureIndex GetTextureIndex(BlockType type)
//            {
//                TextureIndex output = new TextureIndex();
//                if (type == BlockType.GRASS)
//                {
//                    output.bottom = TextureLocation.dirt;
//                    output.side = TextureLocation.grassSide;
//                    output.top = TextureLocation.grassTop;
//                }
//                else if (type == BlockType.DIRT)
//                {
//                    output.bottom = TextureLocation.dirt;
//                    output.side = TextureLocation.dirt;
//                    output.top = TextureLocation.dirt;
//                }
//                else if (type == BlockType.STONE)
//                {
//                    output.bottom = TextureLocation.stone;
//                    output.side = TextureLocation.stone;
//                    output.top = TextureLocation.stone;
//                }
//                return output;
//            }

//            bool CalculateMesh()
//            {
//                if (changed)
//                {
//                    clear();
//                    int sideNr = 0;
//                    foreach (KeyValuePair<ByteVector3, BlockData> block in blocks)
//                    {

//                        List<Side> visableSides = GetVisableSides(block.Value);
//                        TextureIndex textureIndex = GetTextureIndex(block.Value.type);
//                        Block newBlock = new Block(visableSides, new Vector3(block.Value.position.x, block.Value.position.y, block.Value.position.z), textureIndex, ref sideNr);
//                        newBlock.AddBlockToMesh(vertices, triangles, uvs);
//                        newBlock = null;
//                        visableSides = null;
//                        textureIndex = null;
//                    }
//                    changed = false;
//                    return true;
//                }
//                return false;
//            }

//            public void AddChunkToMesh(List<Vector3> outputVertices, List<int> outputTriangles, List<Vector2> outputUvs)
//            {
//                outputVertices.AddRange(vertices);
//                outputTriangles.AddRange(triangles);
//                outputUvs.AddRange(uvs);
//            }

//            public void DestroyBlock(ByteVector3 position)
//            {
//                Debug.Log(position.x);
//                Debug.Log(position.y);
//                Debug.Log(position.z);

//                blocks.Remove(position);
//                changed = true;
//                update();
//            }

//        }


//        void reloadMeshCollider()
//        {
//            DestroyImmediate(this.GetComponent<MeshCollider>());
//            var collider = this.gameObject.AddComponent<MeshCollider>();
//            collider.sharedMesh = mesh;
//        }





//        class Block
//        {
//            Vector3 position { get; }
//            TextureIndex textureIndex;

//            public List<Vector3> vertices;
//            public List<int> triangles;
//            public List<Vector2> uvs;

//            public Block(List<Side> visableSides, Vector3 position, TextureIndex textureIndex, ref int sideNr)
//            {
//                this.position = position;
//                this.textureIndex = textureIndex;

//                vertices = new List<Vector3>();
//                triangles = new List<int>();
//                uvs = new List<Vector2>();

//                int sideIndex = 0;

//                BlockSide[] sides = new BlockSide[visableSides.Count];
//                foreach (var side in visableSides)
//                {
//                    if (side == Side.back || side == Side.front || side == Side.left || side == Side.right)
//                    {
//                        sides[sideIndex] = new BlockSide(side, position, textureIndex.side, sideNr + sideIndex);
//                    }
//                    else if (side == Side.top)
//                    {
//                        sides[sideIndex] = new BlockSide(side, position, textureIndex.top, sideNr + sideIndex);
//                    }
//                    else
//                    {
//                        sides[sideIndex] = new BlockSide(side, position, textureIndex.bottom, sideNr + sideIndex);
//                    }
//                    sides[sideIndex].AddSideToMesh(vertices, triangles, uvs);
//                    sideIndex++;
//                }
//                sideNr += visableSides.Count;
//            }

//            public void AddBlockToMesh(List<Vector3> outputVertices, List<int> outputTriangles, List<Vector2> outputUvs)
//            {
//                outputVertices.AddRange(vertices);
//                outputTriangles.AddRange(triangles);
//                outputUvs.AddRange(uvs);
//            }
//        };

//        class BlockSide
//        {

//            public Side side;
//            public Vector3[] vertices;
//            public int[] triangles;
//            public Vector2[] uvs;
//            Vector2 indexSize;

//            public Vector2[] getUv(Vector2 index)
//            {
//                return new Vector2[]
//                {
//            new Vector2(index.x * indexSize.x, index.y * indexSize.y),
//            new Vector2(index.x * indexSize.x, index.y * indexSize.y + indexSize.y),
//            new Vector2(index.x * indexSize.x + indexSize.x, index.y * indexSize.y),
//            new Vector2(index.x * indexSize.x + indexSize.x, index.y * indexSize.y + indexSize.y)
//                };
//            }

//            public BlockSide(Side side, Vector3 position, Vector2 textureIndex, int SideNr)
//            {
//                indexSize = new Vector2((float)TextureLocation.textureSizeX / (float)TextureLocation.sizeX, (float)TextureLocation.textureSizeY / (float)TextureLocation.sizeY);
//                switch (side)
//                {
//                    case Side.back:
//                        vertices = new Vector3[]
//                        {
//                        new Vector3(1,0,1) + position,
//                        new Vector3(1,1,1) + position,
//                        new Vector3(0,0,1) + position,
//                        new Vector3(0,1,1) + position
//                        };
//                        break;

//                    case Side.front:
//                        vertices = new Vector3[]
//                        {
//                        new Vector3(0,0,0) + position,
//                        new Vector3(0,1,0) + position,
//                        new Vector3(1,0,0) + position,
//                        new Vector3(1,1,0) + position
//                        };
//                        break;

//                    case Side.top:
//                        vertices = new Vector3[]
//                        {
//                        new Vector3(0,1,0) + position,
//                        new Vector3(0,1,1) + position,
//                        new Vector3(1,1,0) + position,
//                        new Vector3(1,1,1) + position
//                        };
//                        break;

//                    case Side.bottom:
//                        vertices = new Vector3[]
//                        {
//                        new Vector3(1,0,0) + position,
//                        new Vector3(1,0,1) + position,
//                        new Vector3(0,0,0) + position,
//                        new Vector3(0,0,1) + position
//                        };
//                        break;

//                    case Side.left:
//                        vertices = new Vector3[]
//                        {
//                        new Vector3(0,0,1) + position,
//                        new Vector3(0,1,1) + position,
//                        new Vector3(0,0,0) + position,
//                        new Vector3(0,1,0) + position
//                        };
//                        break;

//                    case Side.right:
//                        vertices = new Vector3[]
//                        {
//                        new Vector3(1,0,0) + position,
//                        new Vector3(1,1,0) + position,
//                        new Vector3(1,0,1) + position,
//                        new Vector3(1,1,1) + position
//                        };
//                        break;
//                }
//                triangles = new int[]
//                {
//                0 + SideNr * 4,
//                1 + SideNr * 4,
//                2 + SideNr * 4,
//                1 + SideNr * 4,
//                3 + SideNr * 4,
//                2 + SideNr * 4
//                };

//                uvs = getUv(textureIndex);
//            }

//            public void AddSideToMesh(List<Vector3> outputVertices, List<int> outputTriangles, List<Vector2> outputUvs)
//            {
//                outputVertices.AddRange(vertices);
//                outputTriangles.AddRange(triangles);
//                outputUvs.AddRange(uvs);
//            }

//        };


//        void CreateShape()
//        {
//            chunk.AddChunkToMesh(vertices, triangles, uvs);
//            mesh.Clear();
//            mesh.SetVertices(vertices);
//            mesh.SetTriangles(triangles, 0);
//            mesh.SetUVs(0, uvs);
//            mesh.RecalculateNormals();
//        }

//        // Update is called once per frame
//        public void Update()
//        {
//        }
//    }
//}
