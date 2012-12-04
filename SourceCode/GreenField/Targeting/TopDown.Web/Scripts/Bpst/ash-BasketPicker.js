ash.Bpst = (function (ns, ko, rns) {

    var utils = rns.Utils;

    ns.BasketPicker = function (repository) {

        this.considerReady = utils.signal();
        this.considerReseting = utils.signal();
        this.considerPicking = utils.signal();
        this.considerClearing = utils.signal();

        this.repository = repository;

        this.targetingTypeGroups = ko.observable(null);
        this.baskets = ko.observable(null);

        this.acceptedTargetingTypeGroup = ko.observable(null);
        this.selectedTargetingTypeGroup = ko.dependentObservable(
        {
            read: function () {
                return this.acceptedTargetingTypeGroup();
            },
            write: function (value) {
                var before = this.acceptedTargetingTypeGroup();
                if (before === value) return;

                var decision = { cancelled: false };
                this.considerReseting(decision);
                if (decision.cancelled) {
                    this.acceptedTargetingTypeGroup.valueHasMutated();
                } else {
                    if (value == null) {
                        this.baskets([]);
                        this.acceptedTargetingTypeGroup(null);
                        this.acceptedBasket(null);
                        this.considerClearing();
                    } else {
                        this.acceptedTargetingTypeGroup(value);
                        this.baskets(value.baskets);
                    }
                }
            },
            owner: this
        });

        this.acceptedBasket = ko.observable(null);
        this.selectedBasket = ko.dependentObservable({
            read: function () {
                return this.acceptedBasket();
            },
            write: function (value) {
                var before = this.acceptedBasket();
                if (value === before) return;

                var decision = { cancelled: false };
                this.considerReseting(decision);
                if (decision.cancelled) {
                    this.acceptedBasket.valueHasMutated();
                } else {
                    this.acceptedBasket(value);
                    if (value != null) {
                        var targetingTypeGroup = this.selectedTargetingTypeGroup();
                        this.considerPicking(targetingTypeGroup, value);
                    } else {
                        this.considerClearing();
                    }
                }
            },
            owner: this
        });
    };


    ns.BasketPicker.prototype = {

        start: function () {
            this.repository.requestBasketPicker(this.takeData, this);
        },

        takeData: function (data) {
            this.targetingTypeGroups(data.groups);
            this.considerReady();
        },

        pick: function (targetingTypeGroupIndex, basketIndex) {
            var targetingTypeGroup = this.targetingTypeGroups()[targetingTypeGroupIndex];
            var basket = targetingTypeGroup.baskets[basketIndex];

            this.selectedTargetingTypeGroup(targetingTypeGroup);
            this.selectedBasket(basket);
        }
    };

    return ns;

})(ash.Bpst || {}, ko, ash);