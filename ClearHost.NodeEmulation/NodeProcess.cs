﻿using System.Configuration;
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
       

        public NodeProcess()
        {
         
          
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

    }
}