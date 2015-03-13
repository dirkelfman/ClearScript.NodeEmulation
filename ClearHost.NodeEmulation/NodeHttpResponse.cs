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
    public class NodeHttpResponse : NodeEventEmitter 
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
            //todo handle connect errors;
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



//        Event: 'readable'
//Event: 'data'
//Event: 'end'
//Event: 'close'
//Event: 'error'
//readable.read([size])
//readable.setEncoding(encoding)
//readable.resume()
//readable.pause()
//readable.isPaused()
//readable.pipe(destination[, options])
//readable.unpipe([destination])
//readable.unshift(chunk)
//readable.wrap(stream)

//        Event: 'close'
//message.httpVersion
//message.headers
//message.rawHeaders
//message.trailers
//message.rawTrailers
//message.setTimeout(msecs, callback)
//message.method
//message.url
//message.statusCode
//message.statusMessage
//message.socket

        public dynamic read(long? size = null)
        {
            var nb = readNodeBuffer(size);
            var jb = nb.CreateJsWrapper();
            emit("json", jb);
            return jb;
        }

        private NodeBuffer readNodeBuffer(long? size= null)
        {
            var len = 0L;
            var contentLength = this.GetHttpResponseMessage().Content.Headers.ContentLength;
            if (contentLength.HasValue)
            {
                len = contentLength.Value - this.InnerStream.Position;
            }
            else
            {
                len = size.GetValueOrDefault(1024*10);
            }
            len = Math.Min(len, size.GetValueOrDefault(len));


            var buff = new byte[len];
            this.InnerStream.Read(buff, 0, buff.Length);
            //should this be on next tick?

            var nb = new NodeBuffer(this.nodeHttpRequest.Require);
            nb.InnerBuffer = buff;
            return nb;
        }

        //  public object bytes { get; set; }
        public string httpVersion { get; set; }
        public PropertyBag  headers { get; set; }

        public PropertyBag rawHeaders
        {
            get { return this.headers; }
        }

        public int statusCode { get; set; }

        public string method
        {
            get { return this.nodeHttpRequest.RequestMessage.Method.Method; }
            
        }
        public string url
        {
            get { return this.nodeHttpRequest.RequestMessage.RequestUri.ToString(); }

        }
        public string statusMessage
        {
            get { return this.GetHttpResponseMessage().ReasonPhrase; }

        }
        //public  long length
        //{
        //    get
        //    {
        //        if (resp.IsCompleted && resp.Result.Content != null && resp.Result.Content.Headers.ContentLength.HasValue)
        //        {
        //            return resp.Result.Content.Headers.ContentLength.Value;
        //        }
        //        if (InnerStream != null && InnerStream.CanSeek)
        //        {
        //            return InnerStream.Length;
        //        }
        //        return 1024*4;
        //    }
        //}
        public Stream InnerStream { get; set; }
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
                    while (true)
                    {

                        var nb = this.readNodeBuffer();
                        if (nb.InnerBuffer.Length == 0)
                        {
                            break;
                        }
                        var jb = nb.CreateJsWrapper();
                        foreach (var pipe in _pipes)
                        {

                            //nodeHttpRequest.Require.Engine.Execute("debugger;");
                            pipe.write(jb);
                           
                        }
                    }
                    foreach (var pipe in _pipes)
                    {

                       pipe.end();
                    }
                    _pipes.Clear();
                }
            });
        }


        //todo .. rework as chunked
        private void OnData()
        {

            if (this.hasEvent("data"))
            {
                while (true)
                {

                    var nb = this.readNodeBuffer();
                    if (nb.InnerBuffer.Length == 0)
                    {
                        break;
                    }
                    var jb = nb.CreateJsWrapper();
                    emit("data", jb);    
                   
                }
               
            }
            

            if (hasEvent("json"))
            {
                var resp = GetHttpResponseMessage();
                if (resp.IsSuccessStatusCode && resp.Content != null)
                {
                    try
                    {
                      
                           // using (var stream = resp.Content.ReadAsStreamAsync().Result)
                            {
                                var sr = new StreamReader(InnerStream);

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