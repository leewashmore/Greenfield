var ash = (function (ns) {

    ns.SerializerHelper = function () {
    };

    ns.SerializerHelper.prototype = {
        serializeArray: function (array, serializer, serializerContext, serializationStateOpt) {
            var result = [];
            for (var index = 0, length = array.length; index < length; index++) {
                result.push(serializer.call(serializerContext, array[index], serializationStateOpt));
            }
            return result;
        },
        serializeEditableValue: function (data, lastOneModified) {
            var result = {
                initialValue: data.initialValue,
                editedValue: data.currentValue(),
                lastOneModified: data === lastOneModified,
                comment: data.comment
            };
            return result;
        }
    };

    return ns;

})(ash || {});