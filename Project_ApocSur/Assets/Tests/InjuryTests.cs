namespace Projapocsur.Tests
{
    using System;
    using NUnit.Framework;
    using Projapocsur.World;

    public class InjuryTests : DefDependantTestsBase
    {
        private static InjuryProcessingContext injuryProcessingContext;     // note, private fields get lost after setup, need to be static to be persistant

        public override void Setup()
        {
            base.Setup();

            Stat bloodLoss = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => bloodLoss = new Stat(DefNameOf.Stat.BloodLoss, 0, 100));

            Stat healingRate = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => healingRate = new Stat(DefNameOf.Stat.HealingRate, 100, false));

            Stat pain = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => pain = new Stat(DefNameOf.Stat.Pain, 0, 100));

            injuryProcessingContext = new InjuryProcessingContext()
            {
                BloodLoss = bloodLoss,
                HealingRate = healingRate,
                Pain = pain
            };
        }

        [Test]
        public void TestLaceration_StatEffects_And_DefaultHealing()
        {
            float cutHealNeeded = Config.DefaultInjuryHealThreshold;

            Injury cutWound = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => cutWound = new Injury(DefNameOf.Injury.Laceration, SeverityLevel.Minor));

            float healingRateValue = cutHealNeeded / 2 / cutWound.HealingRateMultiplier;    // we set this up like this so that no matter what the healing rate multiplier is, the wound heals in 2 updates

            injuryProcessingContext.CopyTo(out InjuryProcessingContext context);
            context.HealingRate = new Stat(DefNameOf.Stat.HealingRate, healingRateValue, false);

            float previousPainValue = context.Pain.Value;
            cutWound.OnStart(context);

            Assert.IsTrue(context.Pain.Value > previousPainValue);                  // check for an increase in pain
            Assert.AreEqual(context.Pain.Value, cutWound.PainIncrease.Value);       // since its an isolated test, these should be equal

            previousPainValue = context.Pain.Value;
            float previousBloodLossValue = context.BloodLoss.Value;
            float previousBleedingRateValue = cutWound.BleedingRate.Value;
            cutWound.OnUpdate(context);

            Assert.IsTrue(context.Pain.Value < previousPainValue);                      // check for a decrease in pain due to healing
            Assert.IsTrue(cutWound.BleedingRate.Value < previousBleedingRateValue);     // check for a decrease in bleeding rate due to healing
            Assert.IsTrue(context.BloodLoss.Value > previousBloodLossValue);            // check for an increase is blood lost
            Assert.IsFalse(cutWound.IsHealed);                                          // since the healing rate is half of the wound's heal threshold, this shouldn't be healed yet

            cutWound.OnUpdate(context);

            Assert.IsTrue(cutWound.IsHealed);           // since the healing rate is half of the bruise's heal threshold, on the second update this should have healed
            Assert.AreEqual(0, context.Pain.Value);     // since wound healed completely, there should be no pain
        }

        [Test]
        public void TestBruise_StatEffects_And_OnDestroyEffect()
        {
            float bruiseHealNeeded = Config.DefaultInjuryHealThreshold;

            Injury bruise = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => bruise = new Injury(DefNameOf.Injury.Bruise, SeverityLevel.Minor));

            float healingRateValue = bruiseHealNeeded / 2 / bruise.HealingRateMultiplier;    // we set this up like this so that no matter what the healing rate multiplier is, the wound heals in 2 updates

            injuryProcessingContext.CopyTo(out InjuryProcessingContext context);
            context.HealingRate = new Stat(DefNameOf.Stat.HealingRate, healingRateValue, false);

            float previousPainValue = context.Pain.Value;
            bruise.OnStart(context);

            Assert.IsTrue(context.Pain.Value > previousPainValue);              // check for an increase in pain
            Assert.AreEqual(context.Pain.Value, bruise.PainIncrease.Value);     // since its an isolated test, these should be equal

            previousPainValue = context.Pain.Value;
            float previousBloodLossValue = context.BloodLoss.Value;
            float previousBleedingRateValue = bruise.BleedingRate.Value;
            bruise.OnUpdate(context);

            Assert.IsTrue(context.Pain.Value < previousPainValue);                      // check for a decrease in pain due to healing
            Assert.AreEqual(previousBleedingRateValue, bruise.BleedingRate.Value);      // check for no change in bleeding rate, N/A
            Assert.AreEqual(previousBloodLossValue, context.BloodLoss.Value);           // check for no change in blood lost, N/A
            Assert.IsFalse(bruise.IsHealed);                                            // since the healing rate is half of the wound's heal threshold, this shouldn't be healed yet

            bruise.OnDestroy(context);

            Assert.IsFalse(bruise.IsHealed);             // bruise instance was destroyed before it could heal (could happen if for example a limb is removed)
            Assert.AreEqual(0, context.Pain.Value);     // since bruise was destroyed, there needs to be no effects left on affected stats (in this case, pain)
        }

        [Test]
        public void TestSeverity_StatEffectAmplification()
        {
            injuryProcessingContext.CopyTo(out InjuryProcessingContext context);

            Injury fracture = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => fracture = new Injury(DefNameOf.Injury.Fracture, SeverityLevel.Trivial));
            fracture.OnStart(context);
            fracture.OnUpdate(context);

            Assert.AreEqual(0, context.BloodLoss.Value);

            var previousPainValue = context.Pain.Value;
            var previousHealingRateMultiplier = fracture.HealingRateMultiplier;

            Assert_NoExceptionThrownTryCatch<Exception>(() => fracture = new Injury(DefNameOf.Injury.Fracture, SeverityLevel.Minor));
            fracture.OnStart(context);
            fracture.OnUpdate(context);

            Assert.AreEqual(0, context.BloodLoss.Value);                                        // since there is no bleeding, blood loss should not change
            Assert.IsTrue(context.Pain.Value > previousPainValue);                              // pain increase as severity increases
            Assert.IsTrue(fracture.HealingRateMultiplier < previousHealingRateMultiplier);      // healing rate multiplier decreases as severity increases

            previousPainValue = context.Pain.Value;
            previousHealingRateMultiplier = fracture.HealingRateMultiplier;

            Assert_NoExceptionThrownTryCatch<Exception>(() => fracture = new Injury(DefNameOf.Injury.Fracture, SeverityLevel.Major));
            fracture.OnStart(context);
            fracture.OnUpdate(context);

            Assert.AreEqual(0, context.BloodLoss.Value);                                        // since there is no bleeding, blood loss should not change
            Assert.IsTrue(context.Pain.Value > previousPainValue);                              // pain increase as severity increases
            Assert.IsTrue(fracture.HealingRateMultiplier < previousHealingRateMultiplier);      // healing rate multiplier decreases as severity increases

            previousPainValue = context.Pain.Value;
            previousHealingRateMultiplier = fracture.HealingRateMultiplier;

            Assert_NoExceptionThrownTryCatch<Exception>(() => fracture = new Injury(DefNameOf.Injury.Fracture, SeverityLevel.Severe));
            fracture.OnStart(context);
            fracture.OnUpdate(context);

            Assert.AreEqual(0, context.BloodLoss.Value);                                        // since there is no bleeding, blood loss should not change
            Assert.IsTrue(context.Pain.Value > previousPainValue);                              // pain increase as severity increases
            Assert.IsTrue(fracture.HealingRateMultiplier < previousHealingRateMultiplier);      // healing rate multiplier decreases as severity increases
        }
    }
}
