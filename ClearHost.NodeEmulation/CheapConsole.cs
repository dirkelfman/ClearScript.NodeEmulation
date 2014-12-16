using System.Linq;
using WebGrease.Css.Extensions;

namespace ClearHost.NodeEmulation
{
    public class CheapConsole
    {
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
        public void log(params object[] stuff)
        {

            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));
        }

        public void info(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        public void warn(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        public void error(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        public void time(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        public void timeEn(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        public void trace(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }
        public void assert(params object[] stuff)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(", ", convert(stuff)));

        }

    }
}