namespace Calabonga.TicTac.Web
{
	/// <summary>
	/// Клиентсткое подключение к SignalR
	/// </summary>
	public class ClientConnection {

	    public string Language { get; set; }

		/// <summary>
		/// Идентификатор подключения
		/// </summary>
		public string ConnectionId { get; set; }

		/// <summary>
		/// Тип клиента
		/// </summary>
		public int AgentType { get; set; }

		public override string ToString()
		{
			return string.Concat(AgentType.ToString(), ": ", ConnectionId);
		}
	}
}