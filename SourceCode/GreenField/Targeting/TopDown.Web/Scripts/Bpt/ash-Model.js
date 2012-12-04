ash.Bpt = (function (ns, ko, rns) {

    var utils = rns.Utils;

    var readValue = utils.readValue,
        writeValue = utils.writeValue,
        tooltip = utils.tooltip;


    ns.Model = function (data, recalculateCallback) {
        if (!recalculateCallback) debugger;
        this.recalculateCallback = recalculateCallback;

        this.data = data;
        var root = data.root;
        this.recalculateCallback = recalculateCallback;
        root.parent = { expanded: ko.observable(true), depth: -2 };
        root['_'] = 'root';
        this.initialize(this, root);
        this.initializeOverlay(data.overlay);
    };

    ns.Model.prototype = {

        initializers: {
            basketCountry: editableInitializer,
            unsavedBasketCountry: editableInitializer,
            basketRegion: function (breakdown, region) {
                editableInitializer.call(this, breakdown, region)
                regionInitializer.call(this, breakdown, region);
                region.expanded(false);
            },
            root: regionInitializer,
            region: regionInitializer,
            other: otherInitializer
        },

        initialize: function (data, current) {
            current.depth = current.parent.depth + 1;
            var initializer = this.initializers[current['_']];
            if (initializer) {
                initializer.call(this, data, current);
            }
        },

        initializeOverlay: function (data) {
            var items = data.items;
            for (var index = 0, length = items.length; index < length; index++) {
                var item = items[index];
                item.overlayFactor = new rns.Value(item.overlayFactor, this.recalculateCallback);
            }
        },

        isModified: function () {
            return this.data != null && this.data.unsaved;
        }
    };

    function otherInitializer(breakdown, other) {
        other.expandedValue = ko.observable(true);
        other.expanded = ko.dependentObservable({
            read: function () {
                return this.parent.expanded() && this.expandedValue()
            },
            write: function (value) {
                this.expandedValue(value);
            },
            owner: other
        });
        other.toggleExpanded = toggleExpanded.bind(other);

        each(other.basketCountries, function (item) {
            item.parent = other;
            this.initialize(breakdown, item);
        }, this);
        each(other.unsavedBasketCountries, function (item) {
            item.parent = other;
            this.initialize(breakdown, item);
        }, this);
    }

    function editableInitializer(breakdown, editable) {
        editable['base'] = new rns.Value(editable['base'], this.recalculateCallback);
        editable.portfolioAdjustment = new rns.Value(editable.portfolioAdjustment, this.recalculateCallback);
    }

    function regionInitializer(breakdown, region) {
        var residents = region.residents || region.countries;
        region.expandedValue = ko.observable(true);
        region.expanded = ko.dependentObservable({
            read: function () {
                return this.parent.expanded() && this.expandedValue()
            },
            write: function (value) {
                this.expandedValue(value);
            },
            owner: region
        });

        region.toggleExpanded = toggleExpanded.bind(region);

        for (var index = 0, length = residents.length; index < length; index++) {
            var resident = residents[index];
            resident.parent = region;
            this.initialize(breakdown, resident);
        }
    }



    return ns;

    function toggleExpanded() {
        this.expanded(!this.expandedValue());
    }


})(ash.Bpt || {}, ko, ash);