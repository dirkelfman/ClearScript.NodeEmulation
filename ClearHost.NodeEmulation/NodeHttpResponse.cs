using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;


namespace ClearScript.NodeEmulation
{
    public class NodeHttpResponse : NodeBuffer 
    {
        private readonly Task<HttpResponseMessage> resp;
        private bool _dataFired;
        public bool isCCnetHttpResponse = true;
        private NodeHttpRequest nodeHttpRequest;

        public NodeHttpResponse(NodeHttpRequest nodeHttpRequest, Task<HttpResponseMessage> resp, Require require)
            : base(require)
        {

            // TODO: Complete member initialization
            this.nodeHttpRequest = nodeHttpRequest;
            this.resp = resp;
            if (!resp.IsFaulted)
            {
                statusCode = (int) resp.Result.StatusCode;

                this.headers = new PropertyBag();
                resp.Result.Headers.ToList().ForEach(x => { headers[x.Key] = x.Value.FirstOrDefault(); });
                if (resp.Result.Content != null)
                {
                    resp.Result.Content.Headers.ToList().ForEach(x => { headers[x.Key] = x.Value.FirstOrDefault(); });
                }
                this.headers.Keys.ToList().ForEach(x=> this.headers[x.ToLowerInvariant()]=this.headers[x]);
            }
            JsWrapper = nodeHttpRequest.Require.BuiltIns.clearCaseHelpers.createIncomingMessage(this);
        }

        public dynamic JsWrapper { get; private set; }

        public override string clientWrapperClass
        {
            get { return "IncomingMessage"; }
        }


        public object bytes { get; set; }

        public object raw { get; set; }
        public object body { get; set; }
      //  public object bytes { get; set; }
        public string httpVersion { get; set; }
        public PropertyBag  headers { get; set; }
        public int statusCode { get; set; }

        public override long length
        {
            get
            {
                if (resp.IsCompleted && resp.Result.Content != null && resp.Result.Content.Headers.ContentLength.HasValue)
                {
                    return resp.Result.Content.Headers.ContentLength.Value;
                }
                if (InnerStream != null && InnerStream.CanSeek)
                {
                    return InnerStream.Length;
                }
                return 1024*4;
            }
        }

        public void InitEvents()
        {
            resp.Result.Content.ReadAsStreamAsync().ContinueWith(x =>
            {
                InnerStream = x.Result;
                OnData();
               // ProcessPipes();
            });
        }

        void ProcessPipes()
        {
            this.nodeHttpRequest.Require.Engine.NextTick(() =>
            {
                if (_pipes != null)
                {
                    foreach (var pipe in _pipes)
                    {
                        //nodeHttpRequest.Require.Engine.Execute("debugger;");
                        pipe.write(this.JsWrapper);
                        pipe.end();
                    }
                    _pipes.Clear();
                }
            });
        }


        //todo .. rework as chunked
        private void OnData()
        {
            emit("data", this.JsWrapper);

            if (hasEvent("json"))
            {
                var resp = GetHttpResponseMessage();
                if (resp.IsSuccessStatusCode && resp.Content != null)
                {
                    try
                    {
                      
                            using (var stream = resp.Content.ReadAsStreamAsync().Result)
                            {
                                var sr = new StreamReader(stream);

                                var ser = new JsonSerializer();
                                ser.Converters.Add(new ExpandoObjectConverter());
                                var json = ser.Deserialize<ExpandoObject>(new JsonTextReader(sr));
                                //    var json = ser.Deserialize(new JsonTextReader(sr));
                                emit("json", json);
                                return;
                            }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                       
                    }
                }
            }
            this.ProcessPipes();
            emit("end", null);
        }

        private List<dynamic> _pipes; 
        public byte[] read(int? size = null)
        {
            throw new NotImplementedException();
        }

        public dynamic pipe(dynamic destinationStream, dynamic options = null)
        {
            if (_pipes == null)
            {
                _pipes = new List<dynamic>();
            }
            _pipes.Add(destinationStream);
            return destinationStream;
        }

        public void unpipe(dynamic destinationStream = null)
        {
            throw new NotImplementedException();
        }

        public void unshift(dynamic chunk = null)
        {
            throw new NotImplementedException();
        }

        internal HttpResponseMessage GetHttpResponseMessage()
        {
            return resp.Result;
        }
    }
}