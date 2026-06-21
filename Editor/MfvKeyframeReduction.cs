using System.Collections.Generic;
using UnityEngine;

namespace ManeuverForVRC.Editor
{
    public static class MfvKeyframeReduction
    {
        public static void Reduce(IReadOnlyList<float> times, IReadOnlyList<float> values, float tolerance, List<float> reducedTimes, List<float> reducedValues)
        {
            reducedTimes.Clear();
            reducedValues.Clear();

            if (times == null || values == null || times.Count != values.Count || times.Count == 0)
            {
                return;
            }

            if (times.Count <= 2 || tolerance <= 0f)
            {
                CopyAll(times, values, reducedTimes, reducedValues);
                return;
            }

            var keep = new bool[times.Count];
            keep[0] = true;
            keep[times.Count - 1] = true;
            ReduceSegment(times, values, tolerance, 0, times.Count - 1, keep);

            for (var i = 0; i < times.Count; i++)
            {
                if (!keep[i])
                {
                    continue;
                }

                reducedTimes.Add(times[i]);
                reducedValues.Add(values[i]);
            }
        }

        private static void CopyAll(IReadOnlyList<float> times, IReadOnlyList<float> values, List<float> reducedTimes, List<float> reducedValues)
        {
            for (var i = 0; i < times.Count; i++)
            {
                reducedTimes.Add(times[i]);
                reducedValues.Add(values[i]);
            }
        }

        private static void ReduceSegment(IReadOnlyList<float> times, IReadOnlyList<float> values, float tolerance, int start, int end, bool[] keep)
        {
            if (end - start <= 1)
            {
                return;
            }

            var maxError = 0f;
            var maxIndex = -1;
            var startTime = times[start];
            var endTime = times[end];
            var startValue = values[start];
            var endValue = values[end];

            for (var i = start + 1; i < end; i++)
            {
                var ratio = endTime > startTime ? Mathf.InverseLerp(startTime, endTime, times[i]) : 0f;
                var predicted = Mathf.Lerp(startValue, endValue, ratio);
                var error = Mathf.Abs(values[i] - predicted);
                if (error > maxError)
                {
                    maxError = error;
                    maxIndex = i;
                }
            }

            if (maxError <= tolerance || maxIndex < 0)
            {
                return;
            }

            keep[maxIndex] = true;
            ReduceSegment(times, values, tolerance, start, maxIndex, keep);
            ReduceSegment(times, values, tolerance, maxIndex, end, keep);
        }
    }
}
