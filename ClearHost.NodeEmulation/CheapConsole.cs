using System.Linq;
using Microsoft.ClearScript;

namespace ClearScript.NodeEmulation
{
    public class CheapConsole : INodeConsole
    {
        private Require _require;

        public CheapConsole(Require require)
        {
            _require = require;
        }

        object[] convert(object[] stuff)
        {
            if (stuff == null || stuff.Length ==0 )
            {
                return stuff;
            }
            return stuff.Select(x =>
            {
                try
                {
                    return Newtonsoft.Json.Linq.JContainer.FromObject(x);
                }
                catch
                {
                    return x;
                }
            }).ToArray();

        }

       


    INodeConsole InnerConsole
        {
            get { return _require.Console; }
        }


        [ScriptMemberAttribute("log")]
        public void Log(params object[] stuff)
        {
            if ( InnerConsole != null)
            {
                InnerConsole.Log(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));
        }

        [ScriptMemberAttribute("info")]
        public void Info(params object[] stuff)
        {
            if (InnerConsole != null)
            {
                InnerConsole.Info(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }

        [ScriptMemberAttribute("warn")]
        public void Warn(params object[] stuff)
        {
            if (InnerConsole != null)
            {
                InnerConsole.Warn(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        [ScriptMemberAttribute("error")]
        public void Error(params object[] stuff)
        {
            if (InnerConsole != null)
            {
                InnerConsole.Error(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        [ScriptMemberAttribute("time")]
        public void Time(params object[] stuff)
        {
            if (InnerConsole != null)
            {
                InnerConsole.Time(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        [ScriptMemberAttribute("timeEn")]
        public void TimeEn(params object[] stuff)
        {
            if (InnerConsole != null)
            {
                InnerConsole.TimeEn(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        [ScriptMemberAttribute("trace")]
        public void Trace(params object[] stuff)
        {
            if (InnerConsole != null)
            {
                InnerConsole.Trace(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        [ScriptMemberAttribute("assert")]
        public void Assert(params object[] stuff)
        {
            if (InnerConsole != null)
            {
                InnerConsole.Assert(stuff);
                return;
            }
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }

    }
}