using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorToggle : MonoBehaviour
{
    [Tooltip("The toggle will alternate between the image's original color and this one.")]
    [SerializeField]
    private Color alternateColor;

    private Color originalImageColor;
    private Image image;
    private bool isUsingOriginalColor;

    public void Start()
    {
        this.image = this.GetComponent<Image>();

        if (image == null)
        {
            Debug.LogWarning($"{nameof(ImageColorToggle)}: no Image comp detected on {this.name}. This comp won't have any effect...");
        }
        else
        {
            this.originalImageColor = image.color;
            this.isUsingOriginalColor = true;
        }
    }

    public void ToggleColor()
    {
        if (this.image == null || this.originalImageColor == null)
        {
            return;
        }

        if (isUsingOriginalColor)
        {
            this.image.color = alternateColor;
            this.isUsingOriginalColor = false;
        }
        else
        {
            this.image.color = this.originalImageColor;
            this.isUsingOriginalColor = true;
        }
    }
}
