var EventHandlerHelper =
    {
        addEventListener: function (elt, nomEvnt, handler) {
            if (elt.addEventListener)
                elt.addEventListener(nomEvnt, handler, false);
            else {
                if (elt.attachEvent)
                    elt.attachEvent("on" + nomEvnt, handler);
                else
                    elt["on" + nomEvnt] = handler;
            }
        },

        removeEventListener: function (elt, nomEvnt, handler) {
            if (elt.removeEventListener)
                elt.removeEventListener(nomEvnt, handler, false);
            else {
                if (elt.detachEvent)
                    elt.detachEvent("on" + nomEvnt, handler);
                else
                    elt["on" + nomEvnt] = null;
            }
        },

        fixEvent: function (event) {
            var evt = event ? event : window.event;
            if (!evt.bubbles) evt.bubbles = evt.cancelBubble;
            if (!evt.cancelable) evt.cancelable = evt.returnValue;
            if (!evt.currentTarget) evt.currentTarget = evt.srcElement;
            if (!evt.preventDefault) evt.preventDefault = function () { evt.returnValue = false; };
            if (!evt.stopPropagation) evt.stopPropagation = function () { evt.cancelBubble = true; };
            if (!evt.target) evt.target = evt.srcElement;
            if (!evt.view) evt.view = window;
            if (!evt.relatedTarget) evt.relatedTarget = evt.fromElement ? evt.fromElement : evt.toElement;
            if (!evt.which) evt.which = evt.keyCode != 0 ? evt.keyCode: evt.charCode;
            if (!evt.key) evt.key = String.fromCharCode(evt.which);
            return evt;
        }
    };