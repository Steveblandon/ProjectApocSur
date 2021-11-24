namespace Projapocsur.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Projapocsur.World;

    public class BodyHitBoxTests : DefDependantTestsBase
    {
        private static BodyPart arm;
        private static BodyPart leg;
        private static BodyPart head;
        private static BodyPart torso;

        public override void Setup()
        {
            base.Setup();

            arm = new BodyPart(DefNameOf.BodyPart.Human_Arm);
            leg = new BodyPart(DefNameOf.BodyPart.Human_Leg);
            head = new BodyPart(DefNameOf.BodyPart.Human_Head);
            torso = new BodyPart(DefNameOf.BodyPart.Human_Torso);
        }

        [Test]
        public void TestHitChanceFormula()
        {
            // expected factors: portion of bodypart length wthin hitbox, bodypart size proportional to size of hitbox, normalized hitchances to add up to 1

            Assert.AreEqual(torso.FloorOffset, leg.FloorHeight);    // torso is expected to be right above leg for this test to work.

            var range = new Span(leg.FloorHeight / 2, torso.FloorOffset + (torso.Length / 2));     // half of the leg and half of the torso are to be in range for test to work.
            var bodyParts = new List<BodyPart>() { leg, torso };
            var hitbox = new BodyHitBoxWithVisibleChances(range, bodyParts);

            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);
            Assert.AreEqual(range, hitbox.TargetSpan);
            Assert.AreEqual(1, hitbox.HitChances.Sum());

            Assert.AreEqual(2, leg.Def.Size);       // update this if def size ever changes, this is hardcoded here to test algorithm
            Assert.AreEqual(5, torso.Def.Size);
            Assert.AreEqual(leg.Def.Size + torso.Def.Size, hitbox.Size);

            float expectedLegHitChance = .5f * leg.Def.Size / hitbox.Size;     // .5 comes from the length of the body part that is within the hitbox based on the range
            float expectedTorsoHitChance = .5f * torso.Def.Size / hitbox.Size;
            float expectedTotalHitChance = expectedLegHitChance + expectedTorsoHitChance;

            float normalizedLegHitChance = expectedLegHitChance / expectedTotalHitChance;
            float normalizedTorsoHitChance = expectedTorsoHitChance / expectedTotalHitChance;

            Assert.AreNotEqual(0, normalizedLegHitChance);
            Assert.AreNotEqual(0, normalizedTorsoHitChance);
            Assert.AreEqual(Math.Round(normalizedLegHitChance, 5, MidpointRounding.AwayFromZero), Math.Round(hitbox.HitChances[0], 5, MidpointRounding.AwayFromZero));
            Assert.AreEqual(Math.Round(normalizedTorsoHitChance, 5, MidpointRounding.AwayFromZero), Math.Round(hitbox.HitChances[1], 5, MidpointRounding.AwayFromZero));

            //Test shrinking / length modifier (stance change (e.g. crouching) scenario)
            float lengthMultiplier = 0.5f;
            hitbox.Calibrate(bodyParts, bodyPartLengthMultiplier: lengthMultiplier);

            Assert.AreEqual(0, hitbox.HitChances[0]);   // leg is expected to be entirely out of range
            Assert.AreEqual(1, hitbox.HitChances[1]);   // torso, even if not entirely within range is now the only bodypart in range, therefore hitchance should 100%
            Assert.AreEqual(torso, hitbox.GetHitBodyPart(bodyParts).bodyPart);
        }

        [Test]
        public void TestNoHitChance()
        {
            Assert.IsTrue(head.FloorOffset > leg.FloorHeight);      // head is expected to be higher up than leg for this test to work

            var range = new Span(leg.FloorHeight, leg.FloorHeight + 0.01f);
            var bodyParts = new List<BodyPart>() { leg, head };
            var hitbox = new BodyHitBoxWithVisibleChances(range, bodyParts);

            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);
            Assert.AreEqual(range, hitbox.TargetSpan);
            Assert.AreEqual(0, hitbox.HitChances.Sum());
            Assert.IsTrue(hitbox.IsEmpty);
            Assert.IsNull(hitbox.GetHitBodyPart(bodyParts).bodyPart);
        }

        [Test]
        public void Test100PercentHitChance()
        {
            Assert.IsTrue(head.FloorOffset > leg.FloorHeight);      // head is expected to be higher up than leg for this test to work

            var range = new Span(0, leg.FloorHeight);
            var bodyParts = new List<BodyPart>() { leg, head };
            var hitbox = new BodyHitBoxWithVisibleChances(range, bodyParts);

            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);
            Assert.AreEqual(range, hitbox.TargetSpan);
            Assert.AreEqual(1, hitbox.HitChances[0]);       // leg is entirely within the target range of the hitbox, but head is not, so hit chance should be 100% for the leg, even though there are 2 body parts.
            Assert.AreEqual(leg, hitbox.GetHitBodyPart(bodyParts).bodyPart);

            range = new Span(leg.FloorHeight / 2, leg.FloorHeight);
            hitbox = new BodyHitBoxWithVisibleChances(range, bodyParts);
            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);
            Assert.AreEqual(range, hitbox.TargetSpan);
            Assert.AreEqual(1, hitbox.HitChances[0]);       // body part is half within the target range of the hitbox, but is the only one in range, so hit chance should still be 100%.
            Assert.AreEqual(leg, hitbox.GetHitBodyPart(bodyParts).bodyPart);

            Assert.AreEqual(leg.Def.Size, hitbox.Size);     // since leg is the only part within range, hitbox size should be equal to leg's as opposed to leg's + head's
        }

        [Test]
        public void TestRecalibrationAfterBodyPartRemoval()
        {
            Assert.IsTrue(arm.Def.Size < leg.Def.Size);     // test assumes arm is smaller than leg
            Assert.IsTrue(leg.Def.Size < torso.Def.Size);   // test assumes leg is smaller than torso
            
            var bodyParts = new List<BodyPart>() { leg, head };
            bodyParts.Add(arm);
            bodyParts.Add(torso);

            var range = new Span(0, head.FloorHeight);
            var hitbox = new BodyHitBoxWithVisibleChances(range, bodyParts);

            hitbox.GetHitBodyPart(bodyParts);   // this is here to see this working from breakpoint

            float totalHitChances = 0;
            hitbox.HitChances.ForEach(hitChance => totalHitChances += hitChance);

            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);

            var (legIndex, armIndex, torsoIndex, headIndex) = GetBodyPartIndices(bodyParts);

            Assert.AreEqual(DefNameOf.BodyPart.Human_Leg, bodyParts[legIndex].Def.Name);
            Assert.AreEqual(DefNameOf.BodyPart.Human_Arm, bodyParts[armIndex].Def.Name);
            Assert.AreEqual(DefNameOf.BodyPart.Human_Torso, bodyParts[torsoIndex].Def.Name);
            Assert.AreEqual(DefNameOf.BodyPart.Human_Head, bodyParts[headIndex].Def.Name);

            float legHitChance = hitbox.HitChances[legIndex];
            float torsoHitChance = hitbox.HitChances[torsoIndex];
            float armHitChance = hitbox.HitChances[armIndex];

            Assert.IsTrue(legHitChance > armHitChance);     // Leg should have a higher hit chance than arm.
            Assert.AreEqual(bodyParts[legIndex].Def.Size / (float)hitbox.Size, legHitChance); // Since all body parts are entirely in range, hit chance is based of size. 
            Assert.AreEqual(bodyParts[armIndex].Def.Size / (float)hitbox.Size, armHitChance); // Since all body parts are entirely in range, hit chance is based of size. 

            Assert.IsTrue(torsoHitChance > legHitChance);     // Torso should have a higher hit chance than leg.
            Assert.AreEqual(bodyParts[torsoIndex].Def.Size / (float)hitbox.Size, torsoHitChance); // Since all body parts are entirely in range, hit chance is based of size. 

            bodyParts.RemoveAt(1);      // remove head
            hitbox.Calibrate(bodyParts);        // recalibrate due to bodyparts list change.
            (legIndex, armIndex, torsoIndex, headIndex) = GetBodyPartIndices(bodyParts);    // re-determine body parts indices now that there is one removed.

            // hit chances should have increased because a part was removed due to lower total size.
            Assert.IsTrue(hitbox.HitChances[legIndex] > legHitChance);
            Assert.IsTrue(hitbox.HitChances[armIndex] > armHitChance);
            Assert.IsTrue(hitbox.HitChances[torsoIndex] > torsoHitChance);

            Assert.IsTrue(hitbox.HitChances[legIndex] > hitbox.HitChances[armIndex]);     // leg should still have a higher hit chance than arm.
            Assert.IsTrue(hitbox.HitChances[torsoIndex] > hitbox.HitChances[legIndex]); // torso should still have a higher hit chance than leg.

            totalHitChances = 0;
            hitbox.HitChances.ForEach(hitChance => totalHitChances += hitChance);

            Assert.AreEqual(1, totalHitChances);

            hitbox.GetHitBodyPart(bodyParts);   // this is here to see this working from breakpoint

            hitbox = new BodyHitBoxWithVisibleChances(new Span(0, head.FloorHeight / 2), bodyParts);

            hitbox.GetHitBodyPart(bodyParts);   // this is here to see this working from breakpoint
        }

        private static (int legIndex, int armIndex, int torsoIndex, int headIndex) GetBodyPartIndices(List<BodyPart> bodyParts)
        {
            int legIndex = -1;
            int headIndex = -1;
            int armIndex = -1;
            int torsoIndex = -1;

            for (int index = 0; index < bodyParts.Count; index++)
            {
                switch (bodyParts[index].Def.Name)
                {
                    case nameof(DefNameOf.BodyPart.Human_Leg):
                        legIndex = index;
                        break;
                    case nameof(DefNameOf.BodyPart.Human_Head):
                        headIndex = index;
                        break;
                    case nameof(DefNameOf.BodyPart.Human_Arm):
                        armIndex = index;
                        break;
                    case nameof(DefNameOf.BodyPart.Human_Torso):
                        torsoIndex = index;
                        break;
                }
            }

            return (legIndex, armIndex, torsoIndex, headIndex);
        }

        public class BodyHitBoxWithVisibleChances : BodyHitBox
        {
            public BodyHitBoxWithVisibleChances(Span targetSpan, List<BodyPart> bodyParts, float multiplier = 1f) : base(targetSpan, bodyParts, multiplier) {   }

            public IReadOnlyList<float> HitChances { get => this.bodyHitChances; }

            public int Size { get => this.withinRangeBodySize; }
        }
    }
}
