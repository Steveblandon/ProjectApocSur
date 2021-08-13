namespace Projapocsur.Common
{
    public static class EventType
    {
        // UI Events
        public static string UI_NothingClicked { get; private set; } = nameof(UI_NothingClicked);
        public static string UI_SelectionChanged { get; private set; } = nameof(UI_SelectionChanged);

        // WorldObject Events
        public static string WO_NothingClicked { get; private set; } = nameof(WO_NothingClicked);
    }

}