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

        public CharacterSelectionTracker PlayerCharacterSelection { get; } = new CharacterSelectionTracker();

        public ObjectPool ObjectPool { get; } = new ObjectPool();
    }
}
