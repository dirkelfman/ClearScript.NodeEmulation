function ActionFilters () {
}

ActionFilters.prototype.afterGetValues = function (ctx, res) {
        res.Add(new Date().toString());
	};







var actionFilters = {
    ActionFilters: ActionFilters,
    afterGetValues: function () {
        var filters = new ActionFilters();
        return filters.afterGetValues.apply(filters, arguments);
    } 
};

module.exports = actionFilters;
