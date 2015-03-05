

var rateProvider = require('./rateprovider');
debugger;
var props={};
process.stdout.write('go...\r\n');
var fn = function(){
	rateProvider.getRatesAsync(props,function(){
		debugger;
		console.log(props);
		fn();
	});
};


fn();