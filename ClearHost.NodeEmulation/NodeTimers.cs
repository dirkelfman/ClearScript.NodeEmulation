using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClearHost.NodeEmulation
{
    public class NodeTimers
    {
        public object setTimeout(dynamic  callback, int delay, params object[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
         
           
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

              Task.Factory.StartNew(action, cts.Token);
              return cts;
            
         

         //   t.Wait();
        }



        public void clearTimout(object cancelToken)
        {
            CancellationTokenSource tokenSource = cancelToken as CancellationTokenSource;
            if (tokenSource != null && tokenSource.Token.CanBeCanceled)
            {
                try
                {
                    tokenSource.Cancel();    
                }
                catch
                {
                   
                }
                
            }
        }

       
    }
}
