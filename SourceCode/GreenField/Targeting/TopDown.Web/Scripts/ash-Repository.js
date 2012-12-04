var ash = (function (ns, $) {

    var utils = ns.Utils;

    $.ajaxSetup({
        global: true,
        error: function (xhr, message, exception) {
            // console.log(xhr, message, exception);
        }
    });

    ns.Repository = function (baseUrl) {
        this.baseUrl = baseUrl;
        this.considerHandlingError = utils.signal();
    };

    ns.Repository.prototype = {

        request: function (path, data, callback, callbackContext) {
            var url = this.baseUrl + path;
            var options = {
                url: url,
                data: data,
                cache: false,
                contentType: 'application/x-www-form-urlencoded',
                context: callbackContext,
                dataType: 'json',
                success: this.unpackResponse.bind(this, callback, callbackContext),
                error: this.packError.bind(this),
                type: 'POST'
            };
            $.ajax(options);
        },

        packError: function (xhr, message, exception) {
            var error = {
                type: xhr.statusText,
                message: message,
                inner: {
                    type: 'details',
                    message: xhr.responseText
                }
            };
            this.considerHandlingError(error);
        },

        unpackResponse: function (callback, callbackContext, response) {
            if (!response) debugger;
            if (response.error != null) {
                this.considerHandlingError(response.error);
            } else if (response.data != null) {
                callback.call(callbackContext, response.data);
            }
        },

        requestTargetings: function (callback, callbackContext) {
            this.request('Get/Targetings', {}, callback, callbackContext);
        },

        requestBasketPicker: function (callback, callbackContext) {
            this.request('Get/BasketPicker', {}, callback, callbackContext);
        },

        requestBreakdown: function (targetingId, portfolioId, callback, callbackContext) {
            this.request('Get/Breakdown', { targetingId: targetingId, portfolioId: portfolioId }, callback, callbackContext);
        },

        requestBreakdownRecalculation: function (bptAsJson, callback, callbackContext) {
            this.request('Recalculate/Breakdown', { bptAsJson: bptAsJson }, callback, callbackContext);
        },

        requestToApplyBpt: function (bptAsJson, callback, callbackContext) {
            this.request('Apply/Bpt', { bptAsJson: bptAsJson }, callback, callbackContext);
        },

        requestSecurities: function (securityNamePattern, atMost, callback, callbackContext) {
            this.request('Get/Securities', { securityNamePattern: securityNamePattern, atMost: atMost }, callback, callbackContext);
        },

        requestBottomUpPortfolios: function (callback, callbackContext) {
            this.request('Get/BottomUpPortfolios', {}, callback, callbackContext);
        },

        requestSecuritiesInBasket: function (securityNamePattern, atMost, basketId, callback, callbackContext) {
            this.request('Get/SecuritiesInBasket', { securityNamePattern: securityNamePattern, atMost: atMost, basketId: basketId }, callback, callbackContext);
        },

        // P-S-T

        requestPstModel: function (portfolioId, callback, callbackContext) {
            this.request('Get/PstModel', { portfolioId: portfolioId }, callback, callbackContext);
        },

        requestPstModelRecalculation: function (pstAsJson, callback, callbackContext) {
            this.request('Recalculate/PstModel', { pstAsJson: pstAsJson }, callback, callbackContext);
        },

        requestToApplyPstModel: function (pstAsJson, callback, callbackContext) {
            this.request('Apply/PstModel', { pstAsJson: pstAsJson }, callback, callbackContext);
        },

        // B-P-S-T

        requestBpstModel: function (targetingTypeGroupId, basketId, callback, callbackContext) {
            this.request('Get/BpstModel', { targetingTypeGroupId: targetingTypeGroupId, basketId: basketId }, callback, callbackContext);
        },

        requestBpstModelRecalculation: function (bpstModelAsJson, callback, callbackContext) {
            this.request('Recalculate/BpstModel', { bpstModelAsJson: bpstModelAsJson }, callback, callbackContext);
        },

        requestToApplyBpstModel: function (json, callback, callbackContext) {
            this.request('Apply/Bpst', { bpstAsJson: json }, callback, callbackContext);
        }
    };

    return ns;

})(ash || {}, jQuery);