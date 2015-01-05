using System.Web.Hosting;
using ClearHost.NodeEmulation;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Web.Http;
using System.Web.Http.Filters;

namespace MvcApplication1
{

    public class FooAttribute : Attribute, IActionFilter
    {
        private V8Runtime runtime;
        private V8ScriptEngine engine;
        public FooAttribute()
        {
            var c = new V8RuntimeConstraints();

            runtime = new V8Runtime("fred", V8RuntimeFlags.EnableDebugging, 5858);
            engine = runtime.CreateScriptEngine();
           

        }

        async System.Threading.Tasks.Task<HttpResponseMessage>  IActionFilter.ExecuteActionFilterAsync(System.Web.Http.Controllers.HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken, Func<System.Threading.Tasks.Task<HttpResponseMessage>> continuation)
        {
            var require = new Require(runtime, engine);
            var file = new System.IO.FileInfo( HostingEnvironment.ApplicationPhysicalPath +   @"\js\actionFilters.built.js");
          
            var modlue = require.LoadModuleByPath(file.FullName);

            //var proto = modlue.ActionFilters.prototype;
            var filters = modlue;
              
         
            var res= await continuation();
            var content = res.Content as ObjectContent;
            var strings = content.Value as IEnumerable<string>;
            if (strings != null)
            {
                List<string> newStrings  = new List<string>(strings);
                filters.afterGetValues(actionContext, newStrings);
                //newStrings.Add("asdf");
                content.Value = newStrings;
            }
           
            return res;

        }

        bool IFilter.AllowMultiple
        {
            get { return false; }
        }
    }


    public class ValuesController : ApiController
    {
        [FooAttribute]
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}