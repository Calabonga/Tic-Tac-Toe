using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using Autofac;
using Calabonga.TicTac.Resx;
using Calabonga.TicTac.Web.infrastructure;
using Microsoft.AspNet.SignalR;

namespace Calabonga.TicTac.Web
{
    public class GameHub : Hub
    {
        private readonly GameManager _gameManager;
        private readonly UserConnectionManager _connectionManager;
        private readonly ICookieService _cookieService;

        // DI Container enabled
        //public GameHub() : this(GameManager.Instance, UserConnectionManager.Instance, new CookieService()) { }

        public GameHub(ILifetimeScope scope)
        {
            _connectionManager = scope.Resolve<UserConnectionManager>();// connectionManager;
            _gameManager = scope.Resolve<GameManager>();//gameManager;
            _cookieService = scope.Resolve<ICookieService>(); //cookieService;
        }

        private void OnGameStarted(GameInfo gameInfo)
        {
            var connections = gameInfo.Player1.Connections.Concat(gameInfo.Player2.Connections).Distinct();
            foreach (var connection in connections)
            {
                Clients.Client(connection.ConnectionId).GameStarted(gameInfo);
            }
        }

        public void StartGame(string player1, string player2)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(player1));
            Contract.Assert(!string.IsNullOrWhiteSpace(player2));
            var user1 = (ConnectedUser)_connectionManager.GetUserByUserName(player1);
            var user2 = (ConnectedUser)_connectionManager.GetUserByUserName(player2);
            var game = new GameInfo { Player1 = user1, Player2 = user2, StartedAt = DateTime.Now };
            _gameManager.Manager.AddGame(game);
            OnGameStarted(game);
            GamesActivity();
        }

        public void Turn(string player, string cell)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(player));
            Contract.Assert(!string.IsNullOrWhiteSpace(cell));
            Contract.Assert(int.Parse(cell) >= 0);

            var game = _gameManager.Manager.Games.FirstOrDefault(x => x.Player1.FullName == player || x.Player2.FullName == player);

            if (game != null)
            {
                int value;
                string opponentConnectionId;
                string myConnectionId;
                string myLocale;
                string opponentLocale;
                var turnByPlayer1 = game.Player1.FullName == player;
                if (turnByPlayer1)
                {
                    value = 1;
                    opponentConnectionId = game.Player2.Connections.First().ConnectionId;
                    myConnectionId = game.Player1.Connections.First().ConnectionId;
                    myLocale = game.Player1.Connections.First().Language;
                    opponentLocale = game.Player2.Connections.First().Language;
                }
                else
                {
                    value = 2;
                    opponentConnectionId = game.Player1.Connections.First().ConnectionId;
                    myConnectionId = game.Player2.Connections.First().ConnectionId;
                    myLocale = game.Player2.Connections.First().Language;
                    opponentLocale = game.Player1.Connections.First().Language;
                }
                var nextMove = game.CanMove(int.Parse(cell), value);
                Clients.Client(opponentConnectionId).Turn(cell);
                if (nextMove.CanMove)
                {
                    return;
                }
                _gameManager.Manager.GameOver(game);

                if (nextMove.HasWinner)
                {
                    var messageWinner = Resource.ResourceManager.GetString("YouAreAWinner", new CultureInfo(myLocale));
                    Clients.Client(myConnectionId).GameOver(messageWinner);
                    var messageLoser = Resource.ResourceManager.GetString("YouAreLoose", new CultureInfo(opponentLocale));
                    Clients.Client(opponentConnectionId).GameOver(messageLoser);
                }
                else
                {
                    var messageNo1 = Resource.ResourceManager.GetString("NoWinner", new CultureInfo(myLocale));
                    Clients.Client(myConnectionId).GameOver(messageNo1);
                    var messageNo2 = Resource.ResourceManager.GetString("NoWinner", new CultureInfo(opponentLocale));
                    Clients.Client(opponentConnectionId).GameOver(messageNo2);
                }

            }
        }

        public void GamesActivity()
        {
            Clients.All.GamesActivity(_gameManager.Manager.Games);
        }

    }

}