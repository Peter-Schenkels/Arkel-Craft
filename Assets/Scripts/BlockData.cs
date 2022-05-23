using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public enum BlockType
    {
        GRASS,
        DIRT,
        AIR,
        STONE
    }

    public class BlockData
    {
        public ByteVector3 position;
        public BlockType type;

        public BlockData(ByteVector3 position, BlockType type)
        {
            this.position = position;
            this.type = type;
        }
    }
}
