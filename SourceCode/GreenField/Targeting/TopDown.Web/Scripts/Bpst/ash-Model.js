ash.Bpst = (function (ns, ko, rns) {

    var utils = rns.Utils;

    ns.Model = function (data, recalculateCallback) {
        if (!recalculateCallback) debugger;
        this.recalculateCallback = recalculateCallback;
        this.unsaved = data.unsaved;
        this.latestBaseChangeset = data.latestBaseChangeset;
        this.latestPortfolioTargetChangeset = data.latestPortfolioTargetChangeset;
        this.targetingTypeGroupId = data.targetingTypeGroupId;
        this.basketId = data.basketId;
        var securities = data.securities;
        this.portfolios = this.createPortfolios(data.portfolios);
        for (var index = 0, length = securities.length; index < length; index++) {
            var security = securities[index];
            this.enrichItem(security);
        }
        this.securities = ko.observableArray(securities);
        this.baseTotal = utils.makeValue(data.baseTotal);
    };

    ns.Model.prototype = {
        enrichItem: function (item) {
            item['base'] = new rns.Value(item['base'], this.recalculateCallback);
            var portfolioTargets = item.portfolioTargets;
            for (var index = 0, length = portfolioTargets.length; index < length; index++) {
                var portfolioTarget = portfolioTargets[index];
                portfolioTarget.target = new rns.Value(portfolioTarget.target, this.recalculateCallback);
            }
        },
        createPortfolios: function (data) {
            var result = [];
            for (var index = 0, length = data.length; index < length; index++) {
                var item = data[index];
                utils.makeValue(item.portfolioTargetTotal);
                result.push(item);
            }
            return result;
        },
        createSecurity: function (security) {
            var result = {
                "base": new rns.Value({ lastOneModified: true }, this.recalculateCallback),
                benchmark: { value: 0.0 },
                security: security,
                portfolioTargets: this.createPortfolioTargets()
            };
            return result;
        },
        createPortfolioTargets: function () {
            var result = [];
            var portfolios = this.portfolios;
            for (var index = 0, length = portfolios.length; index < length; index++) {
                var portfolio = portfolios[index];
                result.push({
                    portfolioId: portfolio.id,
                    target: new rns.Value({}, this.recalculateCallback)
                });
            }
            return result;
        },

        isModified: function () {
            return this.unsaved;
        }
    };

    return ns;

} (ash.Bpst || {}, ko, ash));