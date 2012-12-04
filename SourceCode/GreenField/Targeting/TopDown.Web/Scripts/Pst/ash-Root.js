ash.Pst = (function (ns, ko, rns) {

	var utils = rns.Utils;

	ns.Root = function (repository, portfolioPicker, securityPicker, serializer) {
		this.repository = repository;
		this.considerLoading = utils.signal();
		this.considerLoaded = utils.signal();
		this.considerReporting = utils.signal();
		this.considerLeaving = utils.signal();
		this.portfolioPicker = portfolioPicker;
		portfolioPicker.considerPicking.add(this.takePortfolio, this);
		portfolioPicker.considerReady.add(this.considerLoaded, this);
		portfolioPicker.considerReseting.add(this.tryReset, this);

		this.securityPicker = securityPicker;
		this.securityPicker.considerAccepting.add(this.addSecurity, this);

		this.model = ko.observable(null);
		this.serializer = serializer;

		this.recalculateCallback = this.considerRecalculating.bind(this);
	};


	ns.Root.prototype = {

		start: function () {
			this.considerLoading();
			this.portfolioPicker.start();
		},

		takePortfolio: function (portfolio) {
			this.considerLoading();
			this.repository.requestPstModel(portfolio.id, this.takeData, this);
			this.model(null);
			this.lastPortfolio = portfolio;
		},

		reload: function () {
			var lastPortfolio = this.lastPortfolio;
			if (lastPortfolio == null) debugger;
			this.takePortfolio(lastPortfolio);
		},

		takeData: function (data) {
			var model = new ns.Model(data, this.recalculateCallback);
			this.model(model);
			this.lastModified = null;
			this.recalculating = false;
			this.considerLoaded();
		},

		addSecurity: function (security) {
			var model = this.model();
			model.addSecurity(security);
		},

		considerRecalculating: function (lastModified) {
			if (this.recalculating) return;
			this.recalculate(lastModified);
		},

		recalculate: function (lastModified) {
			this.recalculating = true;
			this.considerLoading();
			var model = this.model();
			var data = this.serializer.serializeComposition(model, lastModified);
			var json = JSON.stringify(data);
			this.repository.requestPstModelRecalculation(json, this.takeData, this);
		},

		save: function () {
			this.considerLoading();
			var model = this.model();
			var data = this.serializer.serializeComposition(model);
			var json = JSON.stringify(data);
			this.repository.requestToApplyPstModel(json, this.seeIfSaved, this);
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
			return model != null && model.unsaved;
		},

		tryReset: function (decision) {
			if (this.isModified()) {
				this.considerLeaving(decision);
			}
		}
	};





	return ns;
})(ash.Pst || {}, ko, ash);