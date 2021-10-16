namespace Projapocsur.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Projapocsur.World;

    public class BodyPartTests : DefDependantTestsBase
    {
        private static InjuryProcessingContext injuryProcessingContext;

        public override void Setup()
        {
            base.Setup();

            Stat bloodLoss = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => bloodLoss = new Stat(DefNameOf.Stat.BloodLoss, 0, 100));

            Stat healingRate = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => healingRate = new Stat(DefNameOf.Stat.HealingRate, 100, false));

            Stat pain = null;
            Assert_NoExceptionThrownTryCatch<Exception>(() => pain = new Stat(DefNameOf.Stat.Pain, 0, 100));

            injuryProcessingContext = new InjuryProcessingContext(pain, bloodLoss, healingRate);
        }

        [Test]
        public void TestTakeDamage()
        {
            injuryProcessingContext.CopyTo(out InjuryProcessingContext context);

            BodyPart bodyPart = new BodyPart(DefNameOf.BodyPart.Human_Arm);
            bodyPart.OnStart(context);

            var injuriesNames = new List<string>() { DefNameOf.Injury.Bruise, DefNameOf.Injury.Fracture };

            foreach (var threshold in Config.SeverityThresholds)
            {
                Assert_NoExceptionThrownTryCatch(() => bodyPart.TakeDamage(bodyPart.HitPoints.MaxValue * threshold.Key - 1, injuriesNames));
                bodyPart.OnUpdate(context);     // due to how injuries processing works now, new injuries will only be exposed after the next update call after taking damage

                if (!bodyPart.IsDestroyed)
                {
                    Assert.AreEqual(injuriesNames.Count, bodyPart.Injuries.Count);
                }

                foreach (var injury in bodyPart.Injuries)
                {
                    Assert.AreEqual(threshold.Value, injury.Severity);
                }

                bodyPart.OnDestroy(context);

                Assert.AreEqual(0, bodyPart.Injuries.Count);
            }
        }

        [Test]
        public void TestHealedInjury()
        {
            injuryProcessingContext.CopyTo(out InjuryProcessingContext context);

            BodyPart bodyPart = new BodyPart(DefNameOf.BodyPart.Human_Arm);

            Assert_NoExceptionThrownTryCatch(() => bodyPart.OnStart(context));       // nothing should happen here since there are no injuries
            Assert_NoExceptionThrownTryCatch(() => bodyPart.OnUpdate(context));      // nothing should happen here since there are no injuries
            Assert_NoExceptionThrownTryCatch(() => bodyPart.OnDestroy(context));     // nothing should happen here since there are no injuries

            var injuriesNames = new List<string>() { DefNameOf.Injury.Laceration, DefNameOf.Injury.Bruise, DefNameOf.Injury.Fracture };
            float painBeforeDamage = context.Pain.Value;

            bodyPart.TakeDamage(1, injuriesNames);
            bodyPart.OnUpdate(context);     // due to how injuries processing works now, new injuries will only be exposed after the next update call after taking damage

            Assert.IsTrue(context.Pain.Value > painBeforeDamage);
            Assert.AreEqual(injuriesNames.Count, bodyPart.Injuries.Count);

            Injury bruise = null;
            Injury fracture = null;

            foreach (var injury in bodyPart.Injuries)
            {
                if (injury.Def.Name == DefNameOf.Injury.Bruise)
                {
                    bruise = injury;
                }
                else if (injury.Def.Name == DefNameOf.Injury.Fracture)
                {
                    fracture = injury;
                }
            }

            Assert.AreEqual(DefNameOf.Injury.Bruise, bruise.Def.Name);
            Assert.AreEqual(DefNameOf.Injury.Fracture, fracture.Def.Name);

            Assert.IsNotNull(bruise);
            Assert.IsTrue(bruise.HealingRateMultiplier > 0);
            Assert.AreEqual(100, context.HealingRate.Value);
            Assert.AreEqual(100, Config.DefaultInjuryHealThreshold);

            while (!bruise.IsHealed)
            {
                bodyPart.OnUpdate(context);
            }

            Assert.IsTrue(bruise.IsHealed);
            Assert.IsFalse(fracture.IsHealed);

            Assert.AreEqual(injuriesNames.Count - 1, bodyPart.Injuries.Count);
        }
    }
}
