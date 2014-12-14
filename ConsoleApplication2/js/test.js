

var rateProvider = require('./rateprovider');
//var process = require('process');

var props = {};


process.stdout.write('go...\r\n');
var rp = rateProvider.getRatesAsync(props, function () {

    console.log(props.joke);
    
})


