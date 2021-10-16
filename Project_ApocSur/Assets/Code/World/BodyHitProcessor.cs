namespace Projapocsur.World
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;

    /// <summary>
    /// Processes <see cref="Body"/> hits. Needs to be recalibrated everytime the <see cref="BodyPart"/>s list changes.
    /// </summary>
    [XmlSerializable]
    public class BodyHitProcessor
    {
        [XmlMember]
        protected Dictionary<string, StanceHitBoxes> hitBoxesPerStance;

        public BodyHitProcessor() { }

        public BodyHitProcessor(List<DefRef<StanceDef>> stanceDefRefs, List<BodyPart> bodyParts, int hitBoxCount, float height)
        {
            this.hitBoxesPerStance = new Dictionary<string, StanceHitBoxes>(stanceDefRefs.Count);
            float rangeLength = height / hitBoxCount;
            
            // determine fixed ranges to use no matter what the stance
            float trailingLowerBound = -rangeLength;
            float trailingUpperBound = 0;

            var targetRanges = new List<Range>(hitBoxCount);

            while (targetRanges.Count < targetRanges.Capacity)
            {
                trailingLowerBound += rangeLength;
                trailingUpperBound += rangeLength;
                targetRanges.Add(new Range(trailingLowerBound > 0f ? trailingLowerBound : -1, trailingUpperBound));     // the lowest range with lower bound 0 is assumed to be ground level, nothing should be lower than this, but the floorOffset of something might be exactly 0, so we shift this down to -1 to make sure it fits
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

        /// <summary>
        /// Processes a body hit.
        /// </summary>
        /// <returns>The index of the damaged body part.</returns>
        public int Process(BodyHitInfo hit, List<BodyPart> bodyParts, DefRef<StanceDef> currentStance)
        {
            var hitBoxes = this.hitBoxesPerStance[currentStance.RefDefName].HitBoxes;

            for (int index = 0; index < hitBoxes.Count; index++)
            {
                var currentHitBox = hitBoxes[index];

                if (currentHitBox.TargetRange.IsValueWithinBounds(hit.Height))
                {
                    var hitBodyPart = currentHitBox.GetHitBodyPart(bodyParts);
                    hitBodyPart.TakeDamage(hit.Damage, hit.InjuryDefNames);
                    return index;
                }
            }

            throw new InvalidOperationException($"unable to process hit. hitBoxCount={hitBoxes.Count}");
        }

        public void ReCalibrate(List<BodyPart> bodyParts)
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