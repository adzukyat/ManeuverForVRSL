using System;
using StageLightManeuver;
using UnityEngine;

namespace ManeuverForVRSL
{
    [Serializable]
    public class MfvVRSLGoboProperty : SlmAdditionalProperty
    {
        [SlmValue("VRSL Gobo Index")]
        public SlmToggleValue<int> goboIndex;

        public MfvVRSLGoboProperty()
        {
            propertyName = "VRSL Gobo";
            propertyOverride = true;
            propertyOrder = 1000;
            goboIndex = new SlmToggleValue<int> { value = 1, propertyOverride = true };
        }

        public MfvVRSLGoboProperty(MfvVRSLGoboProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            propertyOrder = other.propertyOrder;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            goboIndex = new SlmToggleValue<int>(other.goboIndex);
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            goboIndex.propertyOverride = toggle;
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is MfvVRSLGoboProperty goboProperty && goboProperty.propertyOverride)
            {
                if (goboProperty.goboIndex.propertyOverride)
                {
                    goboIndex.value = goboProperty.goboIndex.value;
                }

                if (goboProperty.clockOverride.propertyOverride)
                {
                    clockOverride = new SlmToggleValue<ClockOverride>(goboProperty.clockOverride);
                }
            }
        }

        public int GetClampedGoboIndex()
        {
            return Mathf.Clamp(goboIndex.value, 1, 8);
        }
    }
}
