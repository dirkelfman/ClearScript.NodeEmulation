using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json.Schema;
using WebGrease.Css.Extensions;

namespace ClearHost.NodeEmulation
{
    public class NodeHttpRequest 
    {
        private HttpRequestMessage _requestMessage;
        //private NodeHttpRequestOptoins _options;
        private DynamicObject _options;
        private HttpClient _client;
        private Task<HttpResponseMessage> _responseTask;
        private Stream _requestStream;
        //private DynamicObject _callback;
        private Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();

        public NodeHttpRequest(DynamicObject options = null, dynamic callback = null) 
        {
            _client = new HttpClient();
            _requestMessage = new HttpRequestMessage();

            _options = options;


            if (callback != null && !(callback is Microsoft.ClearScript.Undefined))
            {
                this.@on("response", callback);
            }
        }

        public NodeHttpRequest(HttpClient client, HttpRequestMessage requestMessage, DynamicObject options = null, dynamic callback = null)
        {
            _client = client;
            _requestMessage = requestMessage;

            _options = options;


            if (callback != null && !(callback is Microsoft.ClearScript.Undefined))
            {
                this.@on("response" ,callback);
            }

        }

        public V8Runtime runtime { get; set; }
        public V8ScriptEngine engine { get; set; }

        public Require require { get; set; }
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

            
            var nodeBuffer = new NodeBuffer(text, encoding);
            write(nodeBuffer, null);
        }

        public void write(NodeBuffer data, string encoding = null)
        {
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

        public string Protocal = "http";
        public void end(NodeBuffer data = null, string encoding = null)
        {
            write(data, encoding);
            var uriBuilder = new UriBuilder(
                this.Protocal,
                _options.GetField<string>("hostname") ?? _options.GetField<string>("host"),
                _options.GetField<int>("port", new Func<object, int>(Convert.ToInt32), 80),
                _options.GetField<string>("path")).Uri;
            _requestMessage.RequestUri = uriBuilder;
            var headers = this.getHeaders();
           
            if ( headers!= null)
            {
                headers.GetDynamicMemberNames().ForEach(key =>
                {
                    var val = headers.GetField<object>(key);
                    if (val != null)
                    {
                        _requestMessage.Headers.TryAddWithoutValidation(key, new string[] {val.ToString()});   
                    }
                });
               

            }
            if (_requestStream != null)
            {
                _requestStream.Position = 0;
                
            }


            _requestMessage.Method =  new HttpMethod(_options.GetField("method", "GET"));
            //todo set up cancel optons
            _responseTask = _client.SendAsync(this._requestMessage)
                .ContinueWith<HttpResponseMessage>(OnResponse);

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
}