var ash = (function (ns, ko, $, Modernizr) {

	ns.Utils = {
		signal: signal,
		makeValue: makeValue,
		readValue: readValue,
		writeValue: writeValue,
		fixPlaceholderAttributeForIE: fixPlaceholderAttributeForIE,
		atleastNull: atleastNull
	};

	return ns;

	function atleastNull(value) {
		// guarantees that value won't be 'undefined', because undefined values are not serialized
		return value == null ? null : value;
	}

	function makeValue(expression) {
		expression.formattedValue = percent(expression.value);
		expression.tooltip = createIssuesTooltip(expression);
		return expression;
	}

	function createIssuesTooltip(data) {
		var messages = [];
		var issues = data.issues || [];
		for (var index = 0, length = issues.length; index < length; index++) {
			messages.push(issues[index].message);
		}
		return messages.join('\r\n');
	}

	function readValue() {
		return percent(this.currentValue());
	}

	function writeValue(value) {
		if (value == null || value == "") {
			this.currentValue(null);
		} else {
			var number = Number(value);
			if (isNaN(number)) {
				// shakeing off a bad value (getting the cell to orginal state)
				var value = this.currentValue();
				this.currentValue(Number.NaN);
				this.currentValue(value);
			} else {
				number = (number / 100).toFixed(5);
				this.currentValue(number);
			}
		}
	}

	function signal() {
		var index = 0, all = {};
		S.add = add;
		return S;
		function add(listener, listenerContext) {
			var one = {
				listener: listener,
				listenerContext: listenerContext
			};
			var key = '_' + index;
			all[key] = one;
			index++;
			return function () {
				var one = all[key];
				all[key] = one = one.listenerContext = one.listener = null;
				delete all[key];
			};
		}
		function S() {
			for (var key in all) {
				if (!all.hasOwnProperty(key)) continue;
				var one = all[key];
				var args = Array.prototype.splice.call(arguments, 0, arguments.length);
				var listener = one.listener;
				var listenerContext = one.listenerContext;
				if (typeof listener === 'undefined') debugger;
				listener.apply(listenerContext, args);
			}
		}
	}


	// fixing placeholder in IE7,8,9,...
	function fixPlaceholderAttributeForIE() {
		if (!Modernizr.input.placeholder) {
			$("input").each(function () {
				var className = 'placeholdered';
				var wrapped = $(this);
				var dummy = wrapped.attr('placeholder');
				if (wrapped.val() == '' && dummy != '') {
					wrapped.val(dummy).addClass(className);
					wrapped.focus(function () {
						if (wrapped.val() == dummy) {
							wrapped.val('').removeClass(className);
						}
					}).blur(function () {
						if (wrapped.val() == '') {
							wrapped.val(dummy).addClass(className);
						}
					});
				}
			});
		}
	}



})(ash || {}, ko, jQuery, Modernizr);