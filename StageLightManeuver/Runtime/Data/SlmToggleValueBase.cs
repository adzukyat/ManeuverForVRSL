using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Disable warning CS8632
// CS8632 : '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
// ファイル全体を Nullable コンテキストにしたくないので一旦警告無視
#pragma warning disable 8632

namespace StageLightManeuver
{

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SlmValueAttribute : PropertyAttribute
    {
        public readonly string? name; //TODO CS8632: ここでNull許容型を使う必要があるか確認
        public readonly bool isHidden;
        public SlmValueAttribute(string? name = null, bool isHidden = false)
        {
            this.name = name;
            this.isHidden = isHidden;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class SlmPropertyAttribute : PropertyAttribute
    {
        public readonly bool isRemovable;
        public SlmPropertyAttribute(bool isRemovable = true)
        {
            this.isRemovable = isRemovable;
        }
    }
    
    
    [Serializable]
    public class SlmToggleValueBase
    {
        [SerializeField, SlmValue(isHidden: true)] public bool propertyOverride = false;
        [SlmValue(isHidden: true)] public int sortOrder = 0;
    }

    [Serializable]
    public class SlmToggleValue<T>:SlmToggleValueBase
    {
        public T value;
        public SlmToggleValue(SlmToggleValue<T> slmToggleValue)
        {
            if (slmToggleValue == null)
            {
                propertyOverride = false;
                value = default;
                return;
            }

            propertyOverride = slmToggleValue.propertyOverride;
            sortOrder = slmToggleValue.sortOrder;
            this.value = slmToggleValue.value;
        }

        public SlmToggleValue(T value)
        {
            propertyOverride = false;
            this.value = value;
        }

        public SlmToggleValue()
        {
            propertyOverride = false;
            value = default;
        }
    }


    [Serializable]
    public enum StageLightPropertyType
    {
        None,
        Array,
    }

    [Serializable]
    public class SlmProperty:SlmToggleValueBase
    {
        [SlmValue(isHidden: true)] public StageLightPropertyType propertyType = StageLightPropertyType.None;
        [SlmValue(isHidden: true)] public string propertyName;
        [SlmValue(isHidden: true)] public int propertyOrder = 0;
        [SlmValue(isHidden: true)] public bool isEditable = true;
        public virtual void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
        }

        public virtual void OnProcessFrame(float time, float clipStart, float clipDuration)
        {
            
        }
        
        public virtual void OverwriteProperty(SlmProperty other)
        {
        }
        
        public virtual void InitStageLightFixture(StageLightFixtureBase stageLightFixtureBase)
        {
        }

    }
    
    
    [Serializable]
      public class ClockOverride
    {
        [SlmValue("Loop Type")] public LoopType loopType = LoopType.Loop;
        [SlmValue("Offset Time")] public float offsetTime = 0f;
        [SlmValue("BPM Scale")]public float bpmScale = 1f;
        [SlmValue("Child Stagger")]public float childStagger = 0f;
        public ArrayStaggerValue arrayStaggerValue = new ArrayStaggerValue();

        public ClockOverride()
        {
            loopType = LoopType.Loop;
            bpmScale = 1f;
            offsetTime = 0f;
            childStagger = 0f;
            arrayStaggerValue = new ArrayStaggerValue();
        }
        
        public ClockOverride(ClockOverride clockOverride)
        {
            if (clockOverride == null)
            {
                loopType = LoopType.Loop;
                bpmScale = 1f;
                offsetTime = 0f;
                childStagger = 0f;
                arrayStaggerValue = new ArrayStaggerValue();
                return;
            }

            loopType = clockOverride.loopType;
            bpmScale = clockOverride.bpmScale;
            offsetTime = clockOverride.offsetTime;
            childStagger = clockOverride.childStagger;
            arrayStaggerValue = clockOverride.arrayStaggerValue != null
                ? new ArrayStaggerValue(clockOverride.arrayStaggerValue)
                : new ArrayStaggerValue();
        }
        
       
        
        
    }
      
      public interface IArrayProperty
      {
          // void ResyncArraySize(StageLightUniverse stageLightUniverse);
          public void ResyncArraySize(List<StageLightFixture> stageLights);
      } 
    
    [Serializable]
    public class SlmAdditionalProperty:SlmProperty,IArrayProperty
    {
        
        public SlmToggleValue<ClockOverride> clockOverride = CreateClockOverrideValue();
        
        public SlmAdditionalProperty()
        {
            propertyType = StageLightPropertyType.Array;
            propertyOverride = true;
            EnsureClockOverride();
        }

        protected static SlmToggleValue<ClockOverride> CreateClockOverrideValue()
        {
            return new SlmToggleValue<ClockOverride>(new ClockOverride())
            {
                sortOrder = -999,
            };
        }

        public void EnsureClockOverride()
        {
            if (clockOverride == null)
            {
                clockOverride = CreateClockOverrideValue();
                return;
            }

            if (clockOverride.value == null)
            {
                clockOverride.value = new ClockOverride();
            }

            if (clockOverride.value.arrayStaggerValue == null)
            {
                clockOverride.value.arrayStaggerValue = new ArrayStaggerValue();
            }

            if (clockOverride.sortOrder == 0)
            {
                clockOverride.sortOrder = -999;
            }
        }

        public virtual void ResyncArraySize(List<StageLightFixture> stageLights)
        {
            EnsureClockOverride();
            if(clockOverride.value != null && clockOverride.value.arrayStaggerValue != null)
                clockOverride.value.arrayStaggerValue.ResyncArraySize(stageLights);
        }
    }

    // [Serializable]
    // public class SlmBarLightProperty : SlmAdditionalProperty
    // {
    //     public virtual void ResizeBarLightArray(List<LightChannel> lightChannels)
    //     {
    //     }
    // }
 
    
    // [Serializable]
    // public class SlmArrayProperty:SlmProperty,IArrayProperty
    // {
    //     public SlmToggleValue<ClockOverride> clockOverride = new  SlmToggleValue<ClockOverride>()
    //     {
    //         sortOrder = -999
    //     };
    //     public virtual void ResyncArraySize(StageLightUniverse stageLightUniverse)
    //     {
    //         
    //     }
    // }
    //
   
    
    
    [Serializable]
    public class ClipProperty
    {
        public float clipStartTime;
        public float clipEndTime;
        
        public ClipProperty()
        {
            clipStartTime = 0f;
            clipEndTime = 0f;
        }

        public ClipProperty(ClipProperty other)
        {
            clipStartTime = other.clipStartTime;
            clipEndTime = other.clipEndTime;
        }
    }







}
