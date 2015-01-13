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
            //var managerPool = new ManagerPool(new ManagerSettings());
            var settings = new ManagerSettings()
                           {

                           };
            //ManagerPool.InitializeCurrentPool(new ManualManagerSettings()
            //                                  {
            //                                      V8DebugEnabled = true,
            //                                      V8DebugPort = 5858
            //                                  });
            ManagerPool.InitializeCurrentPool(new ManagerSettings()
                                              {

                                              });

            for (int i = 0; i < 1; i++)
            {
                Task t = new Task(()=> main2());
                t.Start();
            }
            Console.ReadLine();
            //Console.WriteLine("gcing");

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
                    V8ScriptEngine engine = runtime.GetEngine(); ;





                    if (engine.MaxRuntimeHeapSize !=  heapSize)
                    {
                        engine.MaxRuntimeHeapSize = heapSize;
                    }
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

                        TryIt(runtime, engine);
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
            require.BuiltIns.process.env.NODE_DEBUG = "request";
            var file = new System.IO.FileInfo(Environment.CurrentDirectory + @"..\..\..\js\rateProvider.built.js");
            if (!file.Exists)
            {
                Console.WriteLine("run npm install and grunt");
                Environment.Exit(1);
            }




            var module = require.LoadModuleByPath(file.FullName);

           // var rateProvider = rateProviderFactory.getRateProvider();


            var cb = new CallBacker();
            var pb = new PropertyBag();


          

            var bing = engine.Script.Object.create(module.RateProvider.prototype);

            
            bing.getRatesAsync(pb, cb.Callback);
          //  provider.getRatesAsync(pb, cb.Callback);
            cb.T.Wait();
            var joke = pb["joke"];
            if (Sstopwatch == null)
            {
                Sstopwatch = new Stopwatch();
                Sstopwatch.Start();
            }

            cnt ++;
            var totsSecs = (cnt*1000 )/(Sstopwatch.ElapsedMilliseconds) ;
            Console.WriteLine("{0} a sec" , totsSecs);
        }

        private static Stopwatch Sstopwatch = null;



    }
   
}
