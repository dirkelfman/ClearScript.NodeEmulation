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

namespace ClearHost.NodeEmulation
{
    public  class Require
    {
        private readonly V8Runtime _runtime;
        private readonly V8ScriptEngine _engine;
        public dynamic BuiltIns { get; set; }

        public Require(V8Runtime runtime , V8ScriptEngine engine)
        {
            _runtime = runtime;
            _engine = engine;

            engine.AddHostType("ccnetBuffer", typeof(NodeBuffer));
            engine.AddHostType("ccnetHttpRequest", typeof(NodeHttpRequest));
            _engine.AddHostType("ccnetProcess", typeof(NodeProcess));
            _engine.AddHostType("ccnetTimers", typeof(NodeProcess));
             
            engine.AddHostType("ccNetEventEmitter", typeof(NodeEventEmitter));
            engine.AddHostObject("util", new NodeUtil(engine));
            _engine.AddHostObject("console", new CheapConsole());
            
            
            using (var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Assembly.GetManifestResourceNames().First(x => x.Contains("modules.js"))))
            {
                var js = new StreamReader(stream).ReadToEnd();

                BuiltIns = _engine.Evaluate(js);
                BuiltIns.container.engine = engine;
                BuiltIns.container.runtime = runtime;
                BuiltIns.container.require = this;
            }
            engine.Execute("Buffer=require('buffer').Buffer;");
            engine.Execute("process=require('process');");
            engine.Execute("setTimeout=require('timers').setTimeout;");
            
        }

        public dynamic LoadModule(string src)
        {
            
            var sb = new StringBuilder("(function(){ module={}; exports={};");

            sb.Append(src);
            sb.Append(";return module;})();");
            var code  = _runtime.Compile(src);
            return ((dynamic)_engine.Evaluate(code)).exports;
        }

        public dynamic LoadModuleByPath(string filePath)
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

            var code = _runtime.Compile(fnTxt);
            return ((dynamic) _engine.Evaluate(code)).exports;

        }

     
    }
}
