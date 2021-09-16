namespace Projapocsur.Deprecated
{
    using System;

    [Obsolete]
    public static class EventType
    {
        // UI Events
        public static string UI_NothingClicked { get; } = nameof(UI_NothingClicked);
        public static string UI_SelectionChanged { get; } = nameof(UI_SelectionChanged);

        // WorldObject Events
        public static string WO_NothingClicked_Left { get; } = nameof(WO_NothingClicked_Left);
        public static string WO_NothingClicked_Right { get; } = nameof(WO_NothingClicked_Right);

        // Character Management
        public static string CM_CharacterDraftStateChanged { get; } = nameof(CM_CharacterDraftStateChanged);
        public static string CM_CharacterSelectionStateChanged { get; } = nameof(CM_CharacterSelectionStateChanged);
    }

}