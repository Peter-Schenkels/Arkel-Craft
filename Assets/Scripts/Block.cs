using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Block
    {
        Vector3 position { get; }
        TextureIndex textureIndex;

        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uvs;

        public Block(List<Side> visableSides, Vector3 position, TextureIndex textureIndex, ref int sideNr)
        {
            this.position = position;
            this.textureIndex = textureIndex;

            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();

            int sideIndex = 0;

            BlockSide[] sides = new BlockSide[visableSides.Count];
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
                sides[sideIndex].AddSideToMesh(vertices, triangles, uvs);
                sideIndex++;
            }
            sideNr += visableSides.Count;
        }

        
        public void AddBlockToMesh(List<Vector3> outputVertices, List<int> outputTriangles, List<Vector2> outputUvs)
        {
            outputVertices.AddRange(vertices);
            outputTriangles.AddRange(triangles);
            outputUvs.AddRange(uvs);
        }
    };
}
