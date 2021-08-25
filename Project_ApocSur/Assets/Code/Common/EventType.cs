namespace Projapocsur.Common
{
    public static class EventType
    {
        // UI Events
        public static string UI_NothingClicked { get; } = nameof(UI_NothingClicked);
        public static string UI_SelectionChanged { get; } = nameof(UI_SelectionChanged);

        // WorldObject Events
        public static string WO_NothingClicked_Left { get; } = nameof(WO_NothingClicked_Left);
        public static string WO_NothingClicked_Right { get; } = nameof(WO_NothingClicked_Right);

        // Character Management
        public static string CM_CharacterSelected { get; } = nameof(CM_CharacterSelected);
    }

}