using System;
using System.Dynamic;
using System.IO;

namespace ClearHost.NodeEmulation
{
    public class NodeBuffer 
    {
        public static bool isEncoding(string isEncoding)
        {
            System.Diagnostics.Debug.WriteLine("isEncoding", isEncoding);
            return true;
           
        }

        
        


      
        public NodeBuffer(object param1)
        {
            if (param1 is string)
            {
                this.InnerStream = new MemoryStream();
                var buf = System.Text.Encoding.UTF8.GetBytes((string) param1);

                this.InnerStream.Write(buf, 0, buf.Length);
                return;
               
            }
            throw new NotImplementedException();

        }
        public NodeBuffer(object param1, object param2)
        {
            throw new NotImplementedException();
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

        public  virtual long length
        {
            get { return InnerStream.Length; }
        }




        public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            throw new NotImplementedException();
        }
    }
}