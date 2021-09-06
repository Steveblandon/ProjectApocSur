namespace Projapocsur.Common
{
    public class GameManager
    {
        private GameManager()
        {
            this.DraftTracker = new DraftTracker();
            this.CharacterSelectionTracker = new CharacterSelectionTracker();
        }

        public static GameManager Instance { get { return _instance; } }
        private static GameManager _instance = new GameManager();

        public DraftTracker DraftTracker { get; private set; }
        public CharacterSelectionTracker CharacterSelectionTracker { get; private set; }

    }
}
