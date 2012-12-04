ash.Bpt = (function (ns, ko, rns) {

    var utils = rns.Utils;

    ns.PortfolioPicker = function (repository, considerTakingHandler) {

        this.considerReady = utils.signal();
        this.considerReseting = utils.signal();
        this.considerPicking = utils.signal();
        this.considerClearing = utils.signal();

        this.repository = repository;

        this.targetings = ko.observableArray([]);
        this.portfolios = ko.observableArray([]);


        this.acceptedTargeting = ko.observable(null);
        this.selectedTargeting = ko.dependentObservable({
            read: function (value) {
                return this.acceptedTargeting();
            }, write: function (value) {
                var before = this.acceptedTargeting();
                if (value === before) return;
                var decision = { cancelled: false };
                this.considerReseting(decision);
                if (decision.cancelled) {
                    this.acceptedTargeting.valueHasMutated();
                } else {
                    this.acceptedTargeting(value);
                    if (value != null) {
                        this.portfolios(value.portfolios);
                    } else {
                        this.portfolios([]);
                        this.considerClearing();
                    }
                }
            }, owner: this
        });

        this.acceptedPortfolio = ko.observable(null);
        this.selectedPortfolio = ko.dependentObservable({
            read: function () {
                return this.acceptedPortfolio();
            }, write: function (value) {
                var before = this.acceptedPortfolio();
                if (before === value) return;
                var decision = { cancelled: false };
                this.considerReseting(decision);
                if (decision.cancelled) {
                    this.acceptedPortfolio.valueHasMutated();
                } else {
                    this.acceptedPortfolio(value);
                    if (value != null) {
                        var targetingType = this.selectedTargeting();
                        this.considerPicking(targetingType, value);
                    } else {
                        this.considerClearing();
                    }
                }
            }, owner: this
        });
    };

    ns.PortfolioPicker.prototype = {
        start: function () {
            this.repository.requestTargetings(this.takeTargetings, this);
        },
        takeTargetings: function (data) {
            this.targetings(data);
            this.considerReady();
        },
        pick: function (targetingTypeIndex, portfolioIndex) {
            var targetingType = this.targetings()[targetingTypeIndex];
            var portfolio = targetingType.portfolios[portfolioIndex];

            this.selectedTargeting(targetingType);
            this.selectedPortfolio(portfolio);
        }
    };

    return ns;


})(ash.Bpt || {}, ko, ash);