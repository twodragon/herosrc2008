using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public class Vector3
    {
        public Vector3()
        {
            x = 0;
            y = 0;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Vector3(Vector3 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
            this.z = vec.z;
        }

        public readonly static Vector3 zero = new Vector3(0, 0);
        public readonly static Vector3 one = new Vector3(1, 1);

        public readonly static Vector3 right = new Vector3(1, 0);
        public readonly static Vector3 left = new Vector3(-1, 0);
        public readonly static Vector3 up = new Vector3(0, 1);
        public readonly static Vector3 down = new Vector3(0, -1);



        public static Vector3 operator +(Vector3 one, Vector3 two)
        {
            return new Vector3(one.x + two.x, one.y + two.y, one.z + two.z);
        }

        public static Vector3 operator -(Vector3 one, Vector3 two)
        {
            return new Vector3(one.x - two.x, one.y - two.y, one.z - two.z);
        }

        public static Vector3 operator *(Vector3 one, float two)
        {
            return new Vector3(one.x * two, one.y * two, one.z * two);
        }


        //public static bool operator ==(Vector3 one, Vector3 two)
        //{
        //    return ((one.x == two.x) && (one.y == two.y) && (one.z == two.z));
        //}

        //public static bool operator !=(Vector3 one, Vector3 two)
        //{
        //    return ((one.x != two.x) || (one.y != two.y) || (one.z != two.z));
        //}


        

        public static Vector3 RandomUnitSphere
        {
            get
            {
                Random random = Randomf.random;

                double angle = 2.0 * Math.PI * random.NextDouble();

                var x = Math.Cos(angle);
                var y = Math.Sin(angle);

                return new Vector3((float)x, (float)y, 0);
            }
        }


        public Vector3 normalize
        {
            get
            {
                float sqrMag = sqrMagnitude;
                return new Vector3(this.x / sqrMag, this.y / sqrMag, this.z / sqrMag);
            }
        }

        public float magnitude
        {
            get
            {
                return (this.x * this.x) + (this.y * this.y) + (this.z * this.z);
            }
        }

        public float sqrMagnitude
        {
            get
            {
                return (float)Math.Sqrt((this.x * this.x) + (this.y * this.y) + (this.z * this.z));
            }
        }

        public string ToStringVector()
        {
            return "X: " + this.x + " , Y: " + this.y + " , Z: " + this.z;
        }

        public string ToByteString()
        {
            return (BitConverter.ToString(BitConverter.GetBytes(x)) + BitConverter.ToString(BitConverter.GetBytes(y)) + BitConverter.ToString(BitConverter.GetBytes(z)));
        }

        public byte[] ToBytes()
        {
            return Functions.FromHex(BitConverter.ToString(BitConverter.GetBytes(x)) + BitConverter.ToString(BitConverter.GetBytes(y)) + BitConverter.ToString(BitConverter.GetBytes(z)));
        }

        public override bool Equals(object obj)
        {
            var vector = obj as Vector3;
            return vector != null &&
                   x == vector.x &&
                   y == vector.y &&
                   z == vector.z;
        }

        public override int GetHashCode()
        {
            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        public float x, y, z;
    }
}

