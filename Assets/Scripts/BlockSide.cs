using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Side
{
    top,
    bottom,
    left,
    right,
    front,
    back
};

namespace Assets.Scripts
{
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

        public void AddSideToMesh(List<Vector3> outputVertices, List<int> outputTriangles, List<Vector2> outputUvs)
        {
            outputVertices.AddRange(vertices);
            outputTriangles.AddRange(triangles);
            outputUvs.AddRange(uvs);
        }
    };
}