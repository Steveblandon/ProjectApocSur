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
        protected int withinRangeBodySize;

        public BodyHitBox(Range targetRange, IReadOnlyList<BodyPart> bodyParts, float bodyPartLengthMultiplier)
        {
            this.TargetRange = targetRange;
            this.Calibrate(bodyParts, bodyPartLengthMultiplier);
        }

        [XmlMember]
        public Range TargetRange { get; protected set; }

        [XmlMember]
        public bool IsEmpty { get; protected set; }

        public void Calibrate(IReadOnlyList<BodyPart> bodyParts, float bodyPartLengthMultiplier = 1f)
        {
            this.bodyHitChances = new List<float>(bodyParts.Count);
            this.withinRangeBodySize = 0;

            foreach (var bodyPart in bodyParts)
            {
                float adjustedLength = bodyPart.Length * bodyPartLengthMultiplier;
                float adjustedFloorOffset = bodyPart.FloorOffset * bodyPartLengthMultiplier;
                float adjustedFloorHeight = adjustedLength + adjustedFloorOffset;
                float lengthWithinRange = adjustedLength;      // begin by assuming the entire length is within range
                bool isfloorOffsetWithinBounds = this.TargetRange.IsValueWithinBounds(adjustedFloorOffset);
                bool isHeightWithinBounds = this.TargetRange.IsValueWithinBounds(adjustedFloorHeight);

                if (isfloorOffsetWithinBounds && !isHeightWithinBounds)
                {
                    lengthWithinRange = this.TargetRange.UpperBound - adjustedFloorOffset;
                }
                else if (!isfloorOffsetWithinBounds && isHeightWithinBounds)
                {
                    lengthWithinRange = adjustedFloorHeight - this.TargetRange.LowerBound;
                }
                else if (!isfloorOffsetWithinBounds && !isHeightWithinBounds)
                {
                    lengthWithinRange = 0;
                }

                if (lengthWithinRange > 0)
                {
                    this.withinRangeBodySize += bodyPart.Def.Size;
                }

                // factor in the portion of body part within range for hit chance
                float hitChance = lengthWithinRange / adjustedLength;
                this.bodyHitChances.Add(hitChance);
            }

            if (this.withinRangeBodySize > 0)
            {
                float totalHitChances = 0;

                // factor in body part size relative to all body parts within range for hit chance
                for (int index = 0; index < bodyHitChances.Count; index++)
                {
                    bodyHitChances[index] *= (bodyParts[index].Def.Size / (float)this.withinRangeBodySize);
                    totalHitChances += bodyHitChances[index];
                }

                // make hit chance proportional to total hit chances, this is so that all hit chances will always add up to 1 and there is no chance to miss
                for (int index = 0; index < bodyHitChances.Count; index++)
                {
                    bodyHitChances[index] /= totalHitChances;
                }
            }
            else
            {
                this.IsEmpty = true;
            }
        }

        public (BodyPart bodyPart, int index) GetHitBodyPart(IReadOnlyList<BodyPart> bodyParts)
        {
            if (this.bodyHitChances.Count != bodyParts.Count)
            {
                throw new ArgumentException($"count = {this.bodyHitChances.Count}, expected count = {bodyParts.Count}", nameof(this.bodyHitChances));
            }

            double randValue = RandomNumberGenerator.Roll();
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
                    return (bodyParts[index], index);
                }
            }

            return (null, -1);
        }
    }
}