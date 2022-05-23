using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class ByteVector3
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

}
