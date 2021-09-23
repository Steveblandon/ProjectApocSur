namespace Projapocsur.Things
{
    public class InjuryProcessingContext
    {
        public Stat Pain { get; set; }

        public Stat BloodLoss { get; set; }

        public Stat HealingRate { get; set; }

        public void CopyTo(InjuryProcessingContext otherContext)
        {
            otherContext.Pain = this.Pain.Clone();
            otherContext.BloodLoss = this.BloodLoss.Clone();
            otherContext.HealingRate = this.HealingRate.Clone();
        }
    }
}