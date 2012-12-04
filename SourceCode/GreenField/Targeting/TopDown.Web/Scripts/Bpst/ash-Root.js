ash.Bpst = (function (ns, ko, rns) {

	var utils = rns.Utils;

	ns.Root = function (repository, basketPicker, serializer, securityPicker) {

		this.considerLoading = utils.signal();
		this.considerLoaded = utils.signal();
		this.considerReporting = utils.signal();
		this.considerLeaving = utils.signal();

		this.serializer = serializer;
		this.repository = repository;

		basketPicker.considerReady.add(this.initializeBasketPicker, this);
		basketPicker.considerReseting.add(this.tryReset, this);
		basketPicker.considerPicking.add(this.takeInput, this);
		basketPicker.considerClearing.add(this.clear, this);
		this.basketPicker = basketPicker;

		this.securityPicker = securityPicker;
		securityPicker.considerAccepting.add(this.addSecurity, this)
		this.recalculateCallback = this.considerRecalculating.bind(this);
		this.model = ko.observable(null);
	};

	ns.Root.prototype = {
		start: function () {
			this.considerLoading();
			this.basketPicker.start();
		},
		initializeBasketPicker: function () {
			this.considerLoaded();
			this.basketPicker.pick(2, 9);
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
			this.securityPicker.basketIdOpt = null;
		},

		takeInput: function (targetingTypeGroup, basket) {
			this.considerLoading();
			this.lastTargetingTypeGroup = targetingTypeGroup;
			this.lastBasket = basket;
			var basketId = basket.id;
			this.repository.requestBpstModel(targetingTypeGroup.id, basketId, this.takeData, this);
			this.securityPicker.basketIdOpt = basketId;
		},
		reload: function () {
			var lastTargetingTypeGroup = this.lastTargetingTypeGroup;
			var lastBasket = this.lastBasket;
			if (lastTargetingTypeGroup == null || lastBasket == null) debugger;
			this.takeInput(lastTargetingTypeGroup, lastBasket);
		},
		takeData: function (data) {
			var model = new ns.Model(data, this.recalculateCallback);
			this.model(model);
			utils.fixPlaceholderAttributeForIE();
			this.recalculating = false;
			this.considerLoaded();
		},
		considerRecalculating: function (lastModified) {
			if (this.recalculating) return;
			this.recalculate(lastModified);
		},
		recalculate: function (lastModifiedOpt, securityIdOpt) {
			this.recalculating = true;
			this.considerLoading();
			var model = this.model();
			var data = this.serializer.serializeRoot(model, lastModifiedOpt, securityIdOpt);
			var bpstModelAsJson = JSON.stringify(data);
			this.repository.requestBpstModelRecalculation(bpstModelAsJson, this.takeData, this);
		},
		addSecurity: function (security) {
			this.recalculate(null, security.id);
		},
		save: function () {
			this.considerLoading();
			var model = this.model();
			var data = this.serializer.serializeRoot(model);
			var json = JSON.stringify(data);
			this.repository.requestToApplyBpstModel(json, this.seeIfSaved, this);
		},
		savingPrevented: function () {
			return !this.isModified();
		},
		seeIfSaved: function (data) {
			this.considerLoaded();
			this.considerReporting(data, this.reload.bind(this));
		}
	};

	return ns;

})(ash.Bpst || {}, ko, ash);