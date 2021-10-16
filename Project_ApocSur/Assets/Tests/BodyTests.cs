namespace Projapocsur.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Projapocsur.World;
    using Projapocsur.Common;

    public class BodyTests : DefDependantTestsBase
    {
        [Test]
        public void TestConstructor()
        {
            Body body = null;
            Assert_NoExceptionThrownTryCatch(() => body = new Body(DefNameOf.Body.Human));
            
            Assert.AreEqual(body.Def.MaxHitPoints, body.HitPoints.Value);
            Assert.AreEqual(0, body.BloodLoss.Value);
            Assert.AreEqual(0, body.Pain.Value);
            Assert.AreEqual(body.Def.DefaultStance, body.CurrentStance);
            Assert.AreEqual(body.Def.BaseHealingRate, body.HealingRate.Value);
            Assert.IsTrue(body.HealingRate.MaxValue > body.HealingRate.Value);
            Assert.IsTrue(body.HealingRate.MinValue < body.HealingRate.Value);
            Assert.AreEqual(body.Def.BodyParts.Count, body.BodyParts.Count);

            float expectedBodyHeight = 0f;
            body.BodyParts.ForEach(bodyPart => expectedBodyHeight = bodyPart.FloorHeight > expectedBodyHeight ? bodyPart.FloorHeight : expectedBodyHeight);

            Assert.IsTrue(expectedBodyHeight > 0f);
            Assert.AreNotEqual(0f, body.Def.MaxHeightDeviationFactor);

            float expectedHeightMin = expectedBodyHeight * (1 - body.Def.MaxHeightDeviationFactor);
            float expectedHeightMax = expectedBodyHeight * (1 + body.Def.MaxHeightDeviationFactor);

            Assert.IsTrue(expectedHeightMin <= body.Height.Value && body.Height.Value <= expectedHeightMax);
        }

        [Test]
        public void TestTakeDamage()
        {
            Body body = null;
            Assert_NoExceptionThrownTryCatch(() => body = new Body(DefNameOf.Body.Human));
            Assert.AreEqual(body.Def.BodyParts.Count, body.BodyParts.Count);

            var legs = new List<BodyPart>();
            legs.AddRange(body.BodyParts, predicate: bodyPart => bodyPart.Def.Name == DefNameOf.BodyPart.Human_Leg);
            Assert.IsFalse(legs.IsNullOrEmpty());

            float damage = 5;
            float expectedHPAfterDamage = legs[0].HitPoints.Value - damage;
            ICollection<string> injuries = new List<string>() { DefNameOf.Injury.Bruise, DefNameOf.Injury.Fracture };
            var hitInfo = new BodyHitInfo(legs[0].FloorHeight - (legs[0].Length / 2), injuries, 5);
            Assert_NoExceptionThrownTryCatch(() => body.TakeDamage(hitInfo));

            BodyPart damagedLeg = null;
            legs.ForEach(leg => damagedLeg = leg.HitPoints.Value != leg.HitPoints.DefaultValue ? leg : damagedLeg);
            Assert.AreEqual(expectedHPAfterDamage, damagedLeg.HitPoints.Value);

            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());    // must run an update for the new injuries to be processed in the damaged leg

            Assert.AreEqual(injuries.Count, damagedLeg.Injuries.Count);
        }

        [Test]
        public void TestUpdate()
        {
            Body body = null;
            Assert_NoExceptionThrownTryCatch(() => body = new Body(DefNameOf.Body.Human));
            Assert.AreEqual(body.Def.BodyParts.Count, body.BodyParts.Count);
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
        }
    }
}
