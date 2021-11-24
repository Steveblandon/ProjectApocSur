namespace Projapocsur.World
{
    using System.Collections.Generic;
    using Projapocsur.Serialization;

    /// <summary>
    /// Processes <see cref="Body"/> hits. Needs to be recalibrated everytime the <see cref="BodyPart"/>s list changes.
    /// </summary>
    [XmlSerializable]
    public class BodyHitProcessor
    {
        [XmlMember]
        protected Dictionary<string, StanceHitBoxes> hitBoxesPerStance;

        public BodyHitProcessor() { }

        public BodyHitProcessor(List<DefRef<StanceDef>> stanceDefRefs, IReadOnlyList<BodyPart> bodyParts, int hitBoxCount, float height)
        {
            this.hitBoxesPerStance = new Dictionary<string, StanceHitBoxes>(stanceDefRefs.Count);
            float rangeLength = height / hitBoxCount;
            
            // determine fixed ranges to use no matter what the stance
            float trailingLowerBound = -rangeLength;
            float trailingUpperBound = 0;

            var targetRanges = new List<Span>(hitBoxCount);

            while (targetRanges.Count < targetRanges.Capacity)
            {
                trailingLowerBound += rangeLength;
                trailingUpperBound += rangeLength;
                targetRanges.Add(new Span(trailingLowerBound > 0f ? trailingLowerBound : -1, trailingUpperBound));     // the lowest range with lower bound 0 is assumed to be ground level, nothing should be lower than this, but the floorOffset of something might be exactly 0, so we shift this down to -1 to make sure it fits
            }

            // for each stance, determine which ranges must be used, and thus whether a hitbox is neccessary or not.
            foreach (var stanceDefRef in stanceDefRefs)
            {
                var hitBoxes = new List<BodyHitBox>(hitBoxCount);
                float adjustedHeight = height * stanceDefRef.Def.HeightMultiplier;
                
                foreach (var range in targetRanges)
                {
                    if (range.LowerBound <= adjustedHeight)      // this helps avoid creating an empty hitbox, but guarantees any ranges below the adjusted height are used.
                    {
                        var hitBox = new BodyHitBox(range, bodyParts, stanceDefRef.Def.HeightMultiplier);

                        if (!hitBox.IsEmpty)
                        {
                            hitBoxes.Add(hitBox);
                        }
                    }
                }

                this.hitBoxesPerStance[stanceDefRef.RefDefName] = new StanceHitBoxes(stanceDefRef, hitBoxes);
            }
        }

        public (BodyPart bodyPart, int index) ProcessHit(DamageInfo hit, IReadOnlyList<BodyPart> bodyParts, DefRef<StanceDef> currentStance)
        {
            var hitBoxes = this.hitBoxesPerStance[currentStance.RefDefName].HitBoxes;

            for (int hitboxIndex = 0; hitboxIndex < hitBoxes.Count; hitboxIndex++)
            {
                var currentHitBox = hitBoxes[hitboxIndex];

                if (currentHitBox.TargetSpan.IsValueWithinBounds(hit.Height))
                {
                    var (hitBodyPart, bodyPartIndex) = currentHitBox.GetHitBodyPart(bodyParts);
                    hitBodyPart.TakeDamage(hit.Damage, hit.InjuryDefNames);
                    return (hitBodyPart, bodyPartIndex);
                }
            }

            LogUtility.Log(LogLevel.Warning, $"processed a bodyHit miss, but that is not expected inside {nameof(BodyHitProcessor)}");
            return (null, -1);
        }

        public void ReCalibrate(IReadOnlyList<BodyPart> bodyParts)
        {
            foreach (var hitBoxesPerStanceItem in hitBoxesPerStance)
            {
                var currentDef = hitBoxesPerStanceItem.Value.StanceDefRef.Def;
                var currentHitboxes = hitBoxesPerStanceItem.Value.HitBoxes;
                var emptyHitBoxIndices = new Stack<int>();

                for (int index = 0; index < currentHitboxes.Count; index++)
                {
                    var hitbox = currentHitboxes[index];
                    hitbox.Calibrate(bodyParts, currentDef.HeightMultiplier);

                    if (hitbox.IsEmpty)
                    {
                        emptyHitBoxIndices.Push(index);
                    }
                }

                while (emptyHitBoxIndices.Count > 0)
                {
                    currentHitboxes.RemoveAt(emptyHitBoxIndices.Pop());
                }
            }
        }
    }
}