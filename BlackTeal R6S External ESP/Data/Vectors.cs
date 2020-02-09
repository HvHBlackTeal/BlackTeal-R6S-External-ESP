using System;

namespace BlackTeal_R6S_External_ESP.Data
{
    public class Vector2
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public class Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public float Dot(Vector3 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public float Distance(float f, Vector3 v)
        {
            return (float)Math.Sqrt((float)Math.Pow(v.x - x, 2.0) + (float)Math.Pow(v.y - y, 2.0) + (float)Math.Pow(v.z - z, 2.0));
        }
        public static float Get3DDistance(Vector3 to, Vector3 from)
        {
            return (float)Math.Sqrt(
                          ((to.x - from.x) * (to.x - from.x)) +
                          ((to.y - from.y) * (to.y - from.y)) +
                          ((to.z - from.z) * (to.z - from.z)));
        }
    }
}
