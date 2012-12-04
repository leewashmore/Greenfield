var ash = (function (ns, jQuery, ko) {

	ns.Page = function (model, window) {
		if (!model || !window) debugger;

		this.model = ko.observable(model);
		model.considerLoading.add(this.showCurtain, this);
		model.considerLoaded.add(this.hideCurtain, this);
		model.considerReporting.add(this.reportResults, this);
		if (model.considerLeaving) {
			model.considerLeaving.add(this.tryLeave, this);
		}
		this.issues = ko.observableArray(null);
		this.loading = ko.observable(false);
		this.error = ko.observable(null);
		$(window).on('beforeunload', this.hasChanges.bind(this));
	};

	ns.Page.prototype = {
		showCurtain: function () {
			this.loading(true);
		},
		hideCurtain: function () {
			this.loading(false);
		},
		reportResults: function (issues, reloadCallback) {
			this.lastReloadCallback = reloadCallback;
			this.issues(issues);
		},
		takeError: function (data) {
			data.expanded = ko.observable(false);
			data.toggleExpanded = function () {
				data.expanded(!data.expanded());
			};
			this.error(data);
		},
		closeError: function (data) {
			this.hideCurtain();
			this.error(null);
		},
		hideIssues: function () {
			var callback = this.lastReloadCallback;
			if (callback != null) {
				var issues = this.issues();
				if (!issues.length) {
					callback();
				}
				this.issues(null); // hide
			}
		},
		hasChanges: function (e) {
			var model = this.model();
			if (model != null && model.isModified()) {
				return 'Unsaved changes! Discard?';
			} else {
				/* return undefined; */
			}
		},
		isStaying: function () {
			var message = this.hasChanges();
			return message && !confirm(message);
		},
		tryLeave: function (decision) {
			decision.cancelled = this.isStaying();
		}
	};

	return ns;

})(ash || {}, $, ko);