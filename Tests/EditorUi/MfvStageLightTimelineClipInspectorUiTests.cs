using System.Collections;
using NUnit.Framework;
using StageLightManeuver;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Timeline;

namespace ManeuverForVRSL.Tests
{
    public class MfvStageLightTimelineClipInspectorUiTests
    {
        private const string SettingsFolderPath = "Assets/StageLightManeuver";
        private const string SettingsAssetPath = SettingsFolderPath + "/StageLightManeuverSettings.asset";

        [UnityTest]
        public IEnumerator FreshTimelineClipInspector_DoesNotLogErrors()
        {
            var hadSettingsFolder = AssetDatabase.IsValidFolder(SettingsFolderPath);
            var hadSettingsAsset =
                AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(SettingsAssetPath) != null;
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();
            var fixtureObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEditor.Editor inspector = null;
            InspectorGuiSmokeWindow window = null;
            try
            {
                var track = timeline.CreateTrack<StageLightTimelineTrack>(null, "Fresh SLM");
                var clip = track.CreateClip<StageLightTimelineClip>();
                var slmClip = (StageLightTimelineClip)clip.asset;
                slmClip.behaviour.stageLightQueueData.stageLightProperties.Add(new LightProperty());
                slmClip.behaviour.stageLightQueueData.stageLightProperties.Add(new LightIntensityProperty());
                slmClip.behaviour.stageLightQueueData.stageLightProperties.Add(new LightColorProperty());

                var stageLightFixture = fixtureObject.AddComponent<StageLightFixture>();
                fixtureObject.AddComponent<MfvVRSLFixtureChannel>();
                stageLightFixture.Init();

                inspector = UnityEditor.Editor.CreateEditor(slmClip);
                Assert.NotNull(inspector, "Unity did not create a custom inspector for StageLightTimelineClip.");

                window = ScriptableObject.CreateInstance<InspectorGuiSmokeWindow>();
                window.hideFlags = HideFlags.HideAndDontSave;
                window.Inspector = inspector;
                window.ShowUtility();
                window.Repaint();

                for (var i = 0; i < 10 && !window.WasDrawn; i++)
                {
                    window.Repaint();
                    yield return null;
                }

                Assert.IsTrue(window.WasDrawn, "StageLightTimelineClip inspector was not drawn in the smoke window.");
                Assert.IsNull(window.Exception, window.Exception?.ToString());
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                    Object.DestroyImmediate(window);
                }

                if (inspector != null)
                {
                    Object.DestroyImmediate(inspector);
                }

                Object.DestroyImmediate(timeline);
                Object.DestroyImmediate(fixtureObject);

                if (!hadSettingsAsset)
                {
                    AssetDatabase.DeleteAsset(SettingsAssetPath);
                }

                if (!hadSettingsFolder && AssetDatabase.IsValidFolder(SettingsFolderPath))
                {
                    AssetDatabase.DeleteAsset(SettingsFolderPath);
                }
            }
        }

        private sealed class InspectorGuiSmokeWindow : EditorWindow
        {
            public UnityEditor.Editor Inspector;
            public System.Exception Exception;
            public bool WasDrawn;

            private void OnGUI()
            {
                WasDrawn = true;
                try
                {
                    Inspector?.OnInspectorGUI();
                }
                catch (System.Exception exception)
                {
                    Exception = exception;
                }
            }
        }
    }
}
