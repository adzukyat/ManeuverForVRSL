using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightPropertyの基底Drawer
    /// </summary>
    [CustomPropertyDrawer(typeof(SlmProperty), true)]
    public class SlmPropertyDrawer : SlmTogglePropertyDrawer
    {
        public SlmPropertyDrawer() : base(true) { }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get SlmProperty from SerializedObject
            label.text = property.FindPropertyRelative("propertyName").stringValue;

            //  Draw header
            DrawHeader(position, property, label);
            if (property.isExpanded == false) return;

            var propertyOverride = property.FindPropertyRelative("propertyOverride").boolValue;
            EditorGUI.BeginDisabledGroup(propertyOverride == false);

            var slmProperty = GetValueFromCache(property) as SlmProperty;
            if (slmProperty == null)
            {
                return;
            }

            if (EnsureDrawableValues(property, slmProperty))
            {
                slmProperty = GetValueFromCache(property) as SlmProperty ?? slmProperty;
            }

            DrawToggleController(slmProperty);

            var fields = slmProperty.GetType().GetFields().ToList();
            var clockOverride = fields.Find(x => x.FieldType == typeof(SlmToggleValue<ClockOverride>));
            if (clockOverride != null)
            {
                fields.Remove(clockOverride);
                fields.Insert(0, clockOverride);
            }

            var useIndent = property.serializedObject.targetObject.GetType() != typeof(StageLightTimelineClip);
            if (useIndent) EditorGUI.indentLevel++;
            // EditorGUI.indentLevel++;
            foreach (var f in fields)
            {
                // Draw SlmToggleValue
                var childProperty = property.FindPropertyRelative(f.Name);
                if (childProperty == null)
                {
                    if (EnsureDrawableField(property, slmProperty, f.Name))
                    {
                        childProperty = property.FindPropertyRelative(f.Name);
                    }

                    if (childProperty == null)
                    {
                        continue;
                    }
                }

                EditorGUI.BeginChangeCheck();
                try
                {
                    EditorGUILayout.PropertyField(childProperty, true);
                }
                catch (NullReferenceException e)
                {
                    if (!EnsureDrawableField(property, slmProperty, f.Name))
                    {
                        Debug.LogWarning(slmProperty.propertyName + "." + f.Name + " is null.\n" + e.Message);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            // EditorGUI.indentLevel--;
            if (useIndent) EditorGUI.indentLevel--;

            GUILayout.Space(SlmEditorStyleConst.SlmPropertyBottomMargin);
            EditorGUI.EndDisabledGroup();
        }

        private static bool EnsureDrawableValues(SerializedProperty property, SlmProperty slmProperty)
        {
            var changed = false;
            if (slmProperty is SlmAdditionalProperty additionalProperty)
            {
                changed |= additionalProperty.clockOverride == null ||
                           additionalProperty.clockOverride.value == null ||
                           additionalProperty.clockOverride.value.arrayStaggerValue == null ||
                           additionalProperty.clockOverride.sortOrder == 0;
                additionalProperty.EnsureClockOverride();
            }

            if (slmProperty is LightColorProperty lightColorProperty)
            {
                changed |= lightColorProperty.EnsureValues();
            }

            if (changed)
            {
                WriteBackManagedReference(property, slmProperty);
            }

            return changed;
        }

        private static bool EnsureDrawableField(SerializedProperty property, SlmProperty slmProperty, string fieldName)
        {
            if (slmProperty is LightColorProperty lightColorProperty &&
                fieldName == nameof(LightColorProperty.lightToggleColor))
            {
                lightColorProperty.EnsureValues();
                WriteBackManagedReference(property, slmProperty);
                return true;
            }

            return false;
        }

        private static void WriteBackManagedReference(SerializedProperty property, SlmProperty slmProperty)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                property.managedReferenceValue = slmProperty;
            }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            RemoveValueFromCache(property);
        }

        protected void DrawHeader(Rect position, SerializedProperty property, GUIContent label, bool withToggle = true)
        {
            if (withToggle)
            {
                base.OnGUI(position, property, label);
            }
            else
            {
                base.DrawHeader(position, property, label);
            }
        }

        protected static void DrawToggleController(SlmProperty slmProperty)
        {
            GUILayout.Space(SlmEditorStyleConst.Spacing);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle style = new GUIStyle();
                style.normal.background = null;
                style.fixedWidth = 40;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.gray;
                // GUILayout.FlexibleSpace();
                if (GUILayout.Button("All", style))
                {
                    slmProperty.ToggleOverride(true);
                }

                GUILayout.Space(SlmEditorStyleConst.Spacing);
                if (GUILayout.Button("None", style))
                {
                    slmProperty.ToggleOverride(false);
                    slmProperty.propertyOverride = true;
                }
            }
            GUILayout.Space(SlmEditorStyleConst.Spacing);
        }
    }
}
