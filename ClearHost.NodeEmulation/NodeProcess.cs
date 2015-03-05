using System.Configuration;
using System.IO;

using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;
using System;
using Microsoft.ClearScript;
using ClearScript.Manager;

namespace ClearScript.NodeEmulation
{
    public class NodeProcess
    {
        private readonly Require _require;


        public NodeProcess(Require require)
        {
            _require = require;
        }

        static UIntPtr heapSize = new UIntPtr(100000000);

        public void nextTick(dynamic callback)
        {
           // _require.Engine.NextTick(callback);
            _require.Engine.NextTick(() =>
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    //todo pipe to log
                }
                
            });
              //var action = new Action (() =>
              //  {
                    
              //      try
              //      {
              //          callback.call();
              //      }
              //      //catch (ScriptEngineException see)
              //      //{
              //      //    var heapInfo = _require.Engine.GetRuntimeHeapInfo();
              //      //    _require.Engine.CollectGarbage(true);
              //      //    var heapInfo2 = _require.Engine.GetRuntimeHeapInfo();
              //      //    var size = _require.Engine.MaxRuntimeHeapSize;
              //      //    var newSize =size.ToUInt32() + 1024*1000000;
              //      //    _require.Engine.MaxRuntimeHeapSize = new UIntPtr(newSize);
              //      //    this.nextTick(callback);
              //      //}
              //      catch (Exception ex)
              //      {
              //          System.Diagnostics.Debug.WriteLine(ex);

              //          //throw ex;
              //      }
              //  });
              //var tt2 = Task.Delay(1).ContinueWith((Task t) =>
              //{
              //    action();
              //}, TaskContinuationOptions.ExecuteSynchronously);
       
        }

    }
}