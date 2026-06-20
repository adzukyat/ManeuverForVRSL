using UnityEngine;

namespace StageLightManeuver
{
    [AddComponentMenu("")]
    public sealed class DecalProjector : MonoBehaviour
    {
        public Vector3 size = Vector3.one;
        public Vector3 pivot;
        public float fadeFactor = 1f;
        public Material material;
    }

    [AddComponentMenu("")]
    public sealed class LensFlareComponentSRP : MonoBehaviour
    {
        public float intensity;
        public float scale;
    }
}
