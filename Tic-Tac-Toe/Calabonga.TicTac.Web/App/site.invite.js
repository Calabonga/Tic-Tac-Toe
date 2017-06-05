site.models.invite = function (json, ok, cancel) {
    var me = this;
    me.UserName = json;
    me.ok = function () {
        if (ok) ok();
    };
    me.timeout = 5;
    me.timer = ko.observable(5);
    me.cancel = function () {
        if (cancel) cancel();
    }
    me.time= setTimeout(function () {
        if (cancel)
            cancel();
    }, 5000);

    me.interval =setInterval(function () {
        me.timeout -= 1;
        me.timer(me.timeout);
    }, 1000);

    me.reset = function() {
        clearTimeout(me.time);
        clearInterval(me.interval);
    }

    return me;
};