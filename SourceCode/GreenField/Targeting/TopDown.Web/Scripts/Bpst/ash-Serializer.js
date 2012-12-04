ash.Bpst = (function (ns, rns) {

	var utils = rns.Utils;

	ns.Serializer = function (helper) {
		this.helper = helper;
	};
	ns.Serializer.prototype = {
		serializeRoot: function (data, lastModified, securityIdTobeAddedOpt) {
			var result = {
				latestBaseChangeset: data.latestBaseChangeset,
				latestPortfolioTargetChangeset: data.latestPortfolioTargetChangeset,
				targetingTypeGroupId: data.targetingTypeGroupId,
				basketId: data.basketId,
				securities: this.helper.serializeArray(data.securities(), this.serializeSecurity, this, lastModified),
				securityIdTobeAdded: utils.atleastNull(securityIdTobeAddedOpt),
				portfolios: this.helper.serializeArray(data.portfolios, this.serializePortfolio, this, lastModified)
			};
			return result;
		},
		serializeSecurity: function (data, lastModified) {
			var result = {
				securityId: data.security.id,
				base: this.helper.serializeEditableValue(data['base'], lastModified),
				benchmark: data.benchmark,
				portfolioTargets: this.helper.serializeArray(data.portfolioTargets, this.serializePortfolioTarget, this, lastModified)
			};
			return result;
		},
		serializePortfolio: function (data) {
			return {
				id: data.id
			};
		},
		serializePortfolioTarget: function (data, lastModified) {
			var result = {
				portfolioId: data.portfolioId,
				target: this.helper.serializeEditableValue(data.target, lastModified)
			};
			return result;
		}
	};

	return ns;
})(ash.Bpst || {}, ash);