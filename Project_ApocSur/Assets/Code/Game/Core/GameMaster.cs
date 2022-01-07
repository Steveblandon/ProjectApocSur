namespace Projapocsur
{
    using Projapocsur.Engine;

    public class GameMaster
    {
        private GameMaster(GameConfiguration config)
        {
            this.Config = config;
        }

        private static GameMaster _instance;

        public static GameMaster Instance { get => _instance; }

        public static GameMaster Init(GameConfiguration config)
        {
            if (_instance == null)
            {
                _instance = new GameMaster(config);
            }

            return _instance;
        }

        public CharacterSelectionTracker PlayerCharacterSelection { get; } = new CharacterSelectionTracker();

        public ObjectPool ObjectPool { get; } = new ObjectPool();

        public GameConfiguration Config { get; }
    }
}
