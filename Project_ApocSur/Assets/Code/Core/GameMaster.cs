namespace Projapocsur
{
    public class GameMaster
    {
        private GameMaster()
        {
        }

        private static GameMaster _instance = new GameMaster();

        public static GameMaster Instance { get => _instance; }

        public static GameMaster Init() => Instance;

        public CharacterSelectionTracker CharacterSelectionTracker { get; } = new CharacterSelectionTracker();

        public DraftTracker DraftTracker { get; } = new DraftTracker();

        public ObjectPool ObjectPool { get; } = new ObjectPool();
    }
}
