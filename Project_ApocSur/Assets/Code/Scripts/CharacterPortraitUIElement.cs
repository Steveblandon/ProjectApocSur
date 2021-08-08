namespace Projapocsur
{
    /// <summary>
    /// Placeholder for if more specific handling is needed.
    /// </summary>
    public class CharacterPortraitUIElement : SelectableUIElement
    {
        public static new readonly string CompName = nameof(CharacterActionUIElement);

        /// <summary>
        /// The character to which this portrait belongs to.
        /// </summary>
        /// NOTE: when a character gets a portrait assigned, it's expected to set this variable so the portrait can link back to it
        /// when interacted with.
        public Character Character { get; set; }
    }
}