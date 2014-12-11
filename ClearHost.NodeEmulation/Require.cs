using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using System.Dynamic;
using Microsoft.ClearScript.V8;
using System;
using Microsoft.ClearScript;

namespace ClearHost.NodeEmulation
{
    public  class Require
    {
        private readonly V8ScriptEngine _engine;
        private readonly string _path;

        public Require(V8ScriptEngine engine, string path)
        {
            
            _engine = engine;
            _path = path;
            Init();

            engine.Execute("Buffer=require('buffer').Buffer;");
        }

        private void Init()
        {
            
            _engine.AddHostObject("_require", new Func<string, object>(x => this.RequireScript(x)));
            _engine.AddHostObject("_clearScriptConstruct", new Func<string, object>(x => this.Create(x)));
            _engine.AddHostObject("console",new CheapConsole());
            _engine.Execute("function require(x){return _require(x);}");
            _engine.AddHostObject("process", new NodeProcess(_engine));
        }

        public object Create(string type, params object[] config)
        {
            return CreateRequreModule( type);
        }

        private Dictionary<string, object> _cache = new Dictionary<string, object>(); 
        public object RequireScript(string script)
        {
            object dep;
            if (_cache.TryGetValue(script, out dep))
            {
                return dep;
            }
            dep = CreateRequreModule(script);

            _cache[script] = dep;
            return dep;
        }

        public object CreateRequreModule(string script)
        {
            switch (script)
            {
                case "cc-http":
                {
                    return new NodeHttp();
                    ;
                }
                case "cc-http-agent":
                {
                    return new NodeEvents(_engine);
                }
                case "fs":
                {
                    var fn = @"x=function(){
   
    return { readFileSync: function() { return '';}};
}();
";
                    return _engine.Evaluate(fn);
                }
                case "https":
                case "http":
                {
                    var fn = @"x=function(){
    function Agent(){ return _clearScriptConstruct('cc-http-agent');};
    function request(options,callback){ return _clearScriptConstruct('cc-http').request(options,callback);};
    
    return { Agent:Agent, request:request };
}();
";
                    return _engine.Evaluate(fn);
                   
                }
                case "util":
                {
                    return new NodeUtil(_engine);
                }
                case "cc-buffer":
                {
                    return new NodeBuffer();
                }
                case "buffer":
                {
                    
                    var fn = @"x=function(){
    function Buffer(){ return _clearScriptConstruct('cc-buffer');};
    Buffer.isBuffer= function (buff){ return buff && buff.isBuffer ;};
    Buffer.isEncoding=function(enc){ return true;};
Buffer.concat=function(list,len){ return list.length ? list[0]: new Buffer();};
    return { Buffer:Buffer  };
}();
";
                    return _engine.Evaluate(fn);
                    
                }
                   
                case "_process":
                case "process":
                {
                    return new NodeProcess(_engine);
                }
//                case "cc-events":
//                {
//                    return new NodeEvents(_engine);
//                }
//                case "events":
//                {
                   
//                    var fn = @"x=function(){
//    function EventEmitter(){ return _clearScriptConstruct('cc-events');};
//    
//    return { EventEmitter:EventEmitter };
//}();
//";
                    
                   
//                    return _engine.Evaluate(fn);
                    
                
                    

//                }
            }
            System.Diagnostics.Debug.WriteLine(script);
            return new PropertyBag();
            
            var scriptFile = new FileInfo(_path + script);
            object module = null;
            

            _engine.Evaluate(scriptFile.FullName, false, "module={};\r\n" + scriptFile.OpenText().ReadToEnd());
            module = _engine.Script.module.exports;

            return module;
        }
    }

    public class CheapConsole
    {
        public void log(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(stuff);
        }
    }
}
