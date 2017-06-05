using System;

namespace Calabonga.TicTac.Web
{
    public sealed class GameManager
    {
        private static readonly Lazy<GameManager> Lazy = new Lazy<GameManager>(() => new GameManager());
        private readonly GameManagerInternal _manager = new GameManagerInternal();
 

        private GameManager() { }

        public static GameManager Instance
        {
            get { return Lazy.Value; }
        }

        public GameManagerInternal Manager
        {
            get { return _manager; }
        }
    }
}