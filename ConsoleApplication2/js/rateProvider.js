function RateProvider () {
}

RateProvider.prototype.afterGetValues= function ( ctx, res )
	{
		res.Add(new Date().toString());
	};




RateProvider.prototype.getRates = function(rateDictionary) {
        var rateCollection = rateDictionary.rateCollection;
        rateCollection.Add({
            id: 'foo',
            name: 'canada ground',
            cost: 12.4
        });
        rateCollection.Add({
            id: 'foo2',
            name: 'canada air',
            cost: 56.4
        });
        return rateCollection;
    };

RateProvider.prototype.getRatesAsync = function (props, callback) {

    

    //var client = require('mozu-javascript-sdk').client({
    //    'appId': 'd4e9bb5.steve.1.0.0.release',
    //    'sharedSecret': '5d8cb5f6275b4a4bbd6a9e32557bb9be',
    //    'baseUrl': 'https://home.mozu.com/',

    //    'tenant': 3696,
    //    'master-catalog': 1,
    //    'tenantPod': 'https://t3696.sandbox.mozu.com'
    //});

    //debugger;
    //client.commerce().catalog().admin().product().getProducts().then(function () {
    //    props["joke"] = "bla";
    //    console.log(arguments);
    //    callback();
    //}, function () {
    //    callback();
    //});


    //request.debug = true;
    var request = require('request');
     request.post({
       
         uri: 'http://restmirror.appspot.com/',
         json: true,
         body:{ joke:'hahaha'},
        // json:true
     }, function (error, response, body) {

         props.joke = body.joke;

         callback(null);
     });

    //request({
    //    uri: 'http://www.omdbapi.com/?t=star%20wars&y=&plot=short&r=json',
    //    json:true
    //}, function(error, response, body) {
    //    if (!error && response.statusCode == 200) {
    //        //console.log(body); // Print the google web page.
    //        props.joke = body.Actors;
    //        callback();
    //    }
    //});


    //request({
    //    url: 'http://www.omdbapi.com/?t=star%20wars&y=&plot=short&r=json',
    //    method: 'GET',
    //    timeout: 10000,
    //    followRedirect: true,
    //    maxRedirects: 10,

    //}, function (error, response, body) {
    // //   console.log(body);
    //   // props.thing = body.actors;
    //    callback();
    //});
}
debugger;

var rateProvider = {
    RateProvider: RateProvider,
    getRatesAsync: new RateProvider().getRatesAsync,
    afterGetValues: new RateProvider().afterGetValues,
    doit: function (stuff) {
        debugger;
        return Object.create(stuff.prototype);
    }

};

module.exports = rateProvider;
