




function RateProvider() {

  




    this.getRates = function(rateDictionary) {
        var rateCollection = rateDictionary.rateCollection;
        rateCollection.Add({
            id:'foo', 
            name:'canada ground', 
            cost:12.4});
        rateCollection.Add({
            id:'foo2', 
            name:'canada air', 
            cost:56.4});
        return rateCollection;
    };

    this.getRatesAsync = function(props, callback ) {
     //   var rateCollection = rateDictionary.rateCollection;
       // var client = rateDictionary.client;

        // debugger;
        // var ggg = require('buffer').Buffer;
        // var g = new ggg();
        // var stuff = ggg.isEncoding('abc');

        // debugger;
        var request = require('request');

       

        request({
            url: 'http://api.icndb.com/jokes/random/1',
            json: true
        }, function (error, response, body) {
            props.joke= body.value[0].joke;
           
            callback();
        });


        /*
        var bing = require('http');

        bing.GetStringAsync('http://api.icndb.com/jokes/random/1')

        //        return client.commerce().catalog().admin().products().getProducts({
        return client.getProducts({
            query: 'productType eq blue'
        }).then(function(res) {
            return res.items[0];

        }).then(function (res) {
            rateCollection.Add({
                id: res.productCode,
                name: 'canada air',
                cost: res.price
            });
           
            callback();
        });*/
    };

}

var rateProviderFactory = {
    getRateProvider: function () {
        return new RateProvider();
    }

};

module.exports = rateProviderFactory;