function RateProvider() {


    this.getRates = function(rateDictionary) {
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

    this.getRatesAsync = function(props, callback) {

        var request = require('request');
        request.debug = true;
        request.post({
            proxy:'http://127.0.0.1:8888',
            uri: 'http://restmirror.appspot.com/',
            json: true,
            body:{ joke:'hahaha'},
           // json:true
        }, function (error, response, body) {
          
            props.joke = body.joke;
            
            callback();
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
    };

    //request({
    //    url: 'http://www.omdbapi.com/?t=star%20wars&y=&plot=short&r=json',
    //    method: "GET",
    //    timeout: 10000,
    //    followRedirect: true,
    //    maxRedirects: 10,

    //}, function (error, response, body) {
    // //   console.log(body);
    //   // props.thing = body.actors;
    //    callback();
    //});

}



var rateProvider = {
    RateProvider: RateProvider,
    getRatesAsync: new RateProvider().getRatesAsync

};

module.exports = rateProvider;
