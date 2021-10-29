namespace Projapocsur.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Projapocsur.Common;
    using System;

    public class StatBarUI : MonoBehaviour
    {
        [SerializeField]
        private Direction direction;

        [SerializeField]
        private float statMinValue, statMaxValue, statValue;

        [SerializeField]
        private int MaxHueDeviation;

        [SerializeField]
        private Image image;

        private RectTransform rectTransform, rectTransformInternal;

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
                this.rectTransform = this.GetComponent<RectTransform>();
                this.rectTransformInternal = this.image.GetComponent<RectTransform>();
                this.imageOriginalColor = this.image.color;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // stat changes will be event-driven and the responsible for triggering a rect transform update,
            // but for the time being call from here with the use of the dummy stats.
            float statValueSpan = Math.Abs(statMaxValue - statMinValue);
            float relativeStatValue = Math.Abs(statValue - statMinValue);
            float statPercentage = relativeStatValue / (statValueSpan == 0 ? 0.000001f : statValueSpan);
            this.UpdateRectTransform(statPercentage);
        }

        void UpdateImageColor(float percentage)
        {
            if (this.MaxHueDeviation <= 0f)
            {
                if (this.imageOriginalColor != this.image.color)
                {
                    this.image.color = this.imageOriginalColor;
                }

                return;
            }

            Color.RGBToHSV(this.imageOriginalColor, out float H, out float S, out float V);
            float newHue = H - (this.MaxHueDeviation / 360f * percentage);
            this.image.color = Color.HSVToRGB(newHue, S, V);
        }

        void UpdateRectTransform(float percentage)
        {
            percentage = 1 - percentage;

            this.UpdateImageColor(percentage);

            switch (direction)
            {
                case Direction.Up:
                    this.rectTransformInternal.SetTop(this.rectTransform.rect.height * percentage);
                    this.rectTransformInternal.SetBottom(0);
                    this.rectTransformInternal.SetLeft(0);
                    this.rectTransformInternal.SetRight(0);
                    break;
                case Direction.Down:
                    this.rectTransformInternal.SetTop(0);
                    this.rectTransformInternal.SetBottom(this.rectTransform.rect.height * percentage);
                    this.rectTransformInternal.SetLeft(0);
                    this.rectTransformInternal.SetRight(0);
                    break;
                case Direction.Left:
                    this.rectTransformInternal.SetTop(0);
                    this.rectTransformInternal.SetBottom(0);
                    this.rectTransformInternal.SetLeft(this.rectTransform.rect.width * percentage);
                    this.rectTransformInternal.SetRight(0);
                    break;
                case Direction.Right:
                    this.rectTransformInternal.SetTop(0);
                    this.rectTransformInternal.SetBottom(0);
                    this.rectTransformInternal.SetLeft(0);
                    this.rectTransformInternal.SetRight(this.rectTransform.rect.width * percentage);
                    break;
            }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
