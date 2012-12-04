ash.Bpt = (function (ns, ko) {

    ns.Serializer = function (helper) {
        this.helper = helper;
    };

    ns.Serializer.prototype = {

        serializers: {
            basketRegion: function (data, lastModified) {
                var result = {
                    _: data._,
                    basketId: data.basketId,
                    "base": this.helper.serializeEditableValue(data['base'], lastModified),
                    portfolioAdjustment: this.helper.serializeEditableValue(data.portfolioAdjustment, lastModified),
                    countries: this.serializeResidents(data.countries, this.serializers['country'], lastModified)
                };
                return result;
            },
            basketCountry: function (data, lastModified) {
                var result = { _: data._,
                    basketId: data.basketId,
                    country: this.serializeCountry(data.country, lastModified),
                    benchmark: data.benchmark,
                    "base": this.helper.serializeEditableValue(data['base'], lastModified),
                    overlay: data.overlay,
                    portfolioAdjustment: this.helper.serializeEditableValue(data.portfolioAdjustment, lastModified)
                };
                return result;
            },
            unsavedBasketCountry: function (data, lastModified) {
                var result = { 
                    _: data._,
                    country: this.serializeCountry(data.country, lastModified),
                    benchmark: data.benchmark,
                    "base": this.helper.serializeEditableValue(data['base'], lastModified),
                    overlay: data.overlay,
                    portfolioAdjustment: this.helper.serializeEditableValue(data.portfolioAdjustment, lastModified)
                };
                return result;
            },
            region: function (data, lastModified) {
                var result = {
                    _: data._,
                    name: data.name,
                    residents: this.serializeResidents(data.residents, null, lastModified)
                };
                return result;
            },
            other: function (data, lastModified) {
                var result = {
                    _: data._,
                    basketCountries: this.serializeResidents(data.basketCountries, null, lastModified),
                    unsavedBasketCountries: this.serializeResidents(data.unsavedBasketCountries, null, lastModified)
                };
                return result;
            },
            country: function (data, lastModified) {
                var result = {
                    _: data._,
                    country: this.serializeCountry(data.country, lastModified),
                    benchmark: data.benchmark,
                    overlay: data.overlay
                };
                return result;
            }
        },

        serializeRoot: function (data, lastModified) {
            var result = {
                targetingTypeId: data.targetingTypeId,
                portfolioId: data.portfolioId,
                latestChangesets: data.latestChangesets,
                root: this.serializeGlobe(data.root, lastModified),
                overlay: this.serializeOverlay(data.overlay, lastModified)
            };
            return result;
        },

        serializeGlobe: function (data, lastModified) {
            var result = {
                residents: this.serializeResidents(data.residents, null, lastModified)
            }
            return result;
        },

        serializeResidents: function (residents, serializer, lastModified) {
            var result = [];
            for (var index = 0, length = residents.length; index < length; index++) {
                var resident = residents[index];
                var value = (serializer || this.serializers[resident._]).call(this, resident, lastModified);
                result.push(value);
            }
            return result;
        },

        serializeCountry: function (data, lastModified) {
            var result = {
                isoCode: data.isoCode,
                name: data.name
            };
            return result;
        },

        serializeOverlay: function (data, lastModified) {
            var result = {
                items: this.helper.serializeArray(data.items, this.serializeOverlayItem, this, lastModified)
            };
            return result;
        },

        serializeOverlayItem: function (data, lastModified) {
            var result = {
                portfolioId: data.portfolio.id,
                overlayFactor: this.helper.serializeEditableValue(data.overlayFactor, lastModified)
            };
            return result;
        }
    };

    return ns;

})(ash.Bpt || {}, ko);