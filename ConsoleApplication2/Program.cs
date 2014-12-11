using System.Runtime.CompilerServices;
using ClearHost.NodeEmulation;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var c = new V8RuntimeConstraints();
                    //{
                    //    MaxExecutableSize = 1024*1024,
                    //    MaxOldSpaceSize = 1024 * 1024,
                    //    MaxYoungSpaceSize = 1024 * 1024
                    //};
            var engine = new V8ScriptEngine(c,V8ScriptEngineFlags.EnableDebugging, 9222);


        
            
            while (true)
            {

               
                try
                {

                    TryIt(engine);
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
                
                int fg = 0;
            }
        }

        static void TryIt(V8ScriptEngine engine)
        {
            
            var require= new Require(engine,"");
            engine.Evaluate("exports = {};module={exports:exports};");
            var file = new System.IO.FileInfo(Environment.CurrentDirectory + @"..\..\..\js\rateProvider.built.js");
            if (!file.Exists)
            {
                Console.WriteLine("run npm install and grunt");
                Environment.Exit(1);
            }
            engine.Evaluate(file.OpenText().ReadToEnd());
            var rateProviderFactory = engine.Script.module.exports;
            var rateProvider = rateProviderFactory.getRateProvider();
            var cb = new CallBacker();
            var pb = new PropertyBag();
           
            rateProvider.getRatesAsync(pb, cb.Callback );
            cb.T.Wait();
            var joke = pb["joke"];
            Console.WriteLine(joke);
        }
        
          

        
    }
   
}
