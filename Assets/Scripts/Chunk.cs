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

        public int sizeX = 16;
        public int sizeY = 16;
        public int sizeZ = 16;

        public Dictionary<ByteVector3, BlockData> blocks;

        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uvs;

        public static TextureIndex GetTextureIndex(BlockType type)
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

        public void Generate()
        {
            blocks = new Dictionary<ByteVector3, BlockData>();
            int airBorder = 8;
            int stoneBorder = 6;
            Vector3 isChunkBorder = new Vector3(0, 0, 0);
            for (byte x = 0; x < sizeX; x++)
            {
                if(x == sizeX-1)
                {
                    isChunkBorder = new Vector3(1, 0, 0) + isChunkBorder;
                }
                for (byte y = 0; y < sizeY; y++)
                {
                    if (y == sizeY - 1)
                    {
                        isChunkBorder = new Vector3(0, 1, 0) + isChunkBorder;
                    }
                    for (byte z = 0; z < sizeZ; z++)
                    {
                        if (z == sizeZ - 1)
                        {
                            isChunkBorder = new Vector3(0, 0, 1) + isChunkBorder;
                        }
                        if (y < stoneBorder)
                        {
                            blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.STONE, new ByteVector3(isChunkBorder)));
                        }
                        else if (y < airBorder)
                        {
                            blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.DIRT, new ByteVector3(isChunkBorder)));
                        }
                        else if (y == airBorder)
                        {
                            blocks.Add(new ByteVector3(x, y, z), new BlockData(new ByteVector3(x, y, z), BlockType.GRASS, new ByteVector3(isChunkBorder)));
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

        class ConsumeOutput
        {
            public bool succes;
            public GreedyMeshStructure ouput;

            public ConsumeOutput(bool succes, GreedyMeshStructure output)
            {
                this.succes = succes;
                this.ouput = output;
            }
        }

        class GreedyMeshStructure
        {
            public List<BlockData> completedBlocks;
            public List<BlockData> currentRow;
            public TextureIndex meshTextureIndex;
            private Side side;
            public int width;
            public int length;
            public BlockType type;

            private List<Vector3> vertices;
            private List<int> triangles;
            private List<Vector2> uvs;

            class CollumnRowHeight
            {
                public byte row;
                public byte collumn;
                public byte height;

                public CollumnRowHeight(byte row, byte collumn, byte height)
                {
                    this.row = row;
                    this.collumn = collumn;
                    this.height = height;
                }
            }

            public GreedyMeshStructure()
            {
                currentRow = new List<BlockData>();
                completedBlocks = new List<BlockData>();
                this.width = 0;
                this.length = 0;
            }

            public GreedyMeshStructure(Side side)
            {
                currentRow = new List<BlockData>();
                completedBlocks = new List<BlockData>();
                this.side = side;
                this.width = 0;
                this.length = 0; 
            }

            public GreedyMeshStructure(List<BlockData> completedBlocks, Side side)
            {
                currentRow = new List<BlockData>();
                this.completedBlocks = completedBlocks;
                this.side = side;
            }

            public ConsumeOutput Consume(BlockData block)
            {
                BlockData firstBlock;
                BlockData lastBlock;
                if (this.currentRow.Count() == 0)
                {
                    this.currentRow.Add(block);
                    firstBlock = block;
                    this.type = block.type;              
                    return new ConsumeOutput(true, null);
                }
                else
                {
                    firstBlock = this.currentRow.First();
                    lastBlock = this.currentRow.Last();
                    if(block.type != firstBlock.type)
                    {
                        return new ConsumeOutput(false, new GreedyMeshStructure(this.currentRow, this.side));
                    }
                    CollumnRowHeight firstCollumnRow;
                    CollumnRowHeight newCollumnRow;
                    CollumnRowHeight lastCollumnRow;
                    if (side == Side.top || side == Side.bottom)
                    {
                        firstCollumnRow = new CollumnRowHeight(firstBlock.position.x, firstBlock.position.z, firstBlock.position.y);
                        lastCollumnRow = new CollumnRowHeight(lastBlock.position.x, lastBlock.position.z, lastBlock.position.y);
                        newCollumnRow = new CollumnRowHeight(block.position.x, block.position.z, block.position.y);
                    }
                    else if (side == Side.back || side == Side.front)
                    {
                        firstCollumnRow = new CollumnRowHeight(firstBlock.position.y, firstBlock.position.x, firstBlock.position.z);
                        lastCollumnRow = new CollumnRowHeight(lastBlock.position.y, lastBlock.position.x, lastBlock.position.z);
                        newCollumnRow = new CollumnRowHeight(block.position.y, block.position.x, block.position.z);
                    }
                    else // default: side is left or right
                    {
                        firstCollumnRow = new CollumnRowHeight(firstBlock.position.y, firstBlock.position.z, firstBlock.position.x);
                        lastCollumnRow = new CollumnRowHeight(lastBlock.position.y, lastBlock.position.z, lastBlock.position.x);
                        newCollumnRow = new CollumnRowHeight(block.position.y, block.position.z, block.position.x);
                    }
                    if (firstCollumnRow.row == newCollumnRow.row && lastCollumnRow.collumn + 1 == newCollumnRow.collumn && firstCollumnRow.height == newCollumnRow.height)
                    {
                        this.currentRow.Add(block);
                        return new ConsumeOutput(true, null);
                    }
                    else if (lastCollumnRow.row + 1 == newCollumnRow.row && firstCollumnRow.collumn == newCollumnRow.collumn && firstCollumnRow.height == newCollumnRow.height)
                    {
                        this.length = currentRow.Count;
                        this.width++;
                        this.completedBlocks.AddRange(this.currentRow);
                        this.currentRow.Clear();
                        this.currentRow.Add(block);
                        return new ConsumeOutput(true, null);
                    }
                    else
                    {
                        return new ConsumeOutput(false, new GreedyMeshStructure(this.currentRow, this.side));
                    }                                
                }
            }

            public Vector2[] getUv(Vector2 index)
            {
                Vector2 indexSize = new Vector2((float)TextureLocation.textureSizeX / (float)TextureLocation.sizeX, (float)TextureLocation.textureSizeY / (float)TextureLocation.sizeY);
                return new Vector2[]
                {
            new Vector2(index.x * indexSize.x, index.y * indexSize.y),
            new Vector2(index.x * indexSize.x, index.y * indexSize.y + indexSize.y),
            new Vector2(index.x * indexSize.x + indexSize.x, index.y * indexSize.y),
            new Vector2(index.x * indexSize.x + indexSize.x, index.y * indexSize.y + indexSize.y)
                };
            }

            private void GenerateMesh(ref int sideNr)
            {
                ByteVector3 bytePos = completedBlocks.First().position;
                Vector3 position = new Vector3(bytePos.x, bytePos.y, bytePos.z);
                Vector3[] vertices;
                int[] triangles;
                TextureIndex textureIndex = GetTextureIndex(this.type);
                switch (this.side)
                {
                    case Side.back:
                        vertices = new Vector3[]
                        {
                        new Vector3(this.width,0,1) + position,
                        new Vector3(this.width,this.length,1) + position,
                        new Vector3(0,0,1) + position,
                        new Vector3(0,this.length,1) + position
                        };
                        break;

                    case Side.front:
                        vertices = new Vector3[]
                        {
                        new Vector3(0,0,0) + position,
                        new Vector3(0,this.length,0) + position,
                        new Vector3(this.width,0,0) + position,
                        new Vector3(this.width,this.length,0) + position
                        };
                        break;

                    case Side.top:
                        vertices = new Vector3[]
                        {
                        new Vector3(0,1,0) + position,
                        new Vector3(0,1,this.length) + position,
                        new Vector3(this.width,1,0) + position,
                        new Vector3(this.width,1,this.length) + position
                        };
                        break;

                    case Side.bottom:
                        vertices = new Vector3[]
                        {
                        new Vector3(this.width,0,0) + position,
                        new Vector3(this.width,0,this.length) + position,
                        new Vector3(0,0,0) + position,
                        new Vector3(0,0,this.length) + position
                        };
                        break;

                    case Side.left:
                        vertices = new Vector3[]
                        {
                        new Vector3(0,0,this.width) + position,
                        new Vector3(0,this.length,this.width) + position,
                        new Vector3(0,0,0) + position,
                        new Vector3(0,this.length,0) + position
                        };
                        break;

                    case Side.right:
                        vertices = new Vector3[]
                        {
                        new Vector3(1,0,0) + position,
                        new Vector3(1,this.length,0) + position,
                        new Vector3(1,0,this.width) + position,
                        new Vector3(1,this.length,this.width) + position
                        };
                        break;
                }
                triangles = new int[]
                {
                0 + sideNr * 4,
                1 + sideNr * 4,
                2 + sideNr * 4,
                1 + sideNr * 4,
                3 + sideNr * 4,
                2 + sideNr * 4
                };

                //uvs = getUv(textureIndex);
            }


            public void AddToMesh(List<Vector3> outputVertices, List<int> outputTriangles, List<Vector2> outputUvs, ref int sideNr)
            {
                this.GenerateMesh(ref sideNr);
            }
        }

        private List<GreedyMeshStructure> CombineGreedyMeshStrips(List<GreedyMeshStructure> input)
        {
            List<GreedyMeshStructure> combinedMeshStripsStructures = new List<GreedyMeshStructure>();
            GreedyMeshStructure createdGreedyMeshStruct = new GreedyMeshStructure();
            bool greedymeshReset = true;
            foreach (GreedyMeshStructure strip in input)
            {
                if (!greedymeshReset)
                {
                    if (strip.length == createdGreedyMeshStruct.length && strip.type == createdGreedyMeshStruct.type)
                    {
                        createdGreedyMeshStruct.completedBlocks.AddRange(strip.completedBlocks);
                        createdGreedyMeshStruct.width += strip.width;
                    }
                    else
                    {
                        combinedMeshStripsStructures.Add(createdGreedyMeshStruct);
                        greedymeshReset = true;
                        continue;
                    }
                }
                else
                {
                    createdGreedyMeshStruct = strip;
                    greedymeshReset = false;
                }
            }
            combinedMeshStripsStructures.Add(createdGreedyMeshStruct);
            return combinedMeshStripsStructures;
        }

        bool CalculateMesh()
        {
            if (changed)
            {
                clear();
                int sideNr = 0;
                Dictionary<Side, List<GreedyMeshStructure>> greedyMesh = new Dictionary<Side, List<GreedyMeshStructure>>
                { 
                    {Side.top,new List<GreedyMeshStructure>()}, 
                    {Side.right, new List<GreedyMeshStructure>()},
                    {Side.left, new List<GreedyMeshStructure>()},
                    {Side.front, new List<GreedyMeshStructure>()},
                    {Side.bottom, new List<GreedyMeshStructure>()},
                    {Side.back, new List<GreedyMeshStructure>()}
                };
                List<GreedyMeshStructure> greedyMeshes = new List<GreedyMeshStructure>();
                GreedyMeshStructure selectedMeshStruct = new GreedyMeshStructure();
                foreach (KeyValuePair<ByteVector3, BlockData> block in blocks)
                {
                    List<Side> visableSides = GetVisableSides(block.Value);
                    foreach (Side visableSide in visableSides)
                    {
                        if (visableSide == Side.front)
                        {
                            if (greedyMesh[visableSide].Count == 0)
                            {
                                greedyMesh[visableSide].Add(new GreedyMeshStructure(visableSide));
                            }
                            selectedMeshStruct = greedyMesh[visableSide].Last();
                            var output = selectedMeshStruct.Consume(block.Value);
                            if (!output.succes)
                            {
                                greedyMesh[visableSide].Add(new GreedyMeshStructure(selectedMeshStruct.completedBlocks, visableSide));
                                greedyMesh[visableSide].Add(new GreedyMeshStructure(selectedMeshStruct.currentRow, visableSide));
                                var input = new GreedyMeshStructure(visableSide);
                                input.Consume(block.Value);
                                greedyMesh[visableSide].Add(input);
                            }
                        }
                    }
                    TextureIndex textureIndex = GetTextureIndex(block.Value.type);
                    Block newBlock = new Block(visableSides, new Vector3(block.Value.position.x, block.Value.position.y, block.Value.position.z), textureIndex, ref sideNr);
                    newBlock.AddBlockToMesh(vertices, triangles, uvs);
                }

                foreach (var collection in greedyMesh)
                {
                    List<GreedyMeshStructure> updatedMeshStructures = this.CombineGreedyMeshStrips(collection.Value);
                    greedyMeshes.AddRange(updatedMeshStructures);                       
                } 
                /* 
                 * WORK IN PROGRESS
                foreach (GreedyMeshStructure greedyStruct in greedyMeshes)
                {
                    greedyStruct.AddToMesh(vertices, triangles, uvs, ref meshSideNr);
                }
                */
                changed = false;
                return true;
            }
            return false;
        }

        void GreedyMeshStructureToMesh(GreedyMeshStructure input)
        {

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
