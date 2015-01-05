

var rateProvider = require('./rateprovider');
//var process = require('process');
var client = require ('mozu-javascript-sdk').client({
    "appId": "d4e9bb5.steve.1.0.0.release",
    "sharedSecret": "5d8cb5f6275b4a4bbd6a9e32557bb9be",
    "baseUrl": "https://home.mozu.com/",

    "tenant":3696,
    "master-catalog":1,
    "tenantPod": "https://t3696.sandbox.mozu.com"
});


  client.commerce().catalog().admin().product().getProducts().then( console.log);


var props = {};


console.log(client);


process.stdout.write('go...\r\n');
var rp = rateProvider.getRatesAsync(props, function () {

    console.log(props.joke);
    
})


