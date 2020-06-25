using System;

namespace GameServer
{
    class Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static explicit operator byte(Vector3 v)
        {
            byte[] tmp = new byte[3];
            tmp[0] = (byte)v.x;
            tmp[1] = (byte)v.y;
            tmp[2] = (byte)v.z;

            return 0;
        }
    }
}