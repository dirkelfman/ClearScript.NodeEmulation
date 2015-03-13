/* global  ccNetEventEmitter,ccnetTimers,ccnetBuffer,ccnetHttpRequest,ccnetProcess, util, ccnetHelpers */
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
    var classes = {};

    function EventEmitter() {
        this.ccInner = container.require.GetService('ccNetEventEmitter');
    }
    EventEmitter.prototype.on = function(event, listner) {

        this.ccInner.on(event, function() {

            var arrArray = Array.prototype.slice.call(arguments, 0);
            for (var i = 0; i < arrArray.length; i++) {
                if (arrArray[i].clientWrapperClass) {
                    arrArray[i] = new classes[arrArray[i].clientWrapperClass](arrArray[i]);
                }
            }

            listner.apply(arguments.callee, arrArray);


            return this;
        });

    };


    EventEmitter.prototype.addListener = EventEmitter.prototype.on;

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
        this.ccInner = container.require.GetService('ccnetBuffer');
        if (arguments.length === 0) {

        } else if (arguments.length == 1) {
            if (arguments[0] && arguments[0].clientWrapperClass == 'Buffer') {
                this.ccInner = arguments[0];
            } else {
                this.ccInner.Init(arguments[0] || null);
            }

        } else if (arguments.length == 2) {
            this.ccInner.Init(arguments[0] || null, arguments[1] || null);
        }

        // console.log("b-new", arguments);
    }

    inherits(Buffer, EventEmitter);




    Buffer.isBuffer = true;

    Buffer.isBuffer = function(obj) {
        return obj && obj.isBuffer === true;
    };
    Buffer.concat = function(list, totalLength) {

        if (list.length === 0) {
            return new Buffer(0);
        } else if (list.length === 1) {
            return list[0];
        }

        var i;
        if (totalLength === undefined) {
            totalLength = 0;
            for (i = 0; i < list.length; i++) {
                totalLength += list[i].length;
            }
        }

        var buf = new Buffer(totalLength);
        var pos = 0;
        for (i = 0; i < list.length; i++) {
            var item = list[i];
            item.copy(buf, pos);
            pos += item.length;
        }
        return buf;

        //if (!list.length) {
        //    return list[0];
        //}
        //var allManaged = !!list[0].ccInner,
        //    noneManaged = !list[0].ccInner
        //for (i = 1; i++; i < list.length) {
        //    allManaged = allManaged && !!list.ccInner;
        //    noneManaged = noneManaged && !list.ccInner[0];
        //}
        //if (allManaged) {
        //    var buffer = new Buffer(list[0]);

        //}
        //return list.length ? list[0] : new Buffer();
    };


    Buffer.isEncoding = function() {
        return true;
    };
    //Buffer.toString = function (encoding, start, end) {
    //    var ret = this.ccInner.toString(encoding || null, start || null, end || null);
    //    console.log("b-toString",arguments, ret);
    //    return ret;
    //}

    Object.defineProperty(Buffer.prototype, "length", {
        get: function() {
            return this.ccInner.Length;
        }
    });

    var createGetter = function(index) {
        return function() {
            if (this.ccInner) {
                return this.ccInner.get(index);
            }
        };
    };
    var createSetter = function(index) {
        return function(newValue) {
            if (this.ccInner) {
                this.ccInner.set(index, newValue);
            }
        };
    };
    for (var i = 0; i < 256; i++) {
        Object.defineProperty(Buffer.prototype, i, {
            get: createGetter(i),
            set: createSetter(i)
        });
    }


    Buffer.prototype.slice = function(start, end) {
        var ret = this.ccInner.slice(start || null, end || null);
        // console.log("b-slice", arguments, ret);
        return ret;
    };
    Buffer.prototype.copy = function(target, target_start, start, end) {
        if (target.ccInner) {
            var ret = this.ccInner.copy(target.ccInner, target_start || null, start || null, end || null);
            //    console.log("b-slice", arguments, ret);
            return ret;
        }

        var self = this; // source

        if (!start) start = 0;
        if (!end && end !== 0) end = this.length;
        if (target_start >= target.length) target_start = target.length;
        if (!target_start) target_start = 0;
        if (end > 0 && end < start) end = start;

        // Copy 0 bytes; we're done
        if (end === start) return 0;
        if (target.length === 0 || self.length === 0) return 0;

        // Fatal error conditions
        if (target_start < 0)
            throw new RangeError('targetStart out of bounds');
        if (start < 0 || start >= self.length) throw new RangeError('sourceStart out of bounds');
        if (end < 0) throw new RangeError('sourceEnd out of bounds');

        // Are we oob?
        if (end > this.length)
            end = this.length;
        if (target.length - target_start < end - start)
            end = target.length - target_start + start;

        var len = end - start;

        for (i = start, i < end; i++;) {
            target[i] = this.ccInner.get(i);
        }



        // console.log("b-slice", arguments, len);
        return len;
    };
    Buffer.prototype.toString = function(encoding, start, end) {
        var ret = this.ccInner._toString(encoding || null, start || null, end || null);

        //  console.log("b-toString", arguments, ret);
        return ret;
    };

    Buffer.prototype.fill = function(value, offset, end) {
        var ret = this.ccInner.fill(value || null, offset || null, end || null);
        //  console.log("b-fill", arguments, ret);
        return ret;
    };
    Buffer.prototype.readInt32BE = function(offset, noAssert) {
        var ret = this.ccInner.readInt32BE(offset || null, noAssert || false);
        //   console.log("b-readInt32BE", arguments, ret);
        return ret;
    };
    Buffer.prototype.readInt32LE = function(offset, noAssert) {
        var ret = this.ccInner.readInt32LE(offset || null, noAssert || false);
        //  console.log("b-readInt32LE", arguments, ret);
        return ret;
    };
    Buffer.prototype.writeInt32BE = function(value, offset, noAssert) {
        var ret = this.ccInner.writeInt32BE(value, offset || 0, noAssert || false);
        //   console.log("b-writeInt32BE", arguments, ret);
        return ret;
    };
    Buffer.prototype.writeInt32LE = function(value, offset, noAssert) {
        var ret = this.ccInner.writeInt32LE(value, offset || 0, noAssert || false);
        //   console.log("b-writeInt32LE", arguments, ret);
        return ret;
    };

    function Agent() {

    }



    function request(options, callback) {
        return new Request(options, callback);
    }

    function httpsRequest(options, callback) {
        var req = new Request(options, callback);
        req.ccInner.protocal = 'https';
        return req;
    }


    function Request(options, callback) {
        this.ccInner = container.require.GetService('ccnetHttpRequest');
        this.ccInner.Init(options, callback);

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


    var ccnetTimersInstance = null;

    var timers = {
        setTimeout: function() {
            var args = Array.prototype.slice(arguments, 2);
            var callback = arguments[0];
            var delay = arguments[1];
            if (!ccnetTimersInstance) {
                ccnetTimersInstance = container.require.GetService('ccnetTimers');
            }
            return ccnetTimersInstance.setTimeout(callback, delay, args);
        },
        clearTimeout: function(t) {
            if (!ccnetTimersInstance) {
                ccnetTimersInstance = container.require.GetService('ccnetTimers');
            }
            return ccnetTimersInstance.clearTimeout(t);
        }
    };



    function convertToJsArray(hostArray) {
        var result = [];
        for (var i = 0; i < hostArray.Length; i++)
            result.push(hostArray[i]);
        return result;
    }

    function convertToHostArray(jsArray) {
        jsArray = jsArray || [];
        var hostArray = ccnetHelpers.createObjectArray(jsArray.length);

        for (var i = 0; i < jsArray.Length; i++) {
            hostArray[i] = jsArray[i];
        }
        return hostArray;
    }


    function createArrayCallbackWrapper(hostFn) {
        return function() {
            var jArray = Array.prototype.slice.call(arguments, 0),
                hostArray = convertToHostArray(jArray);
            hostFn(hostArray);
        };
    }



    function IncomingMessage(ccInner) {
        this.ccInner = ccInner;
        //this.httpVersion = ccInner.httpVersion;
        this.headers = ccInner.headers;
        this.statusCode = ccInner.statusCode;

    }
    inherits(IncomingMessage, EventEmitter);



    IncomingMessage.prototype.setEncoding = function(enc) {
        this.ccInner.setEncoding(enc);
    };


    IncomingMessage.prototype.resume = function() {
        this.ccInner.resume();
    };

    IncomingMessage.prototype.pipe = function(dest, options) {
        return this.ccInner.pipe(dest, options ? options : null);
    };

    IncomingMessage.prototype.unpipe = function() {
        var args = Array.prototype.slice(arguments, 2);
        //todo unwind arrray to 
        return this.ccInner.unpipe(args);
    };



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
    classes.IncomingMessage = IncomingMessage;
    classes.Request = Request;
    classes.EventEmitter = EventEmitter;


    var process = {
        nextTick: function(callback) {
            if (!process.ccProcess) {
                process.ccProcess = container.require.GetService('ccnetProcess');
            }

            process.ccProcess.nextTick(function() {
                callback();
            });
        },
        env: {}
    };



    var modules = {
        container: container,
        require: function(id) {

            var mod = modules[id];
            if (!mod) {
                mod = container.require.GetService(id);
                if (mod === null) {
                    console.error('module:' + id + ' not found');
                }
            }
            return mod;
        },

        fs: {
            readSync: function() {

                return new Buffer();
            },
            readFileSync: function(fn) {
                if (fn.toLowerCase().indexOf('package.json') > -1) {
                    return new Buffer(JSON.stringify({
                        version: 1.0
                    }));
                }
                return new Buffer();
            }
        },
        clearCaseHelpers: {
            convertToJsArray: convertToJsArray,
            createArrayCallbackWrapper: createArrayCallbackWrapper,
            convertToHostArray: convertToHostArray,
            createIncomingMessage: function(ccInner) {
                return new IncomingMessage(ccInner);
            },
            createBuffer: function(ccInner) {
                return new Buffer(ccInner);
            }

        },
        crypto: {

        },
        zlib: {

        },
        timers: timers,
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
        https: {
            Agent: Agent,
            request: httpsRequest,
            get: httpsRequest
        }
    };

    return modules;

})();

require = builtinModules.require;
Buffer = require('buffer').Buffer;
process = require('process');
setTimeout = require('timers').setTimeout;
clearTimeout = require('timers').clearTimeout;
builtinModules = builtinModules;
