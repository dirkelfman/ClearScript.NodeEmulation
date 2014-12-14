using System.Configuration;
using System.IO;

using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;
using System;
using Microsoft.ClearScript;

namespace ClearHost.NodeEmulation
{
    public class NodeProcess
    {
        private readonly V8ScriptEngine _engine;

        public NodeProcess(Microsoft.ClearScript.V8.V8ScriptEngine engine)
        {
            _engine = engine;
            this.env = new PropertyBag();
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
                    }
                });

             Task.Factory.StartNew(action);
            
         

         //   t.Wait();
        }

        private PropertyBag _env;
        public PropertyBag env
        {
            get { return this._env; }
            set { _env = value; }
        }
    }
}