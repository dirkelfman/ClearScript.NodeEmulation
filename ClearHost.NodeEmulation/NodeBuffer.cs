using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ClearScript.NodeEmulation
{
    public class NodeBuffer : NodeEventEmitter
    {
        public bool isCCnetBuffer = true;
        private readonly Require _require;

        public override  string clientWrapperClass
        {
            get { return "Buffer"; }
        }

        public dynamic  CreateJsWrapper()
        {
            return _require.BuiltIns.clearCaseHelpers.createBuffer(this);
        }

        public NodeBuffer(Require require)
            : base(require)
        {
            //  InnerStream = new M;
            _require = require;
        }

        public Byte[] InnerBuffer { get; set; }


        private int? _length;

        public int Length
        {
            get
            {
                if (_length.HasValue)
                {
                    return _length.Value;
                }
                return InnerBuffer.Length;
            }
            set { _length = value; }
        }

        public void Init(int size)
        {
            this.InnerBuffer = new byte[size];

        }

        public void Init(object  o)
        {
            var otherBuffer = o as NodeBuffer;
            if (otherBuffer == null)
            {
                throw new NotImplementedException("xxx");
            }
            this.InnerBuffer = otherBuffer.InnerBuffer;
            this._length = otherBuffer._length;

        }

        public void Init(string text="", string encoding = null)
        {
            if (text == null)
            { 
                text = "";
            }
            this.InnerBuffer = GetEncoding(encoding).GetBytes((string)text);

        }

        NodeEncoding GetEncoding(string encoding)
        {
            return NodeEncoding.GetEncoding(encoding);
        }

       


      
        public void Write(string txt, int? offset, int? length=null, string encoding = null)
        {
            var o = offset.GetValueOrDefault(0);
            var l = length.GetValueOrDefault(this.Length - o);
            var e = this.GetEncoding(encoding);
            var source = e.GetBytes(txt);
            Array.Copy(source, 0, this.InnerBuffer, o, l);
        }

        public void writeInt32LE(int value, int? o = 0,  bool? noAssert = false)
        {
            var offset = o.GetValueOrDefault(0);
            InnerBuffer[0 + offset] = (byte)value;
            InnerBuffer[1 + offset] = (byte)(value >> 8);
            InnerBuffer[2 + offset] = (byte)(value >> 16);
            InnerBuffer[3 + offset] = (byte)(value >> 24);

        }
        public void writeInt32BE(int value, int? o = 0,  bool? noAssert = false)
        {
            var offset = o.GetValueOrDefault(0);

            InnerBuffer[0 + offset] = (byte)(value >> 24);
            InnerBuffer[1 + offset] = (byte)(value >> 16);
            InnerBuffer[2 + offset] = (byte)(value >> 8);
            InnerBuffer[3 + offset] = (byte)value;
            
            
            

           
        }

        public int readInt32LE(int? os = 0, bool? noAssert = false)
        {
            var offset = os.GetValueOrDefault(0);

            var num = (int)this.InnerBuffer[0 + offset] | (int)InnerBuffer[1 + offset] << 8 | (int)InnerBuffer[2 + offset] << 16 | (int)InnerBuffer[3 + offset] << 24;
            return num;
        
        }
        public int readInt32BE(int? os = 0, bool? noAssert = false)
        {
            var offset = os.GetValueOrDefault(0);

            var num = (int)this.InnerBuffer[3 + offset] | (int)InnerBuffer[2 + offset] << 8 | (int)InnerBuffer[1 + offset] << 16 | (int)InnerBuffer[0 + offset] << 24;
            return num;

        }

       
        
        public string _toString(string encoding = null, int? start = null, int? end = null)
        {
            var enc = this.GetEncoding(encoding);
            var s = start.GetValueOrDefault(0);
            var e = end.GetValueOrDefault(this.Length);
            
            

            return  enc.GetString(this.InnerBuffer, s, e);
            
        }
        [Microsoft.ClearScript.ScriptMember("toString")]
        public string Blurg(string encoding = null, int? start = null, int? end = null)
        {
            return _toString(encoding, start, end);

        }
     




        public NodeBuffer slice(int? start = null, int? end = null)
        {
            var s = start.GetValueOrDefault(0);
            var e = end.GetValueOrDefault(this.Length);
            if (s == 0)
            {
                return new NodeBuffer(require: this._require)
                       {
                           InnerBuffer = this.InnerBuffer,
                           Length = e
                       };

            }
            var buff = new byte[e - s];

            Array.Copy(InnerBuffer, s,  buff,0, e - s);

            return new NodeBuffer(require: this._require)
            {
                InnerBuffer = buff,
                Length = e - s
            };

        }

        public void fill(string value, int? offset, int? end)
        {
            var o = offset.GetValueOrDefault(0);
            var e = end.GetValueOrDefault(this.Length-o);

            if (!string.IsNullOrEmpty(value))
            {
                var buf = System.Text.Encoding.UTF8.GetBytes((string)value);
                Array.Copy(buf, 0, InnerBuffer, o, e);
                
            }
        }

        public void copy(NodeBuffer target, int? targetStart = null, int? sourceStart = null, int? sourceEnd = null)
        {
            var ts = targetStart.GetValueOrDefault(0);
            var ss = sourceStart.GetValueOrDefault(0);
            var se = sourceEnd.GetValueOrDefault( (int)target.Length-ts );

            if (ts + se > this.InnerBuffer.Length - ss)
            {
                se = this.InnerBuffer.Length - ss;
            }

            Array.Copy(this.InnerBuffer, ss, target.InnerBuffer, ts, se);
           

        }

        public void copy(DynamicObject target, int? targetStart = null, int? sourceStart = null, int? sourceEnd = null)
        {
            throw  new NotImplementedException("xxx");
        
             

        }

        public sbyte? get(int pos)
        {
            if (pos > InnerBuffer.Length)
            {
                return null;
            }
            return (sbyte)this.InnerBuffer[pos];
        }
        public void set(int pos, object value)
        {
            if (pos > InnerBuffer.Length)
            {
                return ;
            }
            this.InnerBuffer[pos] = Convert.ToByte(value);
        }

    }


    public  class NodeEncoding
    {
       
        private Func<byte[],int,int, string> ToStringFn;
        private Func<string, byte[]> ToByteFn; 
        static string ToHexString(byte[] val, int offset, int len)
        {
            return BitConverter.ToString(val, offset, len).Replace("-", String.Empty).ToLowerInvariant();
        }

        static string ToBase64String(byte[] val, int offset, int len)
        {
            return Convert.ToBase64String(val, offset, len);
        }

        static string ToUtf8String(byte[] val, int offset, int len)
        {
            return System.Text.Encoding.UTF8.GetString(val, offset, len);
        }
        static string ToAsciiString(byte[] val, int offset, int len)
        {
            return System.Text.Encoding.ASCII.GetString(val, offset, len);
        }
        static string ToUtf16LEString(byte[] val, int offset, int len)
        {
            return System.Text.Encoding.Unicode.GetString(val, offset, len);
        }

        public string GetString(byte[] val, int offset, int len)
        {
            return ToStringFn(val, offset, len);
        }

        public byte[] GetBytes(string text)
        {
            return ToByteFn(text);
        }

        public static NodeEncoding GetEncoding(string encoding)
        {
            NodeEncoding enc = new NodeEncoding();
            enc.ToByteFn = System.Text.Encoding.UTF8.GetBytes;
            enc.ToStringFn = ToUtf8String;
            if (string.IsNullOrEmpty(encoding))
            {
                return enc;
            }
            switch (encoding.ToLowerInvariant())
            {
                case "hex":
                {
                    enc.ToStringFn = ToHexString;
                    break;
                }
                
                case "ascii":
                {
                    enc.ToStringFn = ToAsciiString;
                    enc.ToByteFn = System.Text.Encoding.ASCII.GetBytes;
                    break;
                }
                case "binary":
                {
                    throw new NotImplementedException("binary encoding not implemented");
                }
                case "base64":
                {
                    enc.ToStringFn = ToAsciiString;
                    break;
                }
                case "raw":
                {
                    throw new NotImplementedException("raw encoding not implemented");
                }
                case "ucs2":
                case "ucs-2":
                case "utf16le":
                case "utf-16le":
                {
                    enc.ToStringFn = ToUtf16LEString;
                    enc.ToByteFn = System.Text.Encoding.Unicode.GetBytes;
                    break;

                }


            }
            return enc;
        }

        public static bool isEncoding(string encoding)
        {
            switch (encoding.ToLowerInvariant())
            {
                case "hex":
                case "utf8":
                case "utf-8":
                case "ascii":
               // case "binary":
                case "base64":
               // case "raw":
                case "ucs2":
                case "ucs-2":
                case "utf16le":
                case "utf-16le":
                    return true;
                default:
                    return false;
            }
        }
    }

    //public class NodeBuffer23 : NodeEventEmitter
    //{
    //    private readonly Require _require;

    //    public bool isCCnetBuffer = true;

    //    public virtual string clientWrapperClass
    //    {
    //        get { return "Buffer"; }
    //    }

    //    public static bool isEncoding(string isEncoding)
    //    {
    //        System.Diagnostics.Debug.WriteLine("isEncoding", isEncoding);
    //        return true;

    //    }


    //    public NodeBuffer23 Init(string param1)
    //    {
    //        return this.Init(param1, "utf-8");

    //    }

    //    public NodeBuffer23 Init(int size)
    //    {
    //        this.InnerStream = new MemoryStream(size);
    //        return this;
    //    }

    //    public void writeInt32BE(int value, int offset = 0, bool noAssert = false)
    //    {
    //        byte[] buff = new byte[4];
    //        buff[0] = (byte)value;
    //        buff[1] = (byte)(value >> 8);
    //        buff[2] = (byte)(value >> 16);
    //        buff[3] = (byte)(value >> 24);
            

    //        if (this.InnerStream.CanSeek)
    //        {
    //            var pos = this.InnerStream.Position;
    //            this.InnerStream.Position = offset;
    //            this.InnerStream.Write(buff, 0, buff.Length);
    //            this.InnerStream.Position = pos;
    //        }
    //        else
    //        {
    //            this.InnerStream.Write(buff, 0, buff.Length);
    //        }

    //    }

       

    //    public int readInt32BE(int offset = 0, bool noAssert = false)
    //    {
    //        byte[] buff = new byte[4];
           
           
           
    //        if (this.InnerStream.CanSeek)
    //        {
    //            var pos = this.InnerStream.Position;
    //            this.InnerStream.Position = offset;
    //            this.InnerStream.Read(buff, 0, buff.Length);
    //            this.InnerStream.Position = pos;
    //        }
    //        else
    //        {
    //            this.InnerStream.Read(buff, 0, buff.Length);
    //        }

           
            
    //        var num = (int)buff[0] | (int)buff[1] << 8 | (int)buff[2] << 16 | (int)buff[3] << 24;
    //        return num;
    //    }




    //    public NodeBuffer Init(object text = null, object bla = null)
    //    {
    //        return Init2(text as string, bla as string);
    //    }

    //    public NodeBuffer Init2(string text, string encodingTxt)
    //    {

    //        this.InnerStream = new MemoryStream();

    //        Encoding enc = System.Text.Encoding.UTF8;
    //        if (!string.IsNullOrEmpty(encodingTxt))
    //        {

    //            enc = Encoding.GetEncodings().Where(x => string.Equals(x.Name, encodingTxt, StringComparison.OrdinalIgnoreCase)).Select(x => x.GetEncoding()).FirstOrDefault()
    //                  ?? enc;
    //        }
    //        //var enc = string.IsNullOrWhiteSpace(encodingTxt) ? System.Text.Encoding.UTF8 : Encoding.GetEncoding(encodingTxt);
    //        //todo: look up encoding.  
    //        if (text != null)
    //        {
    //            var buf = enc.GetBytes((string)text);
    //            this.InnerStream.Write(buf, 0, buf.Length);
    //        }
    //        return this;




    //    }


    //    public NodeBuffer23(Require require)
    //        : base(require)
    //    {
    //        //  InnerStream = new M;
    //        _require = require;
    //    }



    //    public static bool isBuffer(object obj)
    //    {
    //        return obj is NodeBuffer;
    //    }


    //    public NodeBuffer slice(int? start = null, int? end = null)
    //    {
    //        if (start == null && end == null)
    //        {
    //            return this;
    //        }
    //        throw new NotImplementedException();

    //    }

    //    public void fill(string value, int? offset, int? end)
    //    {
    //        if (value != null)
    //        {
    //            var buf = System.Text.Encoding.UTF8.GetBytes((string)value);
    //            this.InnerStream.Write(buf, 0, buf.Length);
    //        }
    //    }

    //    public Stream InnerStream { get; set; }



    //    public string StupidToString(object enc = null, int? start = null, int? end = null)
    //    {
    //        return this.toString(enc, start, end);
    //    }

    //    public string toString(object enc = null, int? start = null, int? end = null)
    //    {
    //        string encoding = null;
    //        long pos = 0;
    //        string ret = null;
    //        if (this.InnerStream.CanSeek)
    //        {
    //            pos = this.InnerStream.Position;
    //            this.InnerStream.Position = 0;

    //        }
    //        if (string.IsNullOrWhiteSpace(encoding))
    //        {
    //            ret = new StreamReader(this.InnerStream, true).ReadToEnd();
    //        }
    //        else
    //        {
    //            ret = new StreamReader(this.InnerStream, System.Text.Encoding.GetEncoding(encoding)).ReadToEnd();
    //        }
    //        if (this.InnerStream.CanSeek)
    //        {
    //            this.InnerStream.Position = pos;
    //        }
    //        return ret;

    //    }

    //    public void copy(NodeBuffer target, int? targetStart = null, int? sourceStart = null, int? sourceEnd = null)
    //    {
    //        this.InnerStream.CopyTo(target.InnerStream);

    //    }

    //    private long? _length;

    //    public virtual long length
    //    {

    //        get
    //        {
    //            if (_length.HasValue)
    //            {
    //                return _length.Value;
    //            }
    //            return this.InnerStream.Length;

    //        }
    //        set { _length = value; }
    //    }




    //}
}