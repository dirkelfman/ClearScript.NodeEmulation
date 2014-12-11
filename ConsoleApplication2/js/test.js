

var rateProvider = require('./rateprovider').getRateProvider();
//var process = require('process');

var props = {};


process.stdout.write('go...\r\n');
var rp = rateProvider.getRatesAsync(props, function () {
    process.stdout.write(props.joke);
    process.stdout.write('\r\n');
})


