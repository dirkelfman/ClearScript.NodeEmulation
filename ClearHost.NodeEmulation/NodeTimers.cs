using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClearScript.NodeEmulation
{
    public class NodeTimers
    {
        private readonly Require _require;

        public NodeTimers(Require require)
        {
            _require = require;
        }

        public object setTimeout(dynamic  callback, int delay, params object[] args)
        {
            if (delay < 2)
            {
               _require.Engine.NextTick(() =>
               {
                   callback.call(null, args);
               });
                return "flurt";
            }
            CancellationTokenSource cts = new CancellationTokenSource();

            var token = cts.Token;
           
              var action = new Action (() =>
                {

                    try
                    {
                        callback.call(null, args);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    finally 
                    {
                        cts.Dispose();
                    }
                });

              Task.Delay(delay == 0 ? 1 : delay).ContinueWith(x=> action, token);
             
            return token;



            //   t.Wait();
        }



        public void clearTimeout(object cancelToken)
        {
            var  tokenSource = cancelToken as CancellationTokenSource;
            if (tokenSource != null )
            {
                try
                {
                    if (tokenSource.Token.CanBeCanceled)
                    {
                        tokenSource.Cancel();        
                    }
                    
                }
                catch
                {
                   
                }
                
            }
        }

       
    }
}
