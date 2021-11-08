namespace Projapocsur
{
    public class GameMaster
    {
        private GameMaster()
        {
            this.DraftTracker = new DraftTracker();
            this.CharacterSelectionTracker = new CharacterSelectionTracker();
        }

        public static GameMaster Instance { get { return _instance; } }
        private static GameMaster _instance = new GameMaster();

        public DraftTracker DraftTracker { get; protected set; }
        public CharacterSelectionTracker CharacterSelectionTracker { get; protected set; }
    }
}
