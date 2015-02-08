using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;

namespace ClearScript.NodeEmulation
{
    public class NodeEventEmitter
    {
        Require _require;
        public bool isCCnetEventEmitter = true;
        public bool isCCnet = true;

     
        

        public NodeEventEmitter(Require require)
        {
            _require = require;
            _require.OnReset += _require_OnReset;
        }

        void _require_OnReset(object sender, EventArgs e)
        {
            _listeners.Clear();
            this._require = null;

        }

        private Dictionary<string, List<Tuple<dynamic,bool>>> _listeners = new Dictionary<string, List<Tuple<dynamic, bool>>>();

        public NodeEventEmitter addListener(string eventName, DynamicObject listener)
        {
            List<Tuple<dynamic,bool>>events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] = new List<Tuple<dynamic, bool>>();
            }

            events.Add( new Tuple<dynamic, bool>(listener, false));
            return this;
        }

        public bool hasEvent(string eventName)
        {
            return this._listeners.ContainsKey(eventName);
        }

        public NodeEventEmitter on(string eventName, DynamicObject listener)
        {
            return this.addListener(eventName, listener);

        }



        public bool emit(string eventName, params object[] args)
        {
             List<Tuple<dynamic,bool>>events;
            if (_listeners.TryGetValue(eventName, out events))
            {
                events.ForEach(listener =>
                {
                    if (args == null)
                    {
                        listener.Item1.call(null);
                    }
                    else if (args.Length == 1)
                    {
                        listener.Item1.call(null, args[0]);
                    }
                    else if (args.Length == 2)
                    {
                        listener.Item1.call(null, args[0], args[1]);
                    }
                    else if (args.Length == 3)
                    {
                        listener.Item1.call(null, args[0], args[1], args[2]);
                    }
                    else if (args.Length == 4)
                    {
                        listener.Item1.call(null, args[0], args[1], args[2], args[3]);
                    }
                    else if (args.Length == 5)
                    {
                        listener.Item1.call(null, args[0], args[1], args[2], args[3], args[4]);
                    }
                    else if ( args.Length> 5)
                    {
                        throw new NotImplementedException();
                    }
                    
                    //if (args.Length > 0)
                    //{
                    //    listener.Item1.call(null, args[0]);
                    //}
                    //else
                    //{
                    //    listener.Item1.apply(null, args);
                    //}
                });
                events.Where(x=>x.Item2).ToList().ForEach(x=> events.Remove(x));
           
                return true;
            }
          
            return false;

        }


    }
}