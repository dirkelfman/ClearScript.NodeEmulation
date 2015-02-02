using System.Configuration;
using System.IO;

using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;
using System;
using Microsoft.ClearScript;

namespace ClearScript.NodeEmulation
{
    public class NodeProcess
    {
        private readonly Require _require;


        public NodeProcess(Require require)
        {
            _require = require;
        }

        public void nextTick(dynamic callback)
        {
              var action = new Action (() =>
                {
                    
                    try
                    {
                        callback.call();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);

                        //throw ex;
                    }
                });

             Task.Factory.StartNew(action);
            
         

         //   t.Wait();
        }

    }
}