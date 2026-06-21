using UnityEngine;

namespace ManeuverForVRC
{
    public class MfvBakedShowAsset : ScriptableObject
    {
        public float duration;
        public float sampleRate;
        public string[] fixtureNames = new string[0];

        public int[] trackFixture = new int[0];
        public int[] trackProperty = new int[0];
        public int[] keyStart = new int[0];
        public int[] keyCount = new int[0];
        public float[] keyTimes = new float[0];
        public float[] keyValues = new float[0];

        public int[] eventFixture = new int[0];
        public int[] eventProperty = new int[0];
        public int[] eventStart = new int[0];
        public int[] eventCount = new int[0];
        public float[] eventTimes = new float[0];
        public int[] eventValues = new int[0];

        public int ContinuousTrackCount => trackFixture != null ? trackFixture.Length : 0;
        public int EventTrackCount => eventFixture != null ? eventFixture.Length : 0;
        public int FixtureCount => fixtureNames != null ? fixtureNames.Length : 0;

        public void CopyTo(MfvVRSLTimelinePlayer player)
        {
            player.trackFixture = trackFixture;
            player.trackProperty = trackProperty;
            player.keyStart = keyStart;
            player.keyCount = keyCount;
            player.keyTimes = keyTimes;
            player.keyValues = keyValues;
            player.eventFixture = eventFixture;
            player.eventProperty = eventProperty;
            player.eventStart = eventStart;
            player.eventCount = eventCount;
            player.eventTimes = eventTimes;
            player.eventValues = eventValues;
        }
    }
}
