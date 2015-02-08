using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ClearScript.NodeEmulation;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClearScript.Manager;
using System.Net.Http;
using System.Threading;
using System.Net;

namespace ConsoleApplication2
{
    class Program
    {
        public class Foo

        {

            public class FooFactory


            {
                public Foo MakeIt
                    ()
                {
                    return new Foo();
                }
            }

            public string go()
            {
                return "hey";
            }
        }

        static object require(string thing)
        {
            return new Foo();
        }

        private static void Main(string[] args)
        {

            var hostname = "food.com";
            var port = 80;
            var path = "/abc/deasdk";

            int qpos = path.IndexOf('?');


            var pathParts = (path ?? "").Split(new char[] { '?' }, 2);
            var uriBuilder = new UriBuilder(
               "http",
              hostname,
              port,
              qpos>-1? path.Substring(0,qpos ) : path,
              qpos>-1? path.Substring (qpos) :null).Uri;




            //var managerPool = new ManagerPool(new ManagerSettings());
            var settings = new ManagerSettings()
                           {

                           };
            ManagerPool.InitializeCurrentPool(new ManualManagerSettings()
                                              {
                                                  V8DebugEnabled = true,
                                                  V8DebugPort = 5858,
                                                  MaxExecutableBytes =0,
                                                  MaxNewSpaceBytes = 0,
                                                  MaxOldSpaceBytes =0,
                                                  RuntimeMaxCount = 10
                                                  
                                              });
            //ManagerPool.InitializeCurrentPool(new ManagerSettings()
            //                                  {

            //                                  });

            for (int i = 0; i < 1; i++)
            {
                Task t = new Task(() => main2());
                t.Start();
            }
            //main2();
            Console.ReadLine();
            Console.WriteLine("gcing");
         //   main2();
        }

        private static int cnt = 0;
        static void main2()
        {
        //var engine = runtime.CreateScriptEngine();

            var heapSize = new UIntPtr(90000000);
            
            while (true)
            {
                using (var scope = new ManagerScope())
                {
                    var runtime = scope.RuntimeManager;

                    //V8Runtime v8Runtime = (V8Runtime)runtime.GetType().InvokeMember("_v8Runtime", BindingFlags.GetField | BindingFlags.NonPublic| BindingFlags.Instance, null, runtime,null);
                    //V8Runtime v8Runtime = new V8Runtime();
                    //runtime.GetType().InvokeMember("_v8Runtime", BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance, null, runtime, new object[] { v8Runtime });
                    //V8ScriptEngine engine = v8Runtime.CreateScriptEngine("steve", V8ScriptEngineFlags.DisableGlobalMembers| V8ScriptEngineFlags.None ,5858);

                    V8ScriptEngine engine = runtime.GetEngine();
                   

                     

                    //if (engine.MaxRuntimeHeapSize != heapSize)
                    //{
                    //    engine.MaxRuntimeHeapSize = heapSize;
                    //}
                    //var heapinfo = engine.GetRuntimeHeapInfo();

                
                    //if (heapinfo.TotalHeapSize > 100000000)
                    //{
                    //    Console.WriteLine("{0} {1} {2}", heapinfo.TotalHeapSize , heapinfo.TotalHeapSizeExecutable, heapinfo.UsedHeapSize);
                    //    engine.CollectGarbage(false);
                    //    heapinfo = engine.GetRuntimeHeapInfo();
                    //    Console.WriteLine("{0} {1} {2}", heapinfo.TotalHeapSize, heapinfo.TotalHeapSizeExecutable, heapinfo.UsedHeapSize);
                    //    engine.MaxRuntimeHeapSize = new UIntPtr(90000000);
                    //}
                    try
                    {
                     //   engine.WriteHeapSnapshot("c:\\temp\\foo" + cnt + ".heapsnapshot");
                       // while (true)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                TryIt(runtime, engine);
                            }
                           
                        }
                      
                        engine.Dispose();
                        
                    }
                    catch (ScriptEngineException see)
                    {
                        System.Diagnostics.Debug.WriteLine(see.Message);
                        System.Diagnostics.Debug.WriteLine(see.ErrorDetails);
                        int f = 0;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        int f = 0;
                    }
                   
                }
                
                int fg = 0;
            }
        }

        static void TryIt(IRuntimeManager runtime, V8ScriptEngine engine)
        {

            
            var require = new Require(runtime, engine);
            require.RequestHandlerFactory = ()=>new DelegatingHandler[]{ new MyDelegatingHandler()}; 
         //   require.BuiltIns.process.env.NODE_DEBUG = "request";

            var file = new System.IO.FileInfo(Environment.CurrentDirectory + @"..\..\..\js\rateProvider.built.js");
            if (!file.Exists)
            {
                Console.WriteLine("run npm install and grunt");
                Environment.Exit(1);
            }




            var module = require.LoadModuleByPath(file.FullName);

           // var rateProvider = rateProviderFactory.getRateProvider();


            for (int i = 0; i < 10000; i++)
            {
                var cb = new CallBacker();
                var pb = new PropertyBag();




                var bing = module;
                if (Sstopwatch == null)
                {
                    Sstopwatch = new Stopwatch();
                    Sstopwatch.Start();
                }

                //System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
                //{
                //    bing.getRatesAsync(pb, cb.Callback);
                //});
                bing.getRatesAsync(pb, cb.Callback);
                //  provider.getRatesAsync(pb, cb.Callback);
                cb.T.ConfigureAwait(false);
                cb.T.Wait();
                var joke = pb["joke"];
              
                System.Diagnostics.Debug.WriteLine(joke);
                cnt ++;
                if (cnt%10000==0)
                {
                    engine.WriteHeapSnapshot("c:\\temp\\foo"+cnt+".heapsnapshot");
                }
                var totsSecs = Sstopwatch.ElapsedMilliseconds == 0 ? 1 : (cnt*1000)/(Sstopwatch.ElapsedMilliseconds);
                Console.WriteLine("{0} a sec", totsSecs);
                require.Reset();



                bing = null;
                cb = null;
                joke = null;
                module = null;
                require = null;
                require = null;


                engine.CollectGarbage(true);
                GC.Collect(0, GCCollectionMode.Forced);
                GC.Collect(2, GCCollectionMode.Forced);
                engine.CollectGarbage(true);
                System.Threading.Thread.Sleep(1005 * 60 * 3);
                System.Threading.Thread.Sleep(5000);
                GC.Collect(0, GCCollectionMode.Forced);
                GC.Collect(2, GCCollectionMode.Forced);
                engine.CollectGarbage(true);
                engine.WriteHeapSnapshot("c:\\temp\\fart.heapsnapshot");
                
            }


        }

        private static Stopwatch Sstopwatch = null;



    }

        public class MyDelegatingHandler : DelegatingHandler
        {
            private HttpMessageInvoker _server;

            public void SetHandler(HttpMessageHandler handler)
            {
                _server = new HttpMessageInvoker(handler);
            }
            protected async  override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            {

                try
                {
                   
                    HttpResponseMessage resp = null;
                    if (base.InnerHandler != null)
                    {
                        resp = await base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        resp = request.CreateResponse();
                    }
                    //if (request.RequestUri.PathAndQuery.IndexOf("api/commerce/catalog/admin/product") > -1)
                    //{
                    //    resp = request.CreateResponse();
                    //    resp.Content = new StringContent("{ \"content\":{ \"productName\":\"steve\"}}");

                    //}
                    return resp;
                }
                catch(Exception)
                {
                    throw;
                }

                Console.WriteLine(request.RequestUri.ToString());

                
            }
        }
   
}
