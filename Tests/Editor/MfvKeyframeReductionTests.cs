using System.Collections.Generic;
using ManeuverForVRSL.Editor;
using NUnit.Framework;

namespace ManeuverForVRSL.Tests
{
    public class MfvKeyframeReductionTests
    {
        [Test]
        public void Reduce_RemovesLinearInteriorKeys()
        {
            var times = new List<float> { 0f, 0.5f, 1f };
            var values = new List<float> { 0f, 0.5f, 1f };
            var reducedTimes = new List<float>();
            var reducedValues = new List<float>();

            MfvKeyframeReduction.Reduce(times, values, 0.001f, reducedTimes, reducedValues);

            Assert.That(reducedTimes, Is.EqualTo(new[] { 0f, 1f }));
            Assert.That(reducedValues, Is.EqualTo(new[] { 0f, 1f }));
        }

        [Test]
        public void Reduce_KeepsKeysNeededForTolerance()
        {
            var times = new List<float> { 0f, 0.5f, 1f };
            var values = new List<float> { 0f, 1f, 0f };
            var reducedTimes = new List<float>();
            var reducedValues = new List<float>();

            MfvKeyframeReduction.Reduce(times, values, 0.01f, reducedTimes, reducedValues);

            Assert.That(reducedTimes, Is.EqualTo(new[] { 0f, 0.5f, 1f }));
            Assert.That(reducedValues, Is.EqualTo(new[] { 0f, 1f, 0f }));
        }
    }
}
