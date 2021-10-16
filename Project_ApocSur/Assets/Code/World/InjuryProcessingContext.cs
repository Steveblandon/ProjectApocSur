namespace Projapocsur.World
{
    public class InjuryProcessingContext
    {
        public InjuryProcessingContext()
        {
        }

        public InjuryProcessingContext(Stat pain, Stat bloodLoss, Stat healingRate)
        {
            this.Pain = pain;
            this.BloodLoss = bloodLoss;
            this.HealingRate = healingRate;
        }

        public Stat Pain { get; set; }

        public Stat BloodLoss { get; set; }

        public Stat HealingRate { get; set; }

        public void CopyTo(out InjuryProcessingContext otherContext)
        {
            otherContext = new InjuryProcessingContext(this.Pain.Clone(), this.BloodLoss.Clone(), this.HealingRate.Clone());
        }
    }
}