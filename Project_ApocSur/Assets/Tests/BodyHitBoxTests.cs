namespace Projapocsur.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Projapocsur.Common;
    using Projapocsur.World;

    public class BodyHitBoxTests : TestsBase
    {
        public override void Setup()
        {
            base.Setup();

            AssertNullExceptionTryCatch(() => DefinitionFinder.Init(TestDataPath));
        }

        [Test]
        public void TestUsage()
        {
            var arm = new BodyPart(DefNameOf.BodyPart.Human_Arm);
            var leg = new BodyPart(DefNameOf.BodyPart.Human_Leg);
            var head = new BodyPart(DefNameOf.BodyPart.Human_Head);
            var torso = new BodyPart(DefNameOf.BodyPart.Human_Torso);
            var bodyParts = new List<BodyPart>() { leg, head };
            float legHeightFromFloor = leg.Def.FloorOffset + leg.Def.Height;

            var hitbox = new BodyHitBoxWithVisibleChances(new Range(legHeightFromFloor, legHeightFromFloor + 1), bodyParts);

            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);
            Assert.AreEqual(0, hitbox.HitChances[0]);       // leg is not within the target range of the hitbox so hit chance should be zero.
            Assert.AreEqual(head, hitbox.GetHitBodyPart(bodyParts));    // head is the only thing within range.

            hitbox = new BodyHitBoxWithVisibleChances(new Range(0, legHeightFromFloor), bodyParts);

            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);
            Assert.AreEqual(1, hitbox.HitChances[0]);       // leg is entirely within the target range of the hitbox, but head is not, so hit chance should be 100% for the leg, even though there are 2 body parts.
            Assert.AreEqual(leg, hitbox.GetHitBodyPart(bodyParts));

            hitbox = new BodyHitBoxWithVisibleChances(new Range(legHeightFromFloor / 2, legHeightFromFloor), bodyParts);

            Assert.AreEqual(bodyParts.Count, hitbox.HitChances.Count);
            Assert.AreEqual(1, hitbox.HitChances[0]);       // body part is half within the target range of the hitbox, but is the only one in range, so hit chance should still be 100%.
            Assert.AreEqual(leg, hitbox.GetHitBodyPart(bodyParts));

            Assert.IsTrue(arm.Def.Size < leg.Def.Size);
            Assert.IsTrue(leg.Def.Size < torso.Def.Size);

            bodyParts.Add(arm);
            bodyParts.Add(torso);

            float totalHeight = head.Def.FloorOffset + head.Def.Height;
            int totalSize = 0;

            bodyParts.ForEach(bodyPart => totalSize += bodyPart.Def.Size);

            hitbox = new BodyHitBoxWithVisibleChances(new Range(0, totalHeight), bodyParts);

            hitbox.GetHitBodyPart(bodyParts);   // run this here to see this working from breakpoint

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
            Assert.AreEqual(bodyParts[legIndex].Def.Size / (float)totalSize, legHitChance); // Since all body parts are entirely in range, hit chance is based of size. 
            Assert.AreEqual(bodyParts[armIndex].Def.Size / (float)totalSize, armHitChance); // Since all body parts are entirely in range, hit chance is based of size. 

            Assert.IsTrue(torsoHitChance > legHitChance);     // Torso should have a higher hit chance than leg.
            Assert.AreEqual(bodyParts[torsoIndex].Def.Size / (float)totalSize, torsoHitChance); // Since all body parts are entirely in range, hit chance is based of size. 

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

            hitbox.GetHitBodyPart(bodyParts);   // run this here to see this working from breakpoint

            hitbox = new BodyHitBoxWithVisibleChances(new Range(0, totalHeight / 2), bodyParts);

            hitbox.GetHitBodyPart(bodyParts);   // run this here to see this working from breakpoint
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
            public BodyHitBoxWithVisibleChances(Range targetRange, List<BodyPart> bodyParts) : base(targetRange, bodyParts) {   }

            public IReadOnlyList<float> HitChances { get => this.bodyHitChances; }
        }
    }
}
