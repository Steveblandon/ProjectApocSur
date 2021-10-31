namespace Projapocsur.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Projapocsur.World;

    public class BodyHitProcessorTests : DefDependantTestsBase
    {
        [Test]
        public void TestConstructor()
        {
            var humanBodyDef = DefinitionFinder.Find<BodyDef>(DefNameOf.Body.Human);
            var bodyParts = new List<BodyPart>(humanBodyDef.BodyParts.Count);
            humanBodyDef.BodyParts.ForEach(defRef => bodyParts.Add(new BodyPart(defRef.RefDefName)));
            var stanceDefs = new List<DefRef<StanceDef>>();
            stanceDefs.Add(new DefRef<StanceDef>(DefNameOf.Stance.Stand));
            stanceDefs.Add(new DefRef<StanceDef>(DefNameOf.Stance.Crouch));
            stanceDefs.Add(new DefRef<StanceDef>(DefNameOf.Stance.Prone));

            // Assert that the right configuration of hitboxes are set up for stance stand, which should have all of the hitboxes as its the tallest stance for human
            float height = 2;
            int hitBoxCount = 3;
            HitProcessorInternal hitProcessor = null;
            Assert_NoExceptionThrownTryCatch(() => hitProcessor = new HitProcessorInternal(stanceDefs, bodyParts, hitBoxCount, height));
            Assert.AreEqual(stanceDefs.Count, hitProcessor.HitBoxesPerStance.Count);
            Assert.IsTrue(hitProcessor.HitBoxesPerStance.TryGetValue(DefNameOf.Stance.Stand, out StanceHitBoxes stanceHitBoxes));
            Assert.IsNotNull(stanceHitBoxes.HitBoxes);
            Assert.IsNotNull(stanceHitBoxes.StanceDefRef?.Def);
            Assert.AreEqual(hitBoxCount, stanceHitBoxes.HitBoxes.Count);
            stanceHitBoxes.HitBoxes.ForEach(hitbox => Assert.IsFalse(hitbox.IsEmpty));
            var ranges = new List<Range>() { stanceHitBoxes.HitBoxes[0].TargetRange };
            
            // Assert that hitbox bounds are different
            for (int index = 1; index < stanceHitBoxes.HitBoxes.Count; index++)
            {
                Range previousRange = stanceHitBoxes.HitBoxes[index - 1].TargetRange;
                Range currentRange = stanceHitBoxes.HitBoxes[index].TargetRange;
                Assert.AreEqual(previousRange.UpperBound, currentRange.LowerBound);
                Assert.IsTrue(previousRange.LowerBound < currentRange.LowerBound);
                Assert.IsTrue(currentRange.UpperBound <= height);
            }

            // Assert crouch is only using 2 hitboxes
            Assert.IsTrue(hitProcessor.HitBoxesPerStance.TryGetValue(DefNameOf.Stance.Crouch, out stanceHitBoxes));
            Assert.IsNotNull(stanceHitBoxes.HitBoxes);
            Assert.IsNotNull(stanceHitBoxes.StanceDefRef?.Def);
            Assert.AreEqual(2, stanceHitBoxes.HitBoxes.Count);

            // Assert prone is only using 1 hitbox
            Assert.IsTrue(hitProcessor.HitBoxesPerStance.TryGetValue(DefNameOf.Stance.Prone, out stanceHitBoxes));
            Assert.IsNotNull(stanceHitBoxes.HitBoxes);
            Assert.IsNotNull(stanceHitBoxes.StanceDefRef?.Def);
            Assert.AreEqual(1, stanceHitBoxes.HitBoxes.Count);
        }

        [Test]
        public void TestProcessingAndRecalibration()
        {
            // Test initialization
            var humanBodyDef = DefinitionFinder.Find<BodyDef>(DefNameOf.Body.Human);
            var bodyParts = new List<BodyPart>(humanBodyDef.BodyParts.Count);
            humanBodyDef.BodyParts.ForEach(defRef => bodyParts.Add(new BodyPart(defRef.RefDefName)));

            float height = 2;
            int hitBoxCount = 3;
            HitProcessorInternal hitProcessor = null;
            Assert_NoExceptionThrownTryCatch(() => hitProcessor = new HitProcessorInternal(humanBodyDef.StanceCapabilities, bodyParts, hitBoxCount, height));
            Assert.AreEqual(humanBodyDef.StanceCapabilities.Count, hitProcessor.HitBoxesPerStance.Count);

            // Test hit processing
            var legs = new List<BodyPart>();
            legs.AddRange(bodyParts, bodyPart => bodyPart.Def.Name == DefNameOf.BodyPart.Human_Leg);
            Assert.AreNotEqual(0, legs.Count);

            var hitInfo = new BodyHitInfo(legs[0].Length / 2, new List<string>(), 5);
            Assert_NoExceptionThrownTryCatch(() => hitProcessor.ProcessHit(hitInfo, bodyParts, humanBodyDef.DefaultStance));

            var damagedLegs = new List<BodyPart>();
            damagedLegs.AddRange(legs, leg => leg.HitPoints.Value < leg.HitPoints.MaxValue);    // since hit was at leg length, we expect one of the legs to be damaged
            Assert.AreNotEqual(0, damagedLegs.Count);

            // Test re-calibrate
            Assert.IsTrue(legs[0].FloorHeight > height / hitBoxCount);      // The height of legs should be greated than height / hitBoxCount which would the upper bound of the lowest range.
            Assert.IsTrue(hitProcessor.HitBoxesPerStance.TryGetValue(DefNameOf.Stance.Stand, out StanceHitBoxes standStanceHitBoxes));
            Assert.IsNotNull(standStanceHitBoxes.HitBoxes);
            Assert.AreEqual(hitBoxCount, standStanceHitBoxes.HitBoxes.Count);

            while (!legs[0].IsDestroyed || !legs[1].IsDestroyed)
            {
                hitProcessor.ProcessHit(hitInfo, bodyParts, humanBodyDef.DefaultStance);
            }

            bodyParts.RemoveAll(bodyPart => bodyPart.Def.Name == DefNameOf.BodyPart.Human_Leg && bodyPart.IsDestroyed);
            Assert.AreEqual(humanBodyDef.BodyParts.Count - 2, bodyParts.Count);
            Assert_NoExceptionThrownTryCatch(() => hitProcessor.ReCalibrate(bodyParts));
            Assert.AreEqual(hitBoxCount - 1, standStanceHitBoxes.HitBoxes.Count);    // since legs were destroyed and should have been the only bodyparts in the lowest range, the hitbox should have been removed once it was empty
        }

        private class HitProcessorInternal : BodyHitProcessor
        {
            public HitProcessorInternal(List<DefRef<StanceDef>> stanceDefRefs, List<BodyPart> bodyParts, int hitBoxCount, float height) 
                : base(stanceDefRefs, bodyParts, hitBoxCount, height)
            {
            }

            public IReadOnlyDictionary<string, StanceHitBoxes> HitBoxesPerStance { get => this.hitBoxesPerStance; }
        }
    }
}
