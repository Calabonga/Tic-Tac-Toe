site.models.User = function(json) {
    var me = this;
    me.UserName = json.FullName;
    me.CanInvite = json.FullName !== window.userName;
    me.ConnectionId = json.Connections[0].ConnectionId;
    return me;
}