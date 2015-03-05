using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClearScript.NodeEmulation
{
    public class NodeBuffer : NodeEventEmitter
    {
        private readonly Require _require;

        public bool isCCnetBuffer = true;

        public virtual string clientWrapperClass
        {
            get { return "Buffer"; }
        }

        public static bool isEncoding(string isEncoding)
        {
            System.Diagnostics.Debug.WriteLine("isEncoding", isEncoding);
            return true;
           
        }


        public NodeBuffer Init(string param1)
        {
            return this.Init( param1 , "utf-8" );
            
        }

        public NodeBuffer Init(object text= null, object bla= null)
        {
            return Init2(text as string, bla as string);
        }

        public NodeBuffer Init2(string text, string encodingTxt)
        {

            this.InnerStream = new MemoryStream();

            Encoding enc = System.Text.Encoding.UTF8;
            if (!string.IsNullOrEmpty(encodingTxt))
            {

                enc = Encoding.GetEncodings().Where( x => string.Equals(x.Name, encodingTxt, StringComparison.OrdinalIgnoreCase)).Select(x=> x.GetEncoding()).FirstOrDefault()
                      ?? enc;
            }
            //var enc = string.IsNullOrWhiteSpace(encodingTxt) ? System.Text.Encoding.UTF8 : Encoding.GetEncoding(encodingTxt);
            //todo: look up encoding.  
            var buf = enc.GetBytes((string)text);

            this.InnerStream.Write(buf, 0, buf.Length);
            return this; 




        }


        public NodeBuffer(Require require) :base (require)
        {
          //  InnerStream = new M;
            _require = require;
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

      

        public string StupidToString(object enc = null, int? start = null, int? end = null)
        {
            return this.toString(enc, start, end);
        }

        public  string toString(object enc = null ,int? start = null , int? end= null)
        {
            string encoding = null;
            long pos = 0;
            string ret = null;
            if (this.InnerStream.CanSeek)
            {
                pos = this.InnerStream.Position;
                this.InnerStream.Position = 0;
               
            }
            if (string.IsNullOrWhiteSpace(encoding))
            {
                ret= new StreamReader(this.InnerStream, true ).ReadToEnd();
            }
            else
            {
                ret= new StreamReader(this.InnerStream, System.Text.Encoding.GetEncoding(encoding)).ReadToEnd();
            }
            if (this.InnerStream.CanSeek)
            {
                this.InnerStream.Position = pos;
            }
            return ret;

        }

        public void copy(NodeBuffer target, int? targetStart= null, int? sourceStart = null, int? sourceEnd = null)
        {
            this.InnerStream.CopyTo(target.InnerStream);
           
        }

        private long? _length;
        
        public  virtual long length {

            get
            {
                if (_length.HasValue)
                {
                    return _length.Value;
                }
                return 1024*4;

            }
            set { _length = value; }
        }




    }
}