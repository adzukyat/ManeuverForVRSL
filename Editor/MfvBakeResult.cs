using ManeuverForVRSL;
using UnityEngine.Timeline;
using VRSL;

namespace ManeuverForVRSL.Editor
{
    public sealed class MfvBakeResult
    {
        public MfvBakedShowAsset bakedAsset;
        public TimelineAsset uploadTimeline;
        public VRStageLighting_DMX_Static[] fixtures;
    }
}
