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


debugger;
    var client = require('mozu-node-sdk').client({
       
            'appId': 'd4e9bb5.nodetest.1.0.0.release',
            'appKey': 'd4e9bb5.nodetest.1.0.0.release',
            'sharedSecret': '0e159c92dd264bf4ab1df7a2435f301d',
            'baseUrl': 'https://home.mozu.com/',
            'tenantID':9105,
            'siteID':11579,
             masterCatalogID:1,
             catalogID:1,
            'tenant':9105,
            'site':11579,
             masterCatalog:1,
             catalog:1,
             tenantPod:'http://foot.com'
            //'developerAccountId': '001',
            //'developerAccount': {
            //    'emailAddress': 'example@volusion.com',
            //    'password': 'Password123!'
            //}
        
    });

    // client.setTenant(9105);
    // client.setSite(11579);
    // client.setMasterCatalog(1);
    // client.setCatalog(1);
    props.joke = 'hey';
    function log(result) {
        //console.log(util.inspect(result));
       // props.joke = result.content.productName;
        callback(null);
    }

    function reportError(error) {
        console.error(error.message, error);
        callback(error);
    }

    var productsClient= client.commerce().catalog().admin().product();
    productsClient.getProduct({
        productCode: 'MS-CAR-RAK-006'
    }).then(log, reportError);


    
    
    //var request = require('request');
    // request.post({

    //     uri: 'http://restmirror.appspot.com/',
    //     json: true,
    //     body:{ joke:'hahaha'},
    //    // json:true
    // }, function (error, response, body) {

    //     props.joke = body.joke;
    //     request = null;
    //     callback(null);
    // });

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
