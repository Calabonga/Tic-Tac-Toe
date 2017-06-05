using System;
using System.Collections.Generic;
using System.Linq;

namespace Calabonga.TicTac.Web
{
    public class GameManagerInternal
    {
        private readonly List<GameInfo> _games = new List<GameInfo>();

        

        public List<GameInfo> Games
        {
            get { return _games; }
        }

        public void AddGame(GameInfo game)
        {
            var player1 = game.Player1.FullName;
            var player2 = game.Player2.FullName;
            var p2Playing = _games.Any(x => x.Player1.FullName == player2 || x.Player2.FullName == player2);
            var p1Playing = _games.Any(x => x.Player1.FullName == player1 || x.Player2.FullName == player1);
            if (!p1Playing && !p2Playing)
            {
                _games.Add(game);
            }
        }

        public void Delete(string connectionId)
        {
            Games.RemoveAll(x =>
                        x.Player1.Connections.First().ConnectionId == connectionId ||
                        x.Player2.Connections.First().ConnectionId == connectionId);
        }

        public void GameOver(GameInfo game)
        {
            _games.Remove(game);
        }

        public string GetConnectionOpponent(string connectionId)
        {
            var game = _games.FirstOrDefault(x =>
                        x.Player1.Connections.First().ConnectionId == connectionId ||
                        x.Player2.Connections.First().ConnectionId == connectionId);

            if (game==null)
            {
                return null;
            }

            return game.Player1.Connections.First().ConnectionId == connectionId
                ? game.Player2.Connections.First().ConnectionId
                : game.Player1.Connections.First().ConnectionId;
        }
    }
}