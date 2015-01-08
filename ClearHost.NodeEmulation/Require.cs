using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Microsoft.ClearScript.V8;
using System;
using Microsoft.ClearScript;
using ClearScript.Manager;
using ClearScript.Manager.Caching;

namespace ClearHost.NodeEmulation
{
    public  class Require
    {
        private readonly IRuntimeManager _runtime;
        private readonly V8ScriptEngine _engine;
        public dynamic BuiltIns { get; set; }

        public Require(IRuntimeManager runtime, V8ScriptEngine engine)
        {
            _runtime = runtime;
            _engine = engine;
           
            engine.AddHostType("ccnetBuffer", typeof(NodeBuffer));
            engine.AddHostType("ccnetHttpRequest", typeof(NodeHttpRequest));
            _engine.AddHostType("ccnetProcess", typeof(NodeProcess));
            _engine.AddHostType("ccnetTimers", typeof(NodeTimers));
             
            engine.AddHostType("ccNetEventEmitter", typeof(NodeEventEmitter));
            engine.AddHostObject("util", new NodeUtil(engine));
            _engine.AddHostObject("console", new CheapConsole());


            LoadBuiltInModules();


           
            
        }

        void LoadBuiltInModules()
        {
            var key = "_builtIn";
            CachedV8Script cachedv8Script;
            V8Script v8Script = null;
            if (!_runtime.TryGetCached(key, out cachedv8Script))
            {
                using (var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Assembly.GetManifestResourceNames().First(x => x.Contains("modules.js"))))
                {
                    var js = new StreamReader(stream).ReadToEnd();

                    v8Script = _runtime.Compile(key, js);


                }
            }
            else
            {
                v8Script = cachedv8Script.Script;
            }




            BuiltIns = _engine.Evaluate(v8Script);
            BuiltIns.container.engine = _engine;
            BuiltIns.container.runtime = _engine;
            BuiltIns.container.require = this;
        }

        //public dynamic LoadModule(string src)
        //{
            
        //    var sb = new StringBuilder("(function(){ module={}; exports={};");

        //    sb.Append(src);
        //    sb.Append(";return module;})();");
        //    var code  = _runtime.Compile(src);
        //    return ((dynamic)_engine.Evaluate(code)).exports;
        //}

        public dynamic LoadModuleByPath(string filePath)
        {
            var key = filePath + File.GetLastWriteTime(filePath);
            CachedV8Script cachedv8Script;
            V8Script v8Script = null;
            if (!_runtime.TryGetCached(key, out cachedv8Script))
            {
                var ms = new MemoryStream();
                var text = System.Text.Encoding.UTF8.GetBytes("(function(){ module={}; exports={};");
                ms.Write(text, 0, text.Length);
                var file = new FileInfo(filePath);
                using (var fs = file.OpenRead())
                {
                    fs.CopyTo(ms);
                }
                text = System.Text.Encoding.UTF8.GetBytes(";return module})()");
                ms.Write(text, 0, text.Length);
                var fnTxt = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                v8Script = _runtime.Compile(key, fnTxt);
            }
            else
            {
                v8Script = cachedv8Script.Script;
            }
            return ((dynamic)_engine.Evaluate(v8Script)).exports; 
            //var code = _runtime.Compile(fnTxt);
         //   return ((dynamic) _engine.Evaluate(code)).exports;

        }


    }
}
