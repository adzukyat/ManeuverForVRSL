using ManeuverForVRSL;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;

namespace ManeuverForVRSL.Editor
{
    public static class MfvBakeMenu
    {
        private const string PlayerName = "ManeuverForVRSL Baked Player";

        [MenuItem("ManeuverForVRSL/Bake Selected Director")]
        public static void BakeSelectedDirector()
        {
            var director = GetSelectedDirector();
            if (director == null)
            {
                Debug.LogError("[ManeuverForVRSL] Select a GameObject with a PlayableDirector, or select a PlayableDirector component.");
                return;
            }

            var settings = MfvBakeSettings.CreateDefault();
            var result = MfvBakeUtility.Bake(director, settings);
            Object.DestroyImmediate(settings);
            if (result == null)
            {
                return;
            }

            var player = GetOrCreatePlayer(director);
            MfvBakeUtility.ConfigurePlayer(player, director, result);
            EditorSceneManager.MarkSceneDirty(director.gameObject.scene);
            Debug.Log($"[ManeuverForVRSL] Configured runtime player '{player.name}'. Upload timeline: {AssetDatabase.GetAssetPath(result.uploadTimeline)}", player);
        }

        [MenuItem("ManeuverForVRSL/Bake Selected Director", true)]
        public static bool ValidateBakeSelectedDirector()
        {
            return GetSelectedDirector() != null;
        }

        private static PlayableDirector GetSelectedDirector()
        {
            if (Selection.activeObject is PlayableDirector selectedDirector)
            {
                return selectedDirector;
            }

            if (Selection.activeGameObject != null)
            {
                return Selection.activeGameObject.GetComponent<PlayableDirector>();
            }

            return null;
        }

        private static MfvVRSLTimelinePlayer GetOrCreatePlayer(PlayableDirector director)
        {
            var child = director.transform.Find(PlayerName);
            GameObject playerObject;
            if (child != null)
            {
                playerObject = child.gameObject;
            }
            else
            {
                playerObject = new GameObject(PlayerName);
                Undo.RegisterCreatedObjectUndo(playerObject, "Create ManeuverForVRSL Player");
                playerObject.transform.SetParent(director.transform, false);
            }

            var player = playerObject.GetComponent<MfvVRSLTimelinePlayer>();
            if (player == null)
            {
                player = Undo.AddComponent<MfvVRSLTimelinePlayer>(playerObject);
            }

            return player;
        }
    }
}
