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
using System.Net.Http;
using System.Threading;
using ClearHost.NodeEmulation;
using Microsoft.ClearScript;
using ClearScript.Manager;
using ClearScript.Manager.Caching;

namespace ClearScript.NodeEmulation
{
    public  class Require :IDisposable
    {
       // private readonly IRuntimeManager _runtime;
       // private readonly V8ScriptEngine _engine;
        public dynamic BuiltIns { get; set; }

        public Require(IRuntimeManager runtime, V8ScriptEngine engine)
        {
            RuntimeManager = runtime;
            Engine = engine;

            //_engine.AddHostType("ccnetBuffer", typeof(NodeBuffer));
            //_engine.AddHostType("ccnetHttpRequest", typeof(NodeHttpRequest));
            //_engine.AddHostType("ccnetProcess", typeof(NodeProcess));
            //_engine.AddHostType("ccnetTimers", typeof(NodeTimers));
            //_engine.AddHostType("ccNetEventEmitter", typeof(NodeEventEmitter));

            Engine.AddHostObject("ccnetHelpers", typeof(Helpers));
            Engine.AddHostObject("util", new NodeUtil(engine));
            Engine.AddHostObject("console", new CheapConsole());


            LoadBuiltInModules();

            //HttpClient client = HttpClientFactory.Create(new Handler1(), new Handler2(), new Handler3())
           
            
        }

        public V8ScriptEngine Engine { get; private set; }
        public IRuntimeManager RuntimeManager { get; private set; }


        public object GetService(string  serviceType)
        {
            object obj = null;
           
            if (ServiceLocator != null)
            {
                obj = ServiceLocator(serviceType);
            }
            if (obj != null)
            {
                return obj;
            }
            switch (serviceType)
            {
                case "ccnetBuffer":
                {
                    return new NodeBuffer(this);
                }
                case "ccnetHttpRequest":
                {
                    return new NodeHttpRequest(this);
                }
                case "ccnetProcess":
                {
                    return new NodeProcess(this);
                }
                case "ccnetTimers":
                {
                    return new NodeTimers(this);
                }
                case "System.Net.Http.HttpClient":
                {
                    if (RequestHandlerFactory != null)
                    {
                        return HttpClientFactory.Create(RequestHandlerFactory());
                    }
                    return HttpClientFactory.Create( );
                }
            }
            return null;
        }

        public Func<DelegatingHandler[]> RequestHandlerFactory { get; set; }



        public Func<string, object> ServiceLocator { get; set; }



        public void Dispose()
        {
            Engine.Evaluate("delete module;delete modules;delete require;delete Buffer;delete process;delete setTimeout;delete builtinModules;");
        }

 

        void LoadBuiltInModules()
        {
            var key = "_builtIn";
            CachedV8Script cachedv8Script;
            V8Script v8Script = null;
            if (!this.RuntimeManager.TryGetCached(key, out cachedv8Script))
            {
                using (var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Assembly.GetManifestResourceNames().First(x => x.Contains("modules.js"))))
                {
                    var js = new StreamReader(stream).ReadToEnd();

                    v8Script = this.RuntimeManager.Compile(key, js);


                }
            }
            else
            {
                v8Script = cachedv8Script.Script;
            }




            BuiltIns = Engine.Evaluate(v8Script);
            BuiltIns.container.engine = Engine;
            BuiltIns.container.runtime = RuntimeManager;
            BuiltIns.container.require = this;
        }

        public int threadId = Thread.CurrentThread.ManagedThreadId;
        public SingleThreadSynchronizationContext Context = new SingleThreadSynchronizationContext();


        public sealed class SingleThreadSynchronizationContext :
            SynchronizationContext
        {
            private readonly
                BlockingCollection<KeyValuePair<SendOrPostCallback, object>>
                m_queue =
                    new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

            public override void Post(SendOrPostCallback d, object state)
            {
                m_queue.Add(
                    new KeyValuePair<SendOrPostCallback, object>(d, state));
            }

            public void RunOnCurrentThread()
            {
                KeyValuePair<SendOrPostCallback, object> workItem;
                while (m_queue.TryTake(out workItem, Timeout.Infinite))
                    workItem.Key(workItem.Value);
            }

            public void Complete()
            {
                m_queue.CompleteAdding();
            }

        
        }



        public dynamic LoadModuleByPath(string filePath)
        {
            var key = filePath + File.GetLastWriteTime(filePath);
            CachedV8Script cachedv8Script;
            V8Script v8Script = null;
            if (!RuntimeManager.TryGetCached(key, out cachedv8Script))
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

                v8Script = RuntimeManager.Compile(key, fnTxt);
               
            }
            else
            {
                v8Script = cachedv8Script.Script;
            }
            return ((dynamic)Engine.Evaluate(v8Script)).exports; 
            //var code = _runtime.Compile(fnTxt);
         //   return ((dynamic) _engine.Evaluate(code)).exports;

        }


    }
}
