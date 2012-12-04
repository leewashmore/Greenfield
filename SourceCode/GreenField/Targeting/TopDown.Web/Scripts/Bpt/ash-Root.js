ash.Bpt = (function (ns, ko, rns) {

    var utils = rns.Utils;

    ns.Root = function (repository, serializer, picker) {
        this.considerLoading = utils.signal();
        this.considerLoaded = utils.signal();
        this.considerReporting = utils.signal();
        this.considerLeaving = utils.signal();

        this.repository = repository;
        this.serializer = serializer;
        this.portfolioPicker = picker;
        picker.considerPicking.add(this.takePortfolio, this);
        picker.considerReady.add(this.initializeBasketPicker, this);
        picker.considerReseting.add(this.tryReset, this);
        picker.considerClearing.add(this.clear, this);
        this.model = ko.observable(null);
        this.recalculateCallback = this.considerRecalculating.bind(this);
    };

    ns.Root.prototype = {

        start: function () {
            this.considerLoading();
            this.portfolioPicker.start();
        },

        initializeBasketPicker: function () {
            this.considerLoaded();
            this.portfolioPicker.pick(3, 0);
        },

        takePortfolio: function (targeting, portfolio) {
            this.lastTargetingType = targeting;
            this.lastPortfolio = portfolio;
            this.considerLoading();
            this.repository.requestBreakdown(targeting.id, portfolio.id, this.takeData, this);
        },

        reload: function () {
            var lastTargetingType = this.lastTargetingType;
            var lastPortfolio = this.lastPortfolio;
            if (lastTargetingType == null || lastPortfolio == null) debugger;
            this.takePortfolio(lastTargetingType, lastPortfolio);
        },

        takeData: function (data) {
            var model = new ns.Model(data, this.recalculateCallback);
            this.model(model);
            this.recalculating = false;
            this.considerLoaded();
        },

        considerRecalculating: function (lastModified) {
            if (this.recalculating) return;
            this.recalculate(lastModified);
        },

        recalculate: function (lastModified) {
            this.considerLoading();
            this.recalculating = true;
            var model = this.model();
            var data = this.serializer.serializeRoot(model.data, lastModified);
            var json = JSON.stringify(data);
            this.repository.requestBreakdownRecalculation(json, this.takeData, this);
        },

        save: function () {
            this.considerLoading();
            var model = this.model();
            var data = this.serializer.serializeRoot(model.data);
            var json = JSON.stringify(data);
            this.repository.requestToApplyBpt(json, this.seeIfSaved, this);
        },

        savingPrevented: function () {
            return !this.isModified();
        },

        seeIfSaved: function (data) {
            this.considerLoaded();
            this.considerReporting(data, this.reload.bind(this));
        },

        isModified: function () {
            var model = this.model();
            return model != null && model.isModified();
        },

        tryReset: function (decision) {
            if (this.isModified()) {
                this.considerLeaving(decision);
            }
        },

        clear: function () {
            this.model(null);
        }
    };

    return ns;


})(ash.Bpt || {}, ko, ash);