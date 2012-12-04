var ash = (function (ns, ko) {

	var utils = ns.Utils;
	var atleastNull = utils.atleastNull;

	

	ns.Value = function (data, recalculateCallback) {
		if (!recalculateCallback) debugger;
		this.recalculateCallback = recalculateCallback;
		this.initialValue = atleastNull(data.initialValue);
		this.editedValue = atleastNull(data.editedValue);
		this.lastOneModified = !!data.lastOneModified;
		this.comment = atleastNull(data.comment);
		var displayValue = data.displayValue;
		this.placeholder = displayValue == null ? '' : percent(data.displayValue);
		this.currentValue = ko.observable(this.editedValue);
		this.issues = data.issues;
		this.tooltip = createEditableTooltip(this);
		this.formattedValue = ko.dependentObservable({ read: utils.readValue, write: utils.writeValue, owner: this });
		this.callout = ko.observable(null);
	};

	ns.Value.prototype = {
		open: function () {
			this.closed = false;
			this.cancelClosingHandle = 0;
			this.callout(new ns.Callout(this));
		},
		startClosing: function (root) {
			this.cancelClosingHandle = window.setTimeout(this.close.bind(this, root), 100);
		},
		cancelClosing: function () {
			if (this.cancelClosingHandle) {
				window.clearTimeout(this.cancelClosingHandle);
			}
		},
		close: function (root) {
			if (this.closed) return;
			var valueBefore = this.editedValue;
			var valueAfter = this.currentValue();

			var callout = this.callout();
			if (callout) {
				var commentBefore = this.comment;
				var commentAfter = callout.value();
				this.comment = commentAfter;
				if (hasCommentChanged(commentBefore, commentAfter) || valueBefore !== valueAfter) {
					this.recalculateCallback(this);
					this.closed = true;
				}
				callout.dispose();
				this.callout(null);
			} else {
				if (valueBefore !== valueAfter) {
					this.recalculateCallback(this);
					this.closed = true;
				}
			}
		},
		cancel: function () {
			var callout = this.callout();
			if (callout != null) {
				callout.dispose();
			}
			this.callout(null);
		}
	};


	ns.Callout = function (model) {
		this.model = model;
		var before = this.before = model.comment;
		this.value = ko.observable(before);
	};

	ns.Callout.prototype = {
		open: function () {
			var model = this.model;
			if (model != null) {
				model.cancelClosing();
			}
		},
		close: function (root) {
			var model = this.model;
			if (model != null) {
				model.close(root);
			}
		},
		cancel: function () {
			var model = this.model;
			if (model != null) {
				model.cancel();
			}
		},
		dispose: function () {
			this.model = null;
		}
	};

	function createEditableTooltip(data) {
		var messages = [];
		var issues = data.issues || [];
		for (var index = 0, length = issues.length; index < length; index++) {
			messages.push(issues[index].message);
		}
		if (data.initialValue !== data.editedValue) {
			messages.push('Used to be: ' + percent(data.initialValue));
		}
		return messages.join('\r\n');
	}

	function hasCommentChanged(before, after) {
		if (before == null || before === '') {
			if (after == null || after === '') {
				return false;
			} else {
				return true;
			}
		} else {
			if (after == null || after === '') {
				return true;
			} else {
				return before !== after;
			}
		}
	}


	return ns;

})(ash || {}, ko);
