var ash = (function (ns, ko, utils) {


    ns.SecurityPicker = function (repository) {
        this.repository = repository;
        this.considerAccepting = utils.signal();
        this.value = ko.observable(null);
        this.basketIdOpt = null;
    };

    ns.SecurityPicker.prototype = {

        requestOptions: function (input, autocompleteCallback) {
            var pattern = input.term;
            var atMost = 20;
            var basketIdOpt = this.basketIdOpt;
            if (basketIdOpt != null) {
                this.repository.requestSecuritiesInBasket(pattern, 20, basketIdOpt, this.takeOptions.bind(this, pattern, autocompleteCallback), this);
            } else {
                this.repository.requestSecurities(pattern, 20, this.takeOptions.bind(this, pattern, autocompleteCallback), this);
            }
        },

        takeOptions: function (pattern, autocompleteCallback, securities) {
            var items = [];
            for (var index = 0, length = securities.length; index < length; index++) {
                var security = securities[index];
                var value = security.shortName + ": " + security.name;
                var label = value;
                label = label.replace(new RegExp('\\b(' + pattern + ')', 'ig'), function (match, p1) {
                    return "<b>" + p1 + "</b>";
                });
                var item = {
                    value: value,
                    label: label,
                    security: security
                };
                items.push(item);
            }
            autocompleteCallback(items);
        },

        startTakingOption: function (e, ui) {
            window.setTimeout(this.finishTakingOption.bind(this, ui.item.security), 0);
        },

        finishTakingOption: function (security) {
            this.considerAccepting(security);
            this.value(Math.random());
            this.value(null);
        }

    };

    return ns;

})(ash || {}, ko, ash.Utils);