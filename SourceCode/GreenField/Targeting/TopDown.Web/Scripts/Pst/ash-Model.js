ash.Pst = (function (ns, ko, rns) {

    var utils = rns.Utils;

    ns.Model = function (data, recalculateCallback) {
        if (!recalculateCallback) debugger;
        this.recalculateCallback = recalculateCallback;

        this.portfolioId = data.portfolioId;
        this.latestChangeset = data.latestChangeset;

        this.unsaved = data.unsaved;

        var items = data.items;
        for (var index = 0, length = items.length; index < length; index++) {
            var item = items[index];
            this.enrichItem(item);
        }
        this.items = ko.observableArray(items);
        this.targetTotal = utils.makeValue(data.targetTotal);
    };

    ns.Model.prototype = {

        enrichItem: function (item) {
            item.target = new rns.Value(item.target, this.recalculateCallback);
        },

        createItem: function (security) {
            var result = {
                target: new rns.Value({ lastOneModified: true }, this.recalculateCallback),
                security: security
            };
            return result;
        },

        addSecurity: function (security) {
            var item = this.createItem(security);
            this.enrichItem(item);
            this.items.push(item);
        }

    };

    return ns;

})(ash.Pst || {}, ko, ash);