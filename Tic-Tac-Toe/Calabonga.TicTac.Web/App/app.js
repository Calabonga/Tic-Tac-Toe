var site = site || {};

site.hubs = site.hubs || {};
site.models = site.models || {};
site.controls = site.controls || {};

site.app = function () {
    var
        isMoveFirst = false,
        setFirstMove = function (value) {
            isMoveFirst = value;
        },
        markCell = function (index, myClick) {
            var cell = $('[data-cell="' + index + '"]');
            if (cell) {
                var marker;
                if (isMoveFirst) {
                    marker = myClick ? 'x' : 'o';
                } else {
                    marker = myClick ? 'o' : 'x';
                }
                $(cell).addClass(marker);
                $(cell).unbind('click');
            }
        },
        bindBoard = function () {
            var cells = $('[data-cell]');

            cells.on('click', function () {
                var index = $(this).data('cell');
                markMove(index, true);
            });
        },
        markMove = function (index, myClick) {
            markCell(index, myClick);
            site.hubs.connections.makeMove(index);
        };

    return {
        markCell: markCell,
        markMove: markMove,
        bindBoard: bindBoard,
        setFirstMove: setFirstMove
    }

}();

function guid() {
    return Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
};

//#region BusyIndicator

site.controls.BusyIndicator = function () {
    var ctrl = this;
    ctrl.uniqueId = guid();
    ctrl.isbusy = ko.observable(false);
    ctrl.modalCss = '<style type="text/css">' +
        ".modalBusy {" +
        'position:absolute;' +
        'z-index:9998;' +
        'margin-left:0;' +
        'top:0;' +
        'left:0;' +
        'height:100%;' +
        'width:100%;' +
        'background:rgba(0,0,0,.1)' +
        '</style>';
    ctrl.ctrlTemplate = function () {
        var modalDiv = '<div id="block' + ctrl.uniqueId + '" ' + 'class="modalBusy">&nbsp;</div>';

        return modalDiv;
    };
    ctrl.show = function () { ctrl.isbusy(true); };
    ctrl.hide = function () { ctrl.isbusy(false); };
    ctrl.init = function () {
        if (!window.hasModelBlocker) {
            $("head").append(ctrl.modalCss);
            window.hasModelBlocker = true;
        }
        return;
    }();

    return {
        template: ctrl.ctrlTemplate,
        uniqueId: ctrl.uniqueId,
        isbusy: ctrl.isbusy,
        show: ctrl.show,
        hide: ctrl.hide
    };
};

//#endregion 

//#region BlockUI

ko.bindingHandlers.blockUI = {
    init: function (element, valueAccessor) {

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var value = valueAccessor(),
            ctrl = ko.utils.unwrapObservable(value);
            var el = $("#block" + ctrl.uniqueId)[0];
            if (el) ko.removeNode(el);
        });
    },
    update: function (element, valueAccessor, allBindingAccessor) {
        var value = valueAccessor(),
            ctrl = ko.utils.unwrapObservable(value);
        $(element).css('position', 'relative');
        $(element).css('min-height', '70px');
        if (ctrl && ctrl.template) {
            var el = $("#block" + ctrl.uniqueId)[0];
            if (!el) {
                var block = ctrl.template(element);
                $(element).append(block);
                el = $("#block" + ctrl.uniqueId)[0];
            }
            if (ctrl.isbusy() && el) {
            } else {
                ko.removeNode(el);
            }
        }
    }
};
//#endregion