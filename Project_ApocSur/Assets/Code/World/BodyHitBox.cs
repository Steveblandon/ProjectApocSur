namespace Projapocsur.World
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.Common;
    using Projapocsur.Common.Serialization;

    /*
     * NOTE: we avoid having bodyParts as a instance variable since serializing them here would create duplicates on deserialization.
     * Only one serializable should contain bodyParts, in this case that would be the Body type.
     */

    /// <summary>
    /// Once calibrated to a list of body parts it can be used to determine if a height-based hit hits and which body part it hit.
    /// 
    /// <para>
    /// The instance should be re-calibrated whenever the list of body parts changes.
    /// </para>
    /// </summary>
    [XmlSerializable]
    public class BodyHitBox
    {
        [XmlMember]
        protected List<float> bodyHitChances = new List<float>();

        [XmlMember]
        private int withinRangeBodySize;

        private Random random = new Random();

        public BodyHitBox(Range targetRange, List<BodyPart> bodyParts)
        {
            this.TargetRange = targetRange;
            this.Calibrate(bodyParts);
        }

        [XmlMember]
        public Range TargetRange { get; }

        public void Calibrate(List<BodyPart> bodyParts)
        {
            this.bodyHitChances = new List<float>(bodyParts.Count);
            this.withinRangeBodySize = 0;

            foreach (var bodyPart in bodyParts)
            {
                withinRangeBodySize += bodyPart.Def.Size;
                float heightFromFloor = bodyPart.Def.Height + bodyPart.Def.FloorOffset;
                float heightWithinRange = bodyPart.Def.Height;
                bool isfloorOffsetWithinBounds = this.TargetRange.IsValueWithinBounds(bodyPart.Def.FloorOffset);
                bool isHeightWithinBounds = this.TargetRange.IsValueWithinBounds(heightFromFloor);

                if (isfloorOffsetWithinBounds && !isHeightWithinBounds)
                {
                    heightWithinRange = this.TargetRange.UpperBound - bodyPart.Def.FloorOffset;
                }
                else if (!isfloorOffsetWithinBounds && isHeightWithinBounds)
                {
                    heightWithinRange = heightFromFloor - this.TargetRange.LowerBound;
                }
                else if (!isfloorOffsetWithinBounds && !isHeightWithinBounds)
                {
                    heightWithinRange = 0;
                }

                // factor in the portion of body part within range for hit chance
                float hitChance = heightWithinRange / bodyPart.Def.Height;
                bodyHitChances.Add(hitChance);
            }

            float totalHitChances = 0;

            // factor in body part size relative to all body parts within range for hit chance
            for (int index = 0; index < bodyHitChances.Count; index++)
            {
                bodyHitChances[index] *= (bodyParts[index].Def.Size / (float) this.withinRangeBodySize);
                totalHitChances += bodyHitChances[index];
            }

            // make hit chance proportional to total hit chances, this is so that all hit chances will always add up to 1 and there is no chance to miss
            for (int index = 0; index < bodyHitChances.Count; index++)
            {
                bodyHitChances[index] /= totalHitChances;
            }
        }

        public BodyPart GetHitBodyPart(List<BodyPart> bodyParts)
        {
            if (this.bodyHitChances.Count != bodyParts.Count)
            {
                throw new ArgumentException($"count = {this.bodyHitChances.Count}, expected count = {bodyParts.Count}", nameof(this.bodyHitChances));
            }

            float randValue = this.random.Next(0, 10) / 10f;
            float trailingSum = 0;

            for (int index = 0; index < this.bodyHitChances.Count; index++)
            {
                if (this.bodyHitChances[index] == 0)
                {
                    continue;
                }

                trailingSum += this.bodyHitChances[index];

                if (randValue <= trailingSum)
                {
                    return bodyParts[index];
                }
            }

            return null;
        }
    }
}