using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearScript.NodeEmulation
{
    public class CallBacker
    {
        public Task T { get; set; }
        public CallbackDelegate Callback { get; set; }

        public CallBacker()
        {
            T = new Task(new Action(() => { }));
            Callback = CallbackImp;
        }

        public delegate void CallbackDelegate(object a = null, object b = null);
        void CallbackImp(object a=null, object b=null)
        {
            T.Start();
        }

    }
}
