using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClearScript.NodeEmulation
{
    public class NodeBuffer2 : NodeEventEmitter
    {
        public bool isCCnetBuffer = true;
        private readonly Require _require;

        public virtual string clientWrapperClass
        {
            get { return "Buffer"; }
        }

        public NodeBuffer2(Require require)
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
            var otherBuffer = o as NodeBuffer2;
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

        Encoding GetEncoding(string encoding)
        {
            Encoding enc = System.Text.Encoding.UTF8;
            if (!string.IsNullOrEmpty(encoding))
            {
                
                enc = Encoding.GetEncodings().Where(x => string.Equals(x.Name, encoding, StringComparison.OrdinalIgnoreCase)).Select(x => x.GetEncoding()).FirstOrDefault()
                      ?? enc;
            }

           
            
            return enc;
        }

        class NumberStringEncoding : Encoding 
        {
            public string EncodingType { get; set; }
            public override int GetByteCount(char[] chars, int index, int count)
            {
                throw new NotImplementedException();
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                throw new NotImplementedException();
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                throw new NotImplementedException();
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                var str = System.Text.Encoding.UTF8.GetChars(bytes, byteIndex, byteCount, chars, charIndex);

                throw new NotImplementedException();
            }

            public override int GetMaxByteCount(int charCount)
            {
                throw new NotImplementedException();
            }

            public override int GetMaxCharCount(int byteCount)
            {
                throw new NotImplementedException();
            }
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
            var byteLength = 4;
            
            int TmpValue = value;
            if (byteLength > 1)
            {
                InnerBuffer[offset] = (byte)TmpValue;
            }

            for (int i = 1; i < byteLength; i++)
            {
                InnerBuffer[offset + i] = (byte)(TmpValue >> (i * 8));
            }
        }
        public void writeInt32BE(double value, int offset = 0, int byteLength = 0, bool noAssert = false)
        {
            ulong TmpValue =(ulong)value;

            if (byteLength > 1)
            {
                InnerBuffer [offset] = (byte)TmpValue;    
            }

            for (int i = 1; i < byteLength; i++)
            {
                InnerBuffer[offset+i] = (byte)(TmpValue >> (i * 8));
            }
           
        }

        public int readInt32LE(int? os = 0, bool? noAssert = false)
        {
            var offset = os.GetValueOrDefault(0);

            var num = (int)this.InnerBuffer[0 + offset] | (int)InnerBuffer[1 + offset] << 8 | (int)InnerBuffer[2 + offset] << 16 | (int)InnerBuffer[3 + offset] << 24;
            return num;
             
        }

       

        public string _toString(string encoding = null, int? start = null, int? end = null)
        {
            var enc = this.GetEncoding(encoding);
            var s = start.GetValueOrDefault(0);
            var e = end.GetValueOrDefault(this.Length);
            
            if (encoding == "hex")
            {
                return BitConverter.ToString(this.InnerBuffer, s, e).Replace("-", String.Empty).ToLowerInvariant();

            }
            if (encoding == "base64")
            {
                return Convert.ToBase64String(this.InnerBuffer, s, e);
            }


            return  enc.GetString(this.InnerBuffer, s, e);
            
        }


        public NodeBuffer2 slice(int? start = null, int? end = null)
        {
            var s = start.GetValueOrDefault(0);
            var e = end.GetValueOrDefault(this.Length);
            if (s == 0)
            {
                return new NodeBuffer2(require: this._require)
                       {
                           InnerBuffer = this.InnerBuffer,
                           Length = e
                       };

            }
            var buff = new byte[e - s];

            Array.Copy(InnerBuffer, s,  buff,0, e - s);

            return new NodeBuffer2(require: this._require)
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

        public void copy(NodeBuffer2 target, int? targetStart = null, int? sourceStart = null, int? sourceEnd = null)
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

        public int get(int pos)
        {
            //may uint?
            return (int) this.InnerBuffer[pos];
        }


    }




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
            return this.Init(param1, "utf-8");

        }

        public NodeBuffer Init(int size)
        {
            this.InnerStream = new MemoryStream(size);
            return this;
        }

        public void writeInt32BE(int value, int offset = 0, bool noAssert = false)
        {
            byte[] buff = new byte[4];
            buff[0] = (byte)value;
            buff[1] = (byte)(value >> 8);
            buff[2] = (byte)(value >> 16);
            buff[3] = (byte)(value >> 24);
            

            if (this.InnerStream.CanSeek)
            {
                var pos = this.InnerStream.Position;
                this.InnerStream.Position = offset;
                this.InnerStream.Write(buff, 0, buff.Length);
                this.InnerStream.Position = pos;
            }
            else
            {
                this.InnerStream.Write(buff, 0, buff.Length);
            }

        }

       

        public int readInt32BE(int offset = 0, bool noAssert = false)
        {
            byte[] buff = new byte[4];
           
           
           
            if (this.InnerStream.CanSeek)
            {
                var pos = this.InnerStream.Position;
                this.InnerStream.Position = offset;
                this.InnerStream.Read(buff, 0, buff.Length);
                this.InnerStream.Position = pos;
            }
            else
            {
                this.InnerStream.Read(buff, 0, buff.Length);
            }

           
            
            var num = (int)buff[0] | (int)buff[1] << 8 | (int)buff[2] << 16 | (int)buff[3] << 24;
            return num;
        }




        public NodeBuffer Init(object text = null, object bla = null)
        {
            return Init2(text as string, bla as string);
        }

        public NodeBuffer Init2(string text, string encodingTxt)
        {

            this.InnerStream = new MemoryStream();

            Encoding enc = System.Text.Encoding.UTF8;
            if (!string.IsNullOrEmpty(encodingTxt))
            {

                enc = Encoding.GetEncodings().Where(x => string.Equals(x.Name, encodingTxt, StringComparison.OrdinalIgnoreCase)).Select(x => x.GetEncoding()).FirstOrDefault()
                      ?? enc;
            }
            //var enc = string.IsNullOrWhiteSpace(encodingTxt) ? System.Text.Encoding.UTF8 : Encoding.GetEncoding(encodingTxt);
            //todo: look up encoding.  
            if (text != null)
            {
                var buf = enc.GetBytes((string)text);
                this.InnerStream.Write(buf, 0, buf.Length);
            }
            return this;




        }


        public NodeBuffer(Require require)
            : base(require)
        {
            //  InnerStream = new M;
            _require = require;
        }



        public static bool isBuffer(object obj)
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

        public void fill(string value, int? offset, int? end)
        {
            if (value != null)
            {
                var buf = System.Text.Encoding.UTF8.GetBytes((string)value);
                this.InnerStream.Write(buf, 0, buf.Length);
            }
        }

        public Stream InnerStream { get; set; }



        public string StupidToString(object enc = null, int? start = null, int? end = null)
        {
            return this.toString(enc, start, end);
        }

        public string toString(object enc = null, int? start = null, int? end = null)
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
                ret = new StreamReader(this.InnerStream, true).ReadToEnd();
            }
            else
            {
                ret = new StreamReader(this.InnerStream, System.Text.Encoding.GetEncoding(encoding)).ReadToEnd();
            }
            if (this.InnerStream.CanSeek)
            {
                this.InnerStream.Position = pos;
            }
            return ret;

        }

        public void copy(NodeBuffer target, int? targetStart = null, int? sourceStart = null, int? sourceEnd = null)
        {
            this.InnerStream.CopyTo(target.InnerStream);

        }

        private long? _length;

        public virtual long length
        {

            get
            {
                if (_length.HasValue)
                {
                    return _length.Value;
                }
                return this.InnerStream.Length;

            }
            set { _length = value; }
        }




    }
}