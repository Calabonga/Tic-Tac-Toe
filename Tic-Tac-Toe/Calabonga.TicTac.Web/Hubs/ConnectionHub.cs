using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Calabonga.TicTac.Resx;
using Calabonga.TicTac.Web.infrastructure;
using Microsoft.AspNet.SignalR;

namespace Calabonga.TicTac.Web
{
    [Authorize]
    public class ConnectionHub : Hub
    {
        private readonly UserConnectionManager _connectionManager;
        private readonly GameManager _gameManager;
        private readonly ICookieService _cookieService;

        private const string GroupName = "TicTac";

        // DI Container enabled
//        public ConnectionHub() : this(UserConnectionManager.Instance, GameManager.Instance, new CookieService()) { }

        public ConnectionHub(/*UserConnectionManager connectionManager, GameManager gameManager, ICookieService cookieService*/ 
            ILifetimeScope scope  )
        {

            _connectionManager = scope.Resolve<UserConnectionManager>();// connectionManager;
            _gameManager = scope.Resolve<GameManager>();//gameManager;
            _cookieService = scope.Resolve<ICookieService>(); //cookieService;
            
        }

        public void Invite(string connectionId, string userName)
        {
            var inviter = _connectionManager.GetUserByUserName(userName);
            var invited = _connectionManager.GetUserByConnectionId(connectionId);
            Clients.Client(connectionId).Invite(userName, invited, inviter);
        }

        public void Busy(string invited, string inviter)
        {
            var message = string.Format(Resource.UserIsBusy, _connectionManager.GetUserByUserName(invited).FullName);
            Clients.Client(_connectionManager.GetUserByUserName(inviter).Connections.First().ConnectionId).Busy(message);
        }

        public void SetBusy(string connection1, string connection2)
        {
            Clients.Client(connection2).setBusy();
        }

        public void UserEnterOrLeave()
        {
            Clients.All.UserEnterOrLeave(_connectionManager.Users);
        }

        public override Task OnConnected()
        {
            var agentTypeString = Context.QueryString["AgentType"];
            if (string.IsNullOrWhiteSpace(agentTypeString)) return null;
            {
                var fullUserName = Context.User != null ? Context.User.Identity.Name : "anonymous";
                var locale = _cookieService.GetCookie(WebApp.LanguageCookieName);
                UserConnectionManager.Instance.ConnectUser(Context.ConnectionId, agentTypeString, fullUserName, locale);
                UserEnterOrLeave();
                Groups.Add(Context.ConnectionId, GroupName);
                return base.OnConnected();
            }
        }


        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully;
        ///             false, if the connection has been lost for longer than the
        ///             <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout"/>.
        ///             Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.
        ///             </param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task"/>
        /// </returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionOpponent = _gameManager.Manager.GetConnectionOpponent(Context.ConnectionId);
            if (!string.IsNullOrWhiteSpace(connectionOpponent))
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

                hubContext.Clients.Client(connectionOpponent).GameOver(Resource.OpponentLoser);
            }
            _gameManager.Manager.Delete(Context.ConnectionId);
            UserConnectionManager.Instance.DisconnectUser(Context.ConnectionId);
            UserEnterOrLeave();
            Groups.Remove(Context.ConnectionId, GroupName);
            return base.OnDisconnected(stopCalled);
        }
    }
}