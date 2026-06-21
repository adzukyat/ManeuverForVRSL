using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightColorProperty:SlmAdditionalProperty
    {
        [SlmValue("Color")]public SlmToggleValue<Gradient> lightToggleColor;// = new StageLightProperty<float>(){value = 1f};
        public LightColorProperty()
        {
            propertyOverride = true;
            propertyName = "Light Color";
            EnsureValues();
        }

        public bool EnsureValues()
        {
            var changed = false;
            EnsureClockOverride();
            if (lightToggleColor == null)
            {
                lightToggleColor = new SlmToggleValue<Gradient>();
                changed = true;
            }

            if (lightToggleColor.value == null)
            {
                lightToggleColor.value = new Gradient();
                changed = true;
            }

            return changed;
        }
        
        public override void ToggleOverride(bool toggle)
        {
            clockOverride.propertyOverride = toggle;
            lightToggleColor.propertyOverride = toggle;
            propertyOverride = toggle;
        }
        
        public LightColorProperty( LightColorProperty other )
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            EnsureClockOverride();
            lightToggleColor = new SlmToggleValue<Gradient>()
            {
                propertyOverride = other.lightToggleColor != null && other.lightToggleColor.propertyOverride,
                value = SlmUtility.CopyGradient(other.lightToggleColor?.value ?? new Gradient())
            };
            EnsureValues();
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            var otherProperty = other as LightColorProperty;
            if (otherProperty == null) return;
            EnsureValues();
            otherProperty.EnsureValues();
            if (other.propertyOverride)
            {
                if(otherProperty.lightToggleColor.propertyOverride) lightToggleColor.value = SlmUtility.CopyGradient(otherProperty.lightToggleColor.value);
                if(otherProperty.clockOverride.propertyOverride) clockOverride= new SlmToggleValue<ClockOverride>(otherProperty.clockOverride);
            }
        }
    }
}
