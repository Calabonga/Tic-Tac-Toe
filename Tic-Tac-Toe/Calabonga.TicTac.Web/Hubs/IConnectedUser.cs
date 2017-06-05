using System;
using System.Collections.Generic;

namespace Calabonga.TicTac.Web
{
	/// <summary>
	/// Контракт на тип подключенного пользователя
	/// </summary>
	public interface IConnectedUser {

        string Language { get; set; }

		string FullName { get; set; }

		DateTime ConnectedAt { get; set; }

		IEnumerable<ClientConnection> Connections { get; }

		void RegisterConnection(string connectionId, string agentType, string locale);

		void UnregisterConnection(string connectionId);
	}
}