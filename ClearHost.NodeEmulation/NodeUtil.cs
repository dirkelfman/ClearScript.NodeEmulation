using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Microsoft.ClearScript.V8;

namespace ClearScript.NodeEmulation
{
    public class NodeUtil
    {
        private readonly V8ScriptEngine _engine;

        public NodeUtil(Microsoft.ClearScript.V8.V8ScriptEngine engine)
        {
            _engine = engine;
        }

        public string format(string format, params Object[] stuff)
        {
            return string.Format(format, stuff);
        }

        public void debug(object str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        public void error(params Object[] stuff)
        {
             System.Diagnostics.Debug.WriteLine(stuff);
        }
         public void puts(params Object[] stuff)
        {
             System.Diagnostics.Debug.WriteLine(stuff);
        }
         public void log(params Object[] stuff)
        {
             System.Diagnostics.Debug.WriteLine(stuff);
        }

        public void inspect(object obj, DynamicObject config = null)
        {
            try
            {
                var str = (string) _engine.Script.JSON.stringify(obj);
                System.Diagnostics.Debug.WriteLine(str);
            }
            catch
            {
            }
        }


        

        public bool isArray(object obj)
        {
            var isArrayFn = (dynamic)_engine.Script.Array.isArray;
            return (bool) isArrayFn(obj);

        }

         public bool isRegExp(object obj)
        {
             throw new NotImplementedException();

        }
        public bool isError(object obj)
        {
             throw new NotImplementedException();

        }
        public void pump(DynamicObject readableStream, DynamicObject writabaleStream, DynamicObject callback = null)
        {
             throw new NotImplementedException();

        }
        public bool inherits(object obj)
        {
             throw new NotImplementedException();

        }

    }
}