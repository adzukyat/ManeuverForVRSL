using UnityEngine;

namespace ManeuverForVRSL
{
    public class MfvBakeSettings : ScriptableObject
    {
        public const float DefaultSampleRate = 120f;
        public const float DefaultPanTiltTolerance = 0.5f;
        public const float DefaultIntensityTolerance = 0.005f;
        public const float DefaultColorTolerance = 2f / 255f;
        public const float DefaultConeTolerance = 0.01f;

        [Min(1f)] public float internalSampleRate = DefaultSampleRate;
        [Min(0f)] public float panTiltTolerance = DefaultPanTiltTolerance;
        [Min(0f)] public float intensityTolerance = DefaultIntensityTolerance;
        [Min(0f)] public float colorTolerance = DefaultColorTolerance;
        [Min(0f)] public float coneTolerance = DefaultConeTolerance;

        public static MfvBakeSettings CreateDefault()
        {
            var settings = CreateInstance<MfvBakeSettings>();
            settings.internalSampleRate = DefaultSampleRate;
            settings.panTiltTolerance = DefaultPanTiltTolerance;
            settings.intensityTolerance = DefaultIntensityTolerance;
            settings.colorTolerance = DefaultColorTolerance;
            settings.coneTolerance = DefaultConeTolerance;
            return settings;
        }

        public float GetTolerance(int propertyId)
        {
            switch (propertyId)
            {
                case MfvVRSLPropertyId.Pan:
                case MfvVRSLPropertyId.Tilt:
                    return panTiltTolerance;
                case MfvVRSLPropertyId.Intensity:
                    return intensityTolerance;
                case MfvVRSLPropertyId.ColorR:
                case MfvVRSLPropertyId.ColorG:
                case MfvVRSLPropertyId.ColorB:
                    return colorTolerance;
                case MfvVRSLPropertyId.ConeWidth:
                case MfvVRSLPropertyId.ConeLength:
                    return coneTolerance;
                default:
                    return 0f;
            }
        }
    }
}
