namespace Projapocsur.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;

    public class BodyPartUI : StatViewUI
    {
        [SerializeField]
        private Image image, outline;

        [Tooltip("percentage of stat reduction at which color change is triggered")]
        [SerializeField]
        private float reductionThreshold;

        [Tooltip("color to which image will change to once stat reduction is below threshold")]
        [SerializeField]
        private Color belowThresholdColor;

        private Color imageOriginalColor;

        // Start is called before the first frame update
        void Start()
        {
            if (image == null)
            {
                Debug.LogError($"missing image reference. disabling {this.name}.");
                this.enabled = false;
            }
            else
            {
                this.imageOriginalColor = this.image.color;
            }
        }

        // Update is called once per frame
        void Update()
        {
            float statPercentageReduction = this.GetStatPercentageReduction();

            if (statPercentageReduction > reductionThreshold)
            {
                this.originalColor = belowThresholdColor;
                image.color = this.GetCurrentColor(statPercentageReduction);
            }
            else
            {
                image.color = imageOriginalColor;
            }
        }
    }
}
