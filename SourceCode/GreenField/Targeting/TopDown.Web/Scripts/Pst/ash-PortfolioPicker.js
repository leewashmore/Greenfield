ash.Pst = (function (ns, ko, rns) {

    var utils = rns.Utils;

    ns.PortfolioPicker = function (repository) {
        this.repository = repository;

        this.portfolios = ko.observableArray([]);
        this.acceptedPortfolio = ko.observable(null);
        this.selectedPortfolio = ko.dependentObservable({
            read: function () {
                return this.acceptedPortfolio();
            },
            write: function (value) {
                var before = this.acceptedPortfolio();
                if (value === before) return;
                var decision = { cancelled: false };
                this.considerReseting(decision);
                if (decision.cancelled) {
                    this.acceptedPortfolio.valueHasMutated();
                } else {
                    this.acceptedPortfolio(value);
                    if (value != null) {
                        this.considerPicking(value);
                    }
                }
            }, owner: this
        });
        this.considerPicking = utils.signal();
        this.considerReseting = utils.signal();
        this.considerReady = utils.signal();
    };

    ns.PortfolioPicker.prototype = {
        start: function () {
            this.repository.requestBottomUpPortfolios(this.takeData, this);
        },
        takeData: function (data) {
            this.portfolios(data);
            this.considerReady();
        }
    };

    return ns;
})(ash.Pst || {}, ko, ash);