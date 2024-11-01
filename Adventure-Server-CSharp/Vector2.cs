using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public class Vector2
    {
        public Vector2()
        {
            x = 0;
            y = 0;
        }

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(Vector2 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
        }

        public static Vector2 zero = new Vector2(0, 0);
        public static Vector2 one = new Vector2(1, 1);

        public readonly static Vector2 right = new Vector2(1, 0);
        public readonly static Vector2 left = new Vector2(-1, 0);
        public readonly static Vector2 up = new Vector2(0, 1);
        public readonly static Vector2 down = new Vector2(0, -1);



        public static Vector2 operator +(Vector2 one, Vector2 two)
        {
            return new Vector2(one.x + two.x, one.y + two.y);
        }

        public static Vector2 operator -(Vector2 one, Vector2 two)
        {
            return new Vector2(one.x - two.x, one.y - two.y);
        }

        public static Vector2 operator *(Vector2 one, int two)
        {
            return new Vector2(one.x * two, one.y * two);
        }


        public static bool operator ==(Vector2 one, Vector2 two)
        {
            return ((one.x == two.x) && (one.y == two.y));
        }

        public static bool operator !=(Vector2 one, Vector2 two)
        {
            return ((one.x != two.x) || (one.y != two.y));
        }


        public string ToStringVector()
        {
            return "X: " + this.x + " , Y: " + this.y;
        }

        public override bool Equals(object obj)
        {
            var vector = obj as Vector2;
            return vector != null &&
                   x == vector.x &&
                   y == vector.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public int x, y;
    }
}
