using ManeuverForVRC;
using UnityEngine.Timeline;
using VRSL;

namespace ManeuverForVRC.Editor
{
    public sealed class MfvBakeResult
    {
        public MfvBakedShowAsset bakedAsset;
        public TimelineAsset uploadTimeline;
        public VRStageLighting_DMX_Static[] fixtures;
    }
}
