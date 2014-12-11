using System.Configuration;
using System.IO;
using System.Linq;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.ClearScript;
using WebGrease.Css.Extensions;

namespace ClearHost.NodeEmulation
{
    public static class DynamicObjectExtensions
    {
        public static bool HasField(this DynamicObject obj, string field)
        {
            return !(obj.AsDynamic().field is Microsoft.ClearScript.Undefined);
        }

        public static bool TryGetField(this DynamicObject obj, string field, out object outField)
        {
           
            outField = obj.AsDynamic().field;
            return !(outField is Microsoft.ClearScript.Undefined);

        }

        public static dynamic AsDynamic(this DynamicObject obj)
        {
            return (dynamic)obj ;
        }
        public static IEnumerable<KeyValuePair<string, object>> GetProperties(this DynamicObject obj  )
        {


            var d = obj.AsDynamic();
            if (obj == null)
            {
                yield break;
            }
            foreach (var varName in obj.GetDynamicMemberNames())
            {
                object prop;
                if (obj.TryGetField(varName, out prop))
                {
                    yield return new KeyValuePair<string, object>(varName,prop);
                }

            }
            
        }

        public class MyGetMemberBinder : GetMemberBinder
        {
            public MyGetMemberBinder(string name)
                : base(name, true)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                return null;
            }
        }

        public static T GetField<T>(this DynamicObject obj , string field, T defaultValue = default(T))
        {
            Object outField;

            if (obj.TryGetMember(new MyGetMemberBinder(field), out outField))
            {
                if (outField is Microsoft.ClearScript.Undefined)
                {
                    return defaultValue;
                }
                return (T) outField;
            }
           
            return defaultValue;
        }
        public static T GetField<T>(this DynamicObject obj, string field, Func<object,T> converter, T defaultValue = default(T))
        {
            //object  outField = obj.field;

            //if (outField is Microsoft.ClearScript.Undefined)
            //{
            //    return defaultValue;
            //}

            //return converter(outField);


            Object outField;

            if (obj.TryGetMember(new MyGetMemberBinder(field), out outField))
            {
                if (outField is Microsoft.ClearScript.Undefined)
                {
                    return defaultValue;
                }
                return converter(outField);
            }
            
            return defaultValue;
        }

        //public static Func<object, PropertyBag> PropertyBagConverter = new Func<object, PropertyBag>((x)=>
        //                                                               {
        //                                                                   if (x == null)
        //                                                                   {
        //                                                                       return new PropertyBag();
        //                                                                   }
        //                                                                   var dx = (dynamic) x;
                                                                          
        //                                                               });
        
            
        
    }
    public class NodeHttp
    {
        public NodeHttpRequest request(dynamic optoins, dynamic callback)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            object p1 = optoins;
            dynamic p2 = callback;
            var req = new NodeHttpRequest(client, requestMessage, p1, p2);


           
            return req;
        }
    }


    public class NodeRequestModule
    {
        public static void MakeRequest(string url, dynamic callback)
        {
            
        }

 


        //function(error, message, response) {

        internal static void MakeRequest(DynamicObject config, DynamicObject callback, Microsoft.ClearScript.V8.V8ScriptEngine engine)
        {
            var options = new NodeHttpRequestOptoins(config);
            var uriObj = new Uri((config.GetField<object>("uri") ?? config.GetField<object>("url")).ToString());
            options.url = (config.GetField<object>("uri") ?? config.GetField<object>("url"));
            options.host = uriObj.Host;
            options.hostname = uriObj.Host;
            options.scheme = uriObj.Scheme;
            options.path = uriObj.PathAndQuery;
            options.port = uriObj.Port;
            options.method = config.GetField<string>( "method", "GET");
            options.headers = config.GetField<DynamicObject>( "headers");
            bool isJson = config.GetField("json", false);


            var req = new NodeHttpRequest(new HttpClient(), new HttpRequestMessage(), options);
            Action<NodeHttpResponse> wrapperCallback = (NodeHttpResponse resp) =>
            {
                if (callback == null)
                {
                    return;
                }
                //    string body = null;
                object body = null;
                var apiResp = resp.GetHttpResponseMessage();
                if (apiResp.Content != null && apiResp.Content.Headers.ContentLength > 0)
                {
                    if (isJson)
                    {
                        string xxx = apiResp.Content.ReadAsStringAsync().Result;
                        var parser = (dynamic)engine.Evaluate("JSON.parse");
                        body =parser(xxx);


                    }
                    else
                    {
                        body = apiResp.Content.ReadAsStringAsync().Result;
                    }
                }



                callback.AsDynamic().call(null, null, resp, body);
            };
            req.@on("response", wrapperCallback);

            req.end();
        }
    }

    public class NodeHttpRequestOptoins
    {
        public NodeHttpRequestOptoins(DynamicObject config)
        {
            host = config.GetField<string>( "host");
            scheme = config.GetField<string>( "scheme", "http");
            hostname = config.GetField<string>( "hostname") ?? host;

            url = (dynamic) config.GetField<object>("uri") ?? config.GetField<object>("url"); 
            method = config.GetField<string>( "method");
            headers = config.GetField<DynamicObject>( "headers");

            port = config.GetField<int>( "port", new Func<object, int>(Convert.ToInt32), 80);
        }

        public string host
        {
            get { return hostname; }
            set { hostname = value; }
        }

        public string hostname { get; set; }
        public int? port { get; set; }
        public string method { get; set; }
        public string path { get; set; }
        public DynamicObject headers { get; set; }



        public DynamicObject auth { get; set; }
        public DynamicObject agent { get; set; }


        public string scheme { get; set; }

        public dynamic uri { 
            get { return url; }
            set { url = value; } 
        }
        public dynamic url {
            get
            {
                if (hostname != null)
                {
                    try
                    {
                        return new UriBuilder(scheme, hostname, port.HasValue ? port.Value : 80, path).Uri;
                    }
                    catch
                    {
                    }
                }
                return null;

            }
            set
            {
                if (value!= null)
                {
                    var uriString = value as string;
                    if (uriString != null)
                    {
                        value = new Uri(uriString);
                   
                    }
                    var uri= value as Uri;
                    if (uri != null)
                    {
                        this.hostname = uri.Host;
                        this.port = uri.Port;
                        this.scheme = uri.Scheme;
                        this.path = uri.PathAndQuery;
                        return;
                    }

                    this.hostname = value.hostname;
                    this.port = value.port;
                    this.scheme = value.protocol;
                    this.path = value.pathname + value.search;

                }
            }
        }
    }

    
    public class NodeHttpRequest 
    {
        private HttpRequestMessage _requestMessage;
        private NodeHttpRequestOptoins _options;
        private HttpClient _client;
        private Task<HttpResponseMessage> _responseTask;
        private Stream _requestStream;
        //private DynamicObject _callback;
        private Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();
        public NodeHttpRequest(HttpClient client, HttpRequestMessage requestMessage, object options = null, dynamic callback = null)
        {
            _client = client;
            _requestMessage = requestMessage;
           
            _options = options as NodeHttpRequestOptoins ??new NodeHttpRequestOptoins((dynamic)options);


            if (callback != null && !(callback is Microsoft.ClearScript.Undefined))
            {
                this.@on("response" ,callback);
            }

        }

        public void init(DynamicObject optoins)
        {
            
        }
        public void on(string eventName, dynamic callbackFn)
        {
            //response
            List<dynamic> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] =  new List<dynamic>();
            }
           
            events.Add(callbackFn);
        }

        public void setHeader(string name, object value)
        {
            var headers = _options.headers = _options.headers ?? (dynamic) new PropertyBag();
            headers.AsDynamic()[name] = value;
        }

        public string getHeader(string name)
        {
            if (_options.headers == null)
            {
                return null;
            }
            return _options.headers.AsDynamic()[name] as string;
        }

        

        public void removeHeader(string name)
        {
            var headers = _options.headers = _options.headers ?? (dynamic)new PropertyBag();
            headers.AsDynamic().Remove(name);
        }




        public void write(string text, string encoding = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var enc = string.IsNullOrWhiteSpace(encoding) ? System.Text.Encoding.UTF8 : System.Text.Encoding.GetEncoding(encoding );
            var data =enc.GetBytes(text);
            write(data, null);
        }

        public void write(byte[] data, string encoding = null)
        {
            if (data != null)
            {


                if (_requestStream == null)
                {
                    _requestStream = new MemoryStream();
                    this._requestMessage.Content = new StreamContent(_requestStream);
                }
                _requestStream.Write(data, 0, data.Length);
            }
        }

        public void end(byte[] data = null, string encoding = null)
        {
            write(data, encoding);
            var uriBuilder = new UriBuilder(_options.scheme, _options.hostname, _options.port.Value, _options.path).Uri;
            _requestMessage.RequestUri = uriBuilder;
            if (_options.headers != null)
            {

                _options.headers.GetProperties().ForEach(kvp=>
                {
                    _requestMessage.Headers.TryAddWithoutValidation(kvp.Key, new string[] { (kvp.Value ?? new object()).ToString() });
                });

            }
            //todo set up cancel optons
            _responseTask = _client.SendAsync(this._requestMessage)
                .ContinueWith<HttpResponseMessage>(OnResponse, TaskContinuationOptions.NotOnFaulted);

        }

        public void abort()
        {
            //do cancelation
        }

        HttpResponseMessage  OnResponse(Task<HttpResponseMessage> responseTask)
        {
            var resp = responseTask.Result;
            List<dynamic> listeners;
            if ( _listeners.TryGetValue("response", out listeners))
            {
                NodeHttpResponse nodeResponse = new NodeHttpResponse(this, responseTask);
                listeners.ForEach(listener =>
                {
                    if (listener is Action<NodeHttpResponse>)
                    {
                        ((Action<NodeHttpResponse>) listener)(nodeResponse);
                    }
                    else
                    {
                        try
                        {
                            listener.call(null, nodeResponse);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                    }
                    


                });
                nodeResponse.InitEvents();
            }
            return resp;
        }
    }

    public class NodeHttpResponse : NodeBuffer
    {
        private NodeHttpRequest nodeHttpRequest;
        private Task<HttpResponseMessage> resp;
        private bool _dataFired;

        private Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();
     
        public NodeHttpResponse(NodeHttpRequest nodeHttpRequest, Task<HttpResponseMessage> resp):base(null)
        {
            // TODO: Complete member initialization
            this.nodeHttpRequest = nodeHttpRequest;
            this.resp = resp;
            if (!resp.IsFaulted)
            {
                this.statusCode = (int) resp.Result.StatusCode;

                this.headers = new PropertyBag();
                resp.Result.Headers.ForEach(x =>
                {
                    this.headers[x.Key] = x.Value.FirstOrDefault();
                });
                if (resp.Result.Content != null)
                {
                    resp.Result.Content.Headers.ForEach(x =>
                    {
                        this.headers[x.Key] = x.Value.FirstOrDefault();
                    });
                }

                
            }
            
            
        }

        public bool isBuffer
        {
            get { 
                return true; 
            }
        }
        public void InitEvents()
        {
            this.resp.Result.Content.ReadAsStreamAsync().ContinueWith(x =>
            {
                this.InnerStream = x.Result;
                this.OnData();
            });
        }
        public object body { get; set; }
        internal HttpResponseMessage GetHttpResponseMessage()
        {
            return resp.Result;
        }
        
        public void on(string eventName, dynamic callbackFn)
        {
       
            List<dynamic> events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] =  new List<dynamic>();
            }
           
            events.Add(callbackFn);
        
            //close
            //readable
            //data
            //end
            //error


       
           
        }


        //todo .. rework as chunked
        void OnData( )
        {
           
            
            List<dynamic> listeners;
            if (_listeners.TryGetValue("data", out listeners))
            {
               
                listeners.ForEach(listener =>
                {
                    listener.call(null, this);
                });
            }
            if (_listeners.TryGetValue("end", out listeners))
            {

                listeners.ForEach(listener =>
                {
                    try
                    {
                        listener.call(null);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                });
            }
           
        }
        public string httpVersion { get; set; }
        public dynamic headers { get; set; }

        public int statusCode { get; set; }

        public byte[] read(int? size = null)
        {
            throw  new NotImplementedException();
        }

        public void pipe(dynamic destinationStream, dynamic options = null)
        {
            throw new NotImplementedException();
        }

        public void unpipe(dynamic destinationStream= null)
        {
            throw new NotImplementedException();
        }

        public void unshift(dynamic chunk = null)
        {
            throw new NotImplementedException();
        }
    }


    public class NodeBuffer 
    {
        public static bool isEncoding(string isEncoding)
        {
            System.Diagnostics.Debug.WriteLine("isEncoding", isEncoding);
            return true;
           
        }

        
        

        public dynamic request { get; set; }
        public dynamic toJSON { get; set; }

        public dynamic caseless { get; set; }
        //public override bool TryGetMember(GetMemberBinder binder, out object result)
        //{
        //    var res = base.TryGetMember(binder, out result);
        //    return res;
        //}

        //public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        //{
        //    var res = base.TryInvoke(binder, args, out result);

        //    return res;
        //}

        //public override IEnumerable<string> GetDynamicMemberNames()
        //{
        //    var g = base.GetDynamicMemberNames().ToList();
        //    return g;
        //}
        public NodeBuffer(int size) : this(new MemoryStream(size))
        {
            
        }
        public NodeBuffer() : this(new MemoryStream())
        {
            
        }
        public NodeBuffer(Stream innerStream)
        {
            InnerStream = innerStream;
        }

        public static bool isBuffer (object obj)
        {
            return obj is NodeBuffer;
        }


        public NodeBuffer slice(int? start = null, int? end = null)
        {
            if (start == null && end == null)
            {
                return this;
            }
            throw new NotImplementedException();

        }
        public Stream InnerStream { get; set; }
        public  string toString(object enc = null ,int? start = null , int? end= null)
        {
            string encoding = null;
            long pos = this.InnerStream.Position;
            this.InnerStream.Position = 0;
            string ret = null;
            if (string.IsNullOrWhiteSpace(encoding))
            {
                ret= new StreamReader(this.InnerStream, true ).ReadToEnd();
            }
            else
            {
                ret= new StreamReader(this.InnerStream, System.Text.Encoding.GetEncoding(encoding)).ReadToEnd();
            }
            
            this.InnerStream.Position = pos;
            return ret;

        }

        public void copy(NodeBuffer target, int? targetStart= null, int? sourceStart = null, int? sourceEnd = null)
        {
            this.InnerStream.CopyTo(target.InnerStream);
           
        }

        public  long length
        {
            get { return InnerStream.Length; }
        }




        public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            throw new NotImplementedException();
        }
    }
}
       