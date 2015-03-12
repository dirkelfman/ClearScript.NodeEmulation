using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearScript.NodeEmulation
{
    public class CallBacker
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        public Task T
        {
            get
            {
                return tcs.Task;
            } 
        }
        public CallbackDelegate Callback { get; set; }

        public CallBacker()
        {
           
           
            Callback = CallbackImp;
        }

        public delegate void CallbackDelegate(object a = null, object b = null);
        void CallbackImp(object a=null, object b=null)
        {
            Console.WriteLine(a);
            tcs.SetResult(true);
        }

    }
}
