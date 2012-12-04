ash.Pst = (function (ns, ko) {


	ns.Serializer = function (helper) {
		this.helper = helper;
	};

	ns.Serializer.prototype = {

		serializeComposition: function (data, lastModified) {
			var result = {
				portfolioId: data.portfolioId,
				latestChangeset: this.serializeLatestChanges(data.latestChangeset),
				items: this.serializeItems(data.items(), lastModified)
			};
			return result;
		},

		serializeLatestChanges: function (data) {
			return data;
		},

		serializeItems: function (data, lastModified) {
			var result = [];
			for (var index = 0, length = data.length; index < length; index++) {
				var item = data[index];
				result.push({
					securityId: item.security.id,
					target: this.helper.serializeEditableValue(item.target, lastModified)
				});
			}
			return result;
		} 
	};


	return ns;

})(ash.Pst || {}, ko);