var builtinModules = (function() {

    // function ccnetBuffer() {}

    function inherits(ctor, superCtor) {
        ctor.super_ = superCtor;
        ctor.prototype = Object.create(superCtor.prototype, {
            constructor: {
                value: ctor,
                enumerable: false,
                writable: true,
                configurable: true
            }
        });
    }

    var container = {
        runtime: null,
        engine: null,
        require: null
    };
    var classes={};

    function EventEmitter() {
        this.ccInner = new ccNetEventEmitter();
    }
    EventEmitter.prototype.on = function (event, listner) {

        this.ccInner.on(event, function () {
          
            var arrArray = Array.prototype.slice.call(arguments, 0);
            for ( var i = 0; i < arrArray.length ; i++){
                if ( arrArray[i].clientWrapperClass)
                {
                    arrArray[i]  = new classes[arrArray[i].clientWrapperClass](arrArray[i]);
                }
            }
               
            listner.apply(arguments.callee, arrArray);
           

            return this;
        });
       
    };

    EventEmitter.prototype.once = function(event, listner) {
        return this.ccInner.once(event, listner);
    };

    EventEmitter.prototype.removeAllListeners = function(event) {
        return this.ccInner.removeAllListeners(event);
    };
    EventEmitter.prototype.setMaxListeners = function(num) {
        return this.ccInner.setMaxListeners(num);
    };
    EventEmitter.prototype.emit = function() {
        return this.ccInner.emit(arguments[1], Array.prototype.slice.call(arguments, 1));
    };

    function Buffer() {
        this.isBuffer = true;
        if (arguments.length === 0) {
            this.ccInner = new ccnetBuffer();
        } else if (arguments.length == 1) {
            this.ccInner = new ccnetBuffer(arguments[0] || null);
        } else if (arguments.length == 2) {
            new ccnetBuffer(arguments[0] || null, arguments[1] || null);
        }

        this.length = this.ccInner.length;
    }

    inherits(Buffer, EventEmitter);

    Buffer.isBuffer = true;

    Buffer.isBuffer = function(obj) {
        return obj && obj.isBuffer === true;
    };
    Buffer.concat = function(list) {
        return list.length ? list[0] : new Buffer();
    };
    Buffer.isEncoding = function(enc) {
        return true;
    };

    Buffer.prototype.slice = function(start, end) {
        return this.ccInner.slice(start, end);
    };
    Buffer.prototype.copy = function(target, target_start, start, end) {
        return this.ccInner.copy(target, target_start, start, end);
    };
    Buffer.prototype.toString = function(encoding, start, end) {
        return this.ccInner.toString(encoding || null, start || null, end || null);
    };

    function Agent() {

    }

    function request(options, callback) {
        return new Request(options, callback);
    }

    function httpsRequest(options, callback) {
        var req = new Request(options, callback);
        req.protocal = 'https';
    }


    function Request(options, callback) {
        this.ccInner = new ccnetHttpRequest(options, callback);
        this.ccInner.runtime = container.runtime;
        this.ccInner.engine = container.engine;
        this.ccInner.require = container.require;
    }

    inherits(Request, EventEmitter);

    Request.prototype.write = function(chunk, encoding) {
        chunk = !chunk ? null : chunk.ccInner ? chunk.ccInner : chunk;
        return this.ccInner.write(chunk, encoding || null);
    };
  

    Request.prototype.end = function(chunk, encoding) {
        chunk = !chunk ? null : chunk.ccInner ? chunk.ccInner : chunk;
        return this.ccInner.end(chunk, encoding || null);
    };
    Request.prototype.abort = function() {
        return this.ccInner.abort();
    };

    


    Request.prototype.setTimeout = function(timeout, callback) {
        return this.ccInner.setTimeout(timeout, callback || null);
    };
    Request.prototype.setNoDelay = function(nodelay) {
        return this.ccInner.setNoDelay(nodelay);
    };

    function IncommingMessage(ccInner) {
        this.ccInner = ccInner;
        //this.httpVersion = ccInner.httpVersion;
        this.headers = ccInner.headers;
        this.statusCode = ccInner.status;
        this.isBuffer = true;
        this.length = ccInner.length;
    }
    inherits(IncommingMessage, Buffer);
    //inherits(IncommingMessage, EventEmitter);

    var STATUS_CODES = {
        '100': 'Continue',
        '101': 'Switching Protocols',
        '102': 'Processing',
        '200': 'OK',
        '201': 'Created',
        '202': 'Accepted',
        '203': 'Non-Authoritative Information',
        '204': 'No Content',
        '205': 'Reset Content',
        '206': 'Partial Content',
        '207': 'Multi-Status',
        '300': 'Multiple Choices',
        '301': 'Moved Permanently',
        '302': 'Moved Temporarily',
        '303': 'See Other',
        '304': 'Not Modified',
        '305': 'Use Proxy',
        '307': 'Temporary Redirect',
        '400': 'Bad Request',
        '401': 'Unauthorized',
        '402': 'Payment Required',
        '403': 'Forbidden',
        '404': 'Not Found',
        '405': 'Method Not Allowed',
        '406': 'Not Acceptable',
        '407': 'Proxy Authentication Required',
        '408': 'Request Time-out',
        '409': 'Conflict',
        '410': 'Gone',
        '411': 'Length Required',
        '412': 'Precondition Failed',
        '413': 'Request Entity Too Large',
        '414': 'Request-URI Too Large',
        '415': 'Unsupported Media Type',
        '416': 'Requested Range Not Satisfiable',
        '417': 'Expectation Failed',
        '418': 'I\'m a teapot',
        '422': 'Unprocessable Entity',
        '423': 'Locked',
        '424': 'Failed Dependency',
        '425': 'Unordered Collection',
        '426': 'Upgrade Required',
        '428': 'Precondition Required',
        '429': 'Too Many Requests',
        '431': 'Request Header Fields Too Large',
        '500': 'Internal Server Error',
        '501': 'Not Implemented',
        '502': 'Bad Gateway',
        '503': 'Service Unavailable',
        '504': 'Gateway Time-out',
        '505': 'HTTP Version Not Supported',
        '506': 'Variant Also Negotiates',
        '507': 'Insufficient Storage',
        '509': 'Bandwidth Limit Exceeded',
        '510': 'Not Extended',
        '511': 'Network Authentication Required'
    };
    classes.Buffer = Buffer;
    classes.IncommingMessage = IncommingMessage;
    classes.Request = Request;
    classes.EventEmitter = EventEmitter;


    var modules = {
        container: container,
        require: function(id) {

            var mod = modules[id];
            if (!mod ) {
                console.error(id + ' not found');
            }
            return mod;
        },
        httpHelper: {
            createIncomingMessage: function(ccInner) {
                return new IncommingMessage(ccInner);
            }
        },
        fs: {
            readSync: function() {
                return new Buffer();
            }
        },
        crypto: {

        },
        zlib: {

        },
        process: process,
        _process: process,
        util: util,
        buffer: {
            Buffer: Buffer,
            SlowBuffer: Buffer,
            INSPECT_MAX_BYTES: 50
        },
        events: {
            usingDomains: false,
            EventEmitter: EventEmitter
        },
        http: {
            Agent: Agent,
            request: request,
            STATUS_CODES: STATUS_CODES
        },
        https : {
            Agent: Agent,
            request: httpsRequest,
            get: httpsRequest
        }
    };
   
    return modules;

})();
require = builtinModules.require;
x = builtinModules;
