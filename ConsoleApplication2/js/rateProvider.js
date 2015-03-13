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




    var crypto = require('crypto');
    var assert = require('assert');
    debugger;
    //var md5sum = crypto.createHash('md5');
    //md5sum.update("password1234");
    //var d = md5sum.digest('hex');
   
    //assert.equal(d, 'bdc87b9c894da5168059e00ebffb9077');


    var shaSum = crypto.createHash('sha256');
    var shaBuff = new Buffer('password1234');
    var g=shaBuff[0];
    shaSum.update(shaBuff);
    var shawHash = shaSum.digest('hex');

    //assert.equal(shawHash, 'b9c950640e1b3740e98acb93e669c65766f6670dd1609ba91ff41052ba48c6f3');

    console.log(shawHash);

    //process.nextTick(function () {
    //    callback(null, shawHash);
    //});

    //debugger;
    var client = require('mozu-node-sdk').client();
    function log(result) {
        callback(null);
    }

    function reportError(error) {
        console.error(error&& error.stack ? error.stack : error);
        callback(null);
    }

    var productsClient= client.commerce().catalog().admin().product();
    productsClient.getProduct({
        productCode: 'MS-CAR-RAK-006'
    }).then(log, reportError);


}


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
