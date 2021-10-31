namespace Projapocsur
{
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class Range
    {
        public Range(float lowerBound, float upperBound)
        {
            this.LowerBound = lowerBound;
            this.UpperBound = upperBound;
        }

        [XmlMember]
        public float LowerBound { get; }

        [XmlMember]
        public float UpperBound { get; }

        /// <summary>
        /// Checks to see if value is within bounds (only upper bound is inclusive).
        /// </summary>
        public bool IsValueWithinBounds(float value)
        {
            return this.LowerBound < value && value <= this.UpperBound;
        }
    }
}