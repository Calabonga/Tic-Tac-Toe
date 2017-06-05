$(function () {

    'use strict';

    site.hubs.connections = function () {
        $.connection.hub.qs = { 'agentType': '0' };
        var
            gameHub = $.connection.gameHub,
            hub = $.connection.connectionHub,
            totalUsers = ko.observable(0),
            games = ko.observableArray([]),
            currentGame = ko.observable(),
            indicator = new site.controls.BusyIndicator(),
            connectedUsers = ko.observable([]),
            monitor = $('#connections'),
            monitorDiv = monitor.find('#totalUsers'),
            isPlaying = ko.observable(false),
            inviteRequest = ko.observable(null),
            init = function () {
                $.connection.hub.start().done(function () { });
            },
            invite = function (user) {
                hub.server.invite(user.ConnectionId, window.userName);
            },
            userBusy = function (invited, inviter) {
                hub.server.busy(invited, inviter);
            },
            setBusy = function (user1, user2) {
                gameHub.server.startGame(user1.FullName, user2.FullName);
                isPlaying(true);
                hub.server.setBusy(user1.Connections[0].ConnectionId, user2.Connections[0].ConnectionId);
                indicator.isbusy(true);
            },
            makeMove = function (index) {
                indicator.isbusy(true);
                gameHub.server.turn(window.userName, index);
            };


        gameHub.client.gameOver = function(message) {
            alert(message);
            isPlaying(false);
            currentGame(null);
            indicator.isbusy(false);
        }

        gameHub.client.turn = function (cell) {
            indicator.isbusy(false);
            site.app.markCell(cell, false);
        }

        gameHub.client.gamesActivity = function (listofGames) {
            games(listofGames);
        }

        gameHub.client.gameStarted = function (game) {
            currentGame(game);
            site.app.bindBoard();
        }

        hub.client.setBusy = function () {
            isPlaying(true);
            site.app.setFirstMove(true);
        }

        hub.client.busy = function (message) {
            alert(message);
        }

        hub.client.invite = function (who, invited, inviter) {
            if (!isPlaying()) {
                inviteRequest(new site.models.invite(who, function () {
                    site.app.setFirstMove(false);
                    setBusy(invited, inviter);
                    inviteRequest().reset();
                    inviteRequest(null);
                }, function () {
                    inviteRequest(null);
                }));

            } else {
                userBusy(invited.FullName, inviter.FullName);
            }
        };

        hub.client.userEnterOrLeave = function (users) {
            var current = totalUsers();
            var mapped = ko.utils.arrayMap(users, function (item) {
                return new site.models.User(item);
            });
            connectedUsers(mapped);
            if (current !== users.length) {
                totalUsers(users.length);
                if (users.length > 1) {
                    site.blink(monitorDiv, 300, 3);
                }
            }
        };

        init();

        return {
            games: games,
            invite: invite,
            makeMove: makeMove,
            isPlaying: isPlaying,
            indicator: indicator,
            totalUsers: totalUsers,
            currentGame: currentGame,
            inviteRequest: inviteRequest,
            connectedUsers: connectedUsers
        };
    }();

    ko.applyBindings(site.hubs.connections);
});