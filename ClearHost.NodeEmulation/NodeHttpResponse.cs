using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using WebGrease.Css.Extensions;

namespace ClearScript.NodeEmulation
{
    public class NodeHttpResponse : NodeBuffer
    {
        private NodeHttpRequest nodeHttpRequest;
        private Task<HttpResponseMessage> resp;
        private bool _dataFired;


        public override string clientWrapperClass {
            get { return "IncommingMessage"; }
        }
  
        
        public bool isCCnetHttpResponse = true;
       
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


      
        public void InitEvents()
        {
            this.resp.Result.Content.ReadAsStreamAsync().ContinueWith(x =>
            {
                this.InnerStream = x.Result;
                this.OnData();
            });
        }
        public object body { get; set; }
        
        
       


        //todo .. rework as chunked
        void OnData( )
        {
            this.emit("data", this);
            this.emit("end", null);
            
            
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
                if (this.resp.IsCompleted && this.resp.Result.Content != null && this.resp.Result.Content.Headers.ContentLength.HasValue)
                {
                    return this.resp.Result.Content.Headers.ContentLength.Value;
                }
                if (this.InnerStream != null && this.InnerStream.CanSeek)
                {
                    return InnerStream.Length;
                }
                return 1024*4;
            }
        }
        
    }
}