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

            Assert.AreEqual(0, body.BloodLoss.Value);
            Assert.AreEqual(0, body.Pain.Value);
            Assert.AreEqual(body.Def.DefaultStance, body.CurrentStance);
            Assert.AreEqual(body.Def.BaseHealingRate, body.HealingRate.Value);
            Assert.IsTrue(body.HealingRate.MaxValue > body.HealingRate.Value);
            Assert.IsTrue(body.HealingRate.MinValue < body.HealingRate.Value);
            Assert.AreEqual(body.Def.BodyParts.Count, body.BodyParts.Count);

            float expectedBodyHeight = 0f;
            float expectedHitPoints = 0f;
            float expectedBodySize = 0f;

            foreach (var bodyPart in body.BodyParts)
            {
                expectedBodyHeight = bodyPart.FloorHeight > expectedBodyHeight ? bodyPart.FloorHeight : expectedBodyHeight;
                expectedHitPoints += bodyPart.HitPoints.Value;
                expectedBodySize += bodyPart.Def.Size;
            }

            float maxBodyBleedRate = Config.DefaultBleedingRateOnLimbLoss * expectedBodySize;
            Assert.AreEqual(0, body.BleedingRate.Value);
            body.BleedingRate.ApplyIncrease(maxBodyBleedRate);
            Assert.AreEqual(maxBodyBleedRate, body.BleedingRate.Value);

            Assert.AreEqual(expectedHitPoints, body.HitPoints.Value);
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

            // test bleeding to death generic scenario 
            Assert.AreEqual(0, body.BleedingRate.Value);
            Assert.IsFalse(body.IsDestroyed);
            body.BleedingRate.ApplyIncrease(body.BleedingRate.MaxValue);
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());    // due to healing it should take about 2 updates for body to bleed to death
            Assert.IsTrue(body.IsDestroyed);
            float currentBleedRate = body.BleedingRate.Value;
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());    // update will be aborted internally because body is destroyed
            Assert.AreEqual(currentBleedRate, body.BleedingRate.Value); // bleeding rate was not reduced due to healing because update was aborted internally
        }

        [Test]
        public void TestHealedNonBleedingInjury()
        {
            Body body = null;
            Assert_NoExceptionThrownTryCatch(() => body = new Body(DefNameOf.Body.Human));

            var legs = new List<BodyPart>();
            legs.AddRange(body.BodyParts, predicate: bodyPart => bodyPart.Def.Name == DefNameOf.BodyPart.Human_Leg);
            Assert.IsFalse(legs.IsNullOrEmpty());

            float damage = legs[0].Def.MaxHitpoints / 2;
            float expectedHPAfterDamage = legs[0].HitPoints.Value - damage;
            ICollection<string> injuries = new List<string>() { DefNameOf.Injury.Bruise };
            var hitInfo = new BodyHitInfo(legs[0].FloorHeight - (legs[0].Length / 2), injuries, damage);
            Assert_NoExceptionThrownTryCatch(() => body.TakeDamage(hitInfo));

            BodyPart damagedLeg = null;
            legs.ForEach(leg => damagedLeg = leg.HitPoints.Value != leg.HitPoints.DefaultValue ? leg : damagedLeg);
            Assert.IsTrue(damagedLeg.IsDamaged);
            Assert.AreEqual(expectedHPAfterDamage, damagedLeg.HitPoints.Value);

            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());    // must run an update for the new injuries to be processed in the damaged leg

            Assert.AreEqual(injuries.Count, damagedLeg.Injuries.Count);
            var injuryEnumerator = damagedLeg.Injuries.GetEnumerator();
            injuryEnumerator.MoveNext();
            Injury bruise = injuryEnumerator.Current;
            Assert.IsNotNull(bruise);
            Assert.IsTrue(damagedLeg.IsDamaged);    // should still be damaged

            //boost healing rate to heal injury in 3 more updates
            float healingRateBonus = Config.DefaultInjuryHealThreshold / body.HealingRate.Value / 3 / bruise.HealingRateMultiplier;
            body.HealingRate.ApplyIncrease(healingRateBonus);

            Assert.IsTrue(body.HealingRate.Value * 3 + bruise.HealedAmount > Config.DefaultInjuryHealThreshold);

            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert.IsTrue(damagedLeg.IsDamaged);
            Assert.IsFalse(bruise.IsHealed);
            Assert.IsFalse(damagedLeg.HitPoints.IsAtMaxValue());
            Assert.IsFalse(body.HitPoints.IsAtMaxValue());

            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert.IsTrue(bruise.IsHealed);

            // at this point, bruise is healed, but since hitpoint recovery was stunted, the leg hasn't fully recovered, however now it's free to do so
            Assert.IsTrue(damagedLeg.IsDamaged);
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert.IsFalse(damagedLeg.IsDamaged);
            
            Assert.IsTrue(damagedLeg.HitPoints.IsAtMaxValue());
            Assert.IsTrue(body.HitPoints.IsAtMaxValue());
        }

        [Test]
        public void TestDestroyedBodyPartBodyBleeding()
        {
            Body body = null;
            Assert_NoExceptionThrownTryCatch(() => body = new Body(DefNameOf.Body.Human));

            var legs = new List<BodyPart>();
            legs.AddRange(body.BodyParts, predicate: bodyPart => bodyPart.Def.Name == DefNameOf.BodyPart.Human_Leg);
            Assert.IsFalse(legs.IsNullOrEmpty());

            float damage = legs[0].Def.MaxHitpoints / 2;
            ICollection<string> injuries = new List<string>() { DefNameOf.Injury.Bruise };
            var hitInfo = new BodyHitInfo(legs[0].FloorHeight - (legs[0].Length / 2), injuries, damage);

            Assert.AreEqual(0, body.BleedingRate.Value);
            Assert.AreEqual(0, body.BloodLoss.Value);

            while (body.BodyParts.Count == body.Def.BodyParts.Count)
            {
                Assert_NoExceptionThrownTryCatch(() => body.TakeDamage(hitInfo));
                Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            }

            BodyPart damagedLeg = null;
            legs.ForEach(leg => damagedLeg = leg.HitPoints.Value != leg.HitPoints.DefaultValue ? leg : damagedLeg);

            while (!damagedLeg.IsDestroyed)
            {
                Assert_NoExceptionThrownTryCatch(() => body.TakeDamage(hitInfo));
                Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            }

            Assert.IsTrue(damagedLeg.IsDamaged);
            Assert.IsTrue(damagedLeg.IsDestroyed);
            Assert.AreEqual(0, damagedLeg.HitPoints.Value);
            Assert.IsTrue(body.BleedingRate.Value > 0);
            Assert.IsTrue(body.BloodLoss.Value > 0);
        }

        [Test]
        public void TestVitalPartDamageAmplificationAndBodyDestructionOnLostVitalPart()
        {
            Body body = null;
            Assert_NoExceptionThrownTryCatch(() => body = new Body(DefNameOf.Body.Human));

            var vitalParts = new List<BodyPart>();
            vitalParts.AddRange(body.BodyParts, predicate: bodyPart => bodyPart.Def.IsVital);
            Assert.IsFalse(vitalParts.IsNullOrEmpty());

            float damage = vitalParts[0].Def.MaxHitpoints / 2;
            ICollection<string> injuries = new List<string>() { DefNameOf.Injury.Bruise };
            var hitInfo = new BodyHitInfo(vitalParts[0].FloorHeight - (vitalParts[0].Length / 2), injuries, damage);

            Assert.AreEqual(0, body.BleedingRate.Value);
            Assert.AreEqual(0, body.BloodLoss.Value);

            BodyPart mostDamagedVitalPart = null;
            float previousBodyHitpoints = body.HitPoints.Value;
            float previousMostDamagedVitalPartHitPoints = 0f;
            bool assertedAmplificationAtLeastOnce = false;

            while (!body.IsDestroyed)
            {
                previousBodyHitpoints = body.HitPoints.Value;

                if (mostDamagedVitalPart != null)
                {
                    previousMostDamagedVitalPartHitPoints = mostDamagedVitalPart.HitPoints.Value;
                }

                Assert_NoExceptionThrownTryCatch(() => body.TakeDamage(hitInfo));             
                
                if (body.IsDestroyed)
                {
                    break;
                }
                
                foreach (var bodyPart in vitalParts)
                {
                    if (!bodyPart.IsDamaged)
                    {
                        continue;
                    }

                    if (mostDamagedVitalPart == null)
                    {
                        mostDamagedVitalPart = bodyPart;
                        break;
                    }

                    float remainingBodyPartHitPointsPercentage = mostDamagedVitalPart.HitPoints.Value / mostDamagedVitalPart.HitPoints.MaxValue;
                    float otherRemainingBodyPartHitPointsPercentage = bodyPart.HitPoints.Value / bodyPart.HitPoints.MaxValue;

                    if (otherRemainingBodyPartHitPointsPercentage < remainingBodyPartHitPointsPercentage)
                    {
                        mostDamagedVitalPart = bodyPart;
                    }
                }

                // test for damage amplification
                if (mostDamagedVitalPart != null && mostDamagedVitalPart.HitPoints.Value != previousMostDamagedVitalPartHitPoints)
                {
                    float remainingBodyPartHitPointsPercentage = mostDamagedVitalPart.HitPoints.Value / mostDamagedVitalPart.HitPoints.MaxValue;

                    if (remainingBodyPartHitPointsPercentage < 0.8f)     // amplification doesn't take effect if damage is minor (anything less than 20% loss of hitpoints gets amplified by just 1, therefore unchanged)
                    {
                        float damageToBodyPart = hitInfo.Damage;
                        float damageToBody = previousBodyHitpoints - body.HitPoints.Value;
                        Assert.IsTrue(damageToBody > damageToBodyPart);
                        assertedAmplificationAtLeastOnce = true;
                    }
                }
                

                Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            }

            Assert.IsTrue(assertedAmplificationAtLeastOnce);

            BodyPart destroyedPart = null;
            vitalParts.ForEach(part => destroyedPart = part.IsDestroyed ? part : destroyedPart);
            Assert.IsTrue(destroyedPart.Def.IsVital);
            Assert.IsTrue(destroyedPart.IsDestroyed);
            Assert.IsTrue(body.IsDestroyed);
            Assert.IsTrue(body.HitPoints.Value > 0);    // although destroyed, body should still have some hp left
        }

        [Test]
        public void TestHealedBleedingInjury()
        {
            Body body = null;
            Assert_NoExceptionThrownTryCatch(() => body = new Body(DefNameOf.Body.Human));

            var legs = new List<BodyPart>();
            legs.AddRange(body.BodyParts, predicate: bodyPart => bodyPart.Def.Name == DefNameOf.BodyPart.Human_Leg);
            Assert.IsFalse(legs.IsNullOrEmpty());

            float damage = legs[0].Def.MaxHitpoints / 4;        // bleeding of the injury is assumed to be high such as that it should limit the healing more than the damage
            float expectedHPAfterDamage = legs[0].HitPoints.Value - damage;
            ICollection<string> injuries = new List<string>() { DefNameOf.Injury.Laceration };
            var hitInfo = new BodyHitInfo(legs[0].FloorHeight - (legs[0].Length / 2), injuries, damage);
            Assert.AreEqual(0, body.BloodLoss.Value);
            Assert_NoExceptionThrownTryCatch(() => body.TakeDamage(hitInfo));
            Assert.IsTrue(body.BloodLoss.Value > 0);

            BodyPart damagedLeg = null;
            legs.ForEach(leg => damagedLeg = leg.HitPoints.Value != leg.HitPoints.DefaultValue ? leg : damagedLeg);
            Assert.IsTrue(damagedLeg.IsDamaged);
            Assert.AreEqual(expectedHPAfterDamage, damagedLeg.HitPoints.Value);

            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());    // must run an update for the new injuries to be processed in the damaged leg

            Assert.AreEqual(injuries.Count, damagedLeg.Injuries.Count);
            var injuryEnumerator = damagedLeg.Injuries.GetEnumerator();
            injuryEnumerator.MoveNext();
            Injury laceration = injuryEnumerator.Current;
            Assert.IsNotNull(laceration);
            Assert.IsTrue(damagedLeg.IsDamaged);    // should still be damaged

            //boost healing rate to heal injury in 3 more updates (+1 an extra one for unhindered healing)
            float healingRateBonus = Config.DefaultInjuryHealThreshold / body.HealingRate.Value / 3 / laceration.HealingRateMultiplier;
            body.HealingRate.ApplyIncrease(healingRateBonus);

            Assert.IsTrue(body.HealingRate.Value * 3 + laceration.HealedAmount > Config.DefaultInjuryHealThreshold);

            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert.IsTrue(damagedLeg.IsDamaged);
            Assert.IsFalse(laceration.IsHealed);
            Assert.IsFalse(damagedLeg.HitPoints.IsAtMaxValue());
            Assert.IsFalse(body.HitPoints.IsAtMaxValue());
            
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert.IsTrue(laceration.IsHealed);

            // whether injury is healed or not, depends on bleeding rate and what not, so as long as that's not fully worked out, this part remains uncertain
            Assert_NoExceptionThrownTryCatch(() => body.OnUpdate());
            Assert.IsFalse(damagedLeg.IsDamaged);

            Assert.IsTrue(damagedLeg.HitPoints.IsAtMaxValue());
            Assert.IsTrue(body.HitPoints.IsAtMaxValue());
            Assert.AreEqual(0, body.BloodLoss.Value);
        }
    }
}
