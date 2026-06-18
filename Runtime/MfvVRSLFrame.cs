using UnityEngine;

namespace ManeuverForVRSL
{
    public struct MfvVRSLFrame
    {
        public float pan;
        public float tilt;
        public float intensity;
        public Color color;
        public float coneWidth;
        public float coneLength;
        public int gobo;

        public static MfvVRSLFrame Default()
        {
            return new MfvVRSLFrame
            {
                color = Color.black,
                coneLength = 0.5f,
                gobo = -1
            };
        }
    }
}
