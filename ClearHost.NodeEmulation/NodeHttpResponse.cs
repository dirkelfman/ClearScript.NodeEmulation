using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using WebGrease.Css.Extensions;

namespace ClearHost.NodeEmulation
{
    public class NodeHttpResponse : NodeBuffer
    {
        private NodeHttpRequest nodeHttpRequest;
        private Task<HttpResponseMessage> resp;
        private bool _dataFired;

        private Dictionary<string, List<dynamic>> _listeners = new Dictionary<string, List<dynamic>>();

        public NodeHttpResponse(NodeHttpRequest nodeHttpRequest, Task<HttpResponseMessage> resp)
            : base((System.IO.MemoryStream)null)
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
                    this.headers[x.Key] = Enumerable.FirstOrDefault<string>(x.Value);
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
                var helper = (dynamic)this.nodeHttpRequest.require.BuiltIns.httpHelper;
                var buffer = helper.createIncomingMessage.call(null, this);

               
                listeners.ForEach(listener =>
                {
                    listener.call(null, buffer);
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

        internal HttpResponseMessage GetHttpResponseMessage()
        {
            return resp.Result;
        }

        public override  long length
        {
            get
            {
                if (this.resp.IsCompleted&& this.resp.Result.Content!= null)
                {
                    return this.resp.Result.Content.Headers.ContentLength.GetValueOrDefault();
                }
                if (this.InnerStream != null)
                {
                    return InnerStream.Length;
                }
                return 0;
            }
        }
        
    }
}