using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearHost.NodeEmulation
{
    public class CallBacker
    {
        public Task T { get; set; }
        public Action Callback { get; set; }

        public CallBacker()
        {
            T = new Task(new Action(() => { }));
            Callback = CallbackImp;
        }

        void CallbackImp()
        {
            T.Start();
        }

    }
}
