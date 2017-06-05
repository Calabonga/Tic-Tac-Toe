using System;
using System.Collections.Generic;
using System.Linq;

namespace Calabonga.TicTac.Web
{

    /// <summary>
    /// Подключенный пользователь к SignalR
    /// </summary>
    public class ConnectedUser : IConnectedUser
    {

        private readonly List<ClientConnection> _connections = new List<ClientConnection>();

        public string Language { get; set; }

        public string FullName { get; set; }

        public IEnumerable<ClientConnection> Connections
        {
            get { return _connections; }
        }

        public void RegisterConnection(string connectionId, string agentType, string locale)
        {
            var connection = new ClientConnection
            {
                AgentType = int.Parse(agentType),
                ConnectionId = connectionId,
                Language = locale
            };
            _connections.Add(connection);
        }

        public void UnregisterConnection(string connectionId)
        {
            var connection = _connections.SingleOrDefault(x => x.ConnectionId.Equals(connectionId));
            if (connection == null) return;
            _connections.Remove(connection);
        }

        public DateTime ConnectedAt { get; set; }
    }
}