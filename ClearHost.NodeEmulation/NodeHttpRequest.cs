using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json.Schema;
using System.Linq;


namespace ClearScript.NodeEmulation
{
    public class NodeHttpsRequest : NodeHttpRequest
    {
        public NodeHttpsRequest(Require require) : base(require)
        {
            
        }
    }

    public class NodeHttpRequest :NodeEventEmitter
    {
        private HttpRequestMessage _requestMessage;
        //private NodeHttpRequestOptoins _options;
        private DynamicObject _options;
        
        private Task<HttpResponseMessage> _responseTask;
        private Stream _requestStream;
        //private DynamicObject _callback;
      
        public bool isCCnetHttpRequest = true;
      
        public void Init (DynamicObject options = null, dynamic callback = null) 
        {
           
            _requestMessage = new HttpRequestMessage();

            _options = options;

            
            if (callback != null && !(callback is Microsoft.ClearScript.Undefined))
            {
                this.@on("response", callback);
            }
        }



        public Require Require { get; set; }
     
     

        public DynamicObject getHeaders()
        {
            return  _options.GetField<DynamicObject>("headers", null);
        }

        public void setHeaders(DynamicObject headers)
        {
            _options.TrySetField("headers", headers);
        }
        public void setHeader(string name, object value)
        {
            var headers = getHeaders();
            if (headers == null)
            {
                headers =(DynamicObject) (object)new PropertyBag();
                this.setHeaders(headers);
            }
            headers.TrySetField(name, value);
            
        }

        public string getHeader(string name)
        {
            var headers = getHeaders();
            if (headers == null)
            {
                return null;
            }
            return headers.GetField<string>(name, null);
        }

        

        public void removeHeader(string name)
        {
            var headers = getHeaders();
            if (headers != null)
            {
                headers.TrySetField(name, null);
            }
        }




        public void write(string text, string encoding = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }


            var nodeBuffer = new NodeBuffer(this.Require).Init(text, encoding);
            write(nodeBuffer, null);
        }

        public void write(NodeBuffer data, string encoding = null)
        {
            var method = Method;
            if (!string.Equals(method,"post", StringComparison.OrdinalIgnoreCase)&&
                !string.Equals(method,"put", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (data != null)
            {
                
                if (_requestStream == null)
                {
                    _requestStream = new MemoryStream();
                    this._requestMessage.Content = new StreamContent(_requestStream);
                }
                var pos = data.InnerStream.Position;
                data.InnerStream.Position = 0;
                data.InnerStream.CopyTo(_requestStream);
                data.InnerStream.Position = pos;
                //_requestStream.Write(data, 0, data.Length);
            }
        }

        public string protocal = "http";
    

        public NodeHttpRequest(Require require):base(require)
        {
            if (require == null) throw new ArgumentNullException("require");
            this.Require = require;
        }

        string Method
        {
            get { return _options.GetField("method", "GET"); }
        }
        public void end(NodeBuffer data = null, string encoding = null)
        {
            write(data, encoding);
            var path = (_options.GetField<string>("path") ?? "");


            int qpos = path.IndexOf('?');



            var uri = new UriBuilder(
                this.protocal,
                _options.GetField<string>("hostname") ?? _options.GetField<string>("host"),
                _options.GetFieldWithConverter<int>("port", new Func<object, int>(Convert.ToInt32), 80),
                qpos > -1 ? path.Substring(0, qpos) : path,
                qpos > -1 ? path.Substring(qpos) : null).Uri;


            _requestMessage.RequestUri = uri;
            var headers = this.getHeaders();
           
            if ( headers!= null)
            {
                headers.GetDynamicMemberNames().ToList().ForEach(key =>
                {
                    var val = headers.GetField<object>(key);
                    if (val != null)
                    {
                        var headerVal = new string[] {val.ToString()};
                        if (!_requestMessage.Headers.TryAddWithoutValidation(key, headerVal) && 
                            _requestMessage.Content != null && 
                            !string.Equals(key,"Content-Length", StringComparison.OrdinalIgnoreCase))
                        {
                            _requestMessage.Content.Headers.TryAddWithoutValidation(key, headerVal);
                        }
                    }
                   
                });
               

            }
            if (_requestStream != null)
            {
                _requestStream.Position = 0;
                
            }

       


            _requestMessage.Method =  new HttpMethod(_options.GetField("method", "GET"));


            var client = Require.GetService(typeof(HttpClient).ToString()) as HttpClient;
            

            //todo set up cancel optons
            _responseTask = client.SendAsync(this._requestMessage, HttpCompletionOption.ResponseHeadersRead)
                .ContinueWith<HttpResponseMessage>(OnResponse);

        }

        public void abort()
        {
            throw new NotImplementedException();
            //do cancelation
        }

        HttpResponseMessage  OnResponse(Task<HttpResponseMessage> responseTask)
        {
            var resp = responseTask.Result;
            List<dynamic> listeners;
            if (this.hasEvent("response"))
            {
                NodeHttpResponse nodeResponse = new NodeHttpResponse(this, responseTask, this.Require);
                this.emit("response", nodeResponse);
                nodeResponse.InitEvents();
            }
            
            return resp;
        }
    }
}