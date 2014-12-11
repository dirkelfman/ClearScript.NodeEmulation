using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;

namespace ClearHost.NodeEmulation
{
    public class NodeEvents
    {
        public class NodeEventRequireModule
        {
            private Microsoft.ClearScript.V8.V8ScriptEngine _engine;
            private dynamic _eventEmitter;

            public NodeEventRequireModule(Microsoft.ClearScript.V8.V8ScriptEngine engine)
            {
                _engine = engine;
            }

            public dynamic EventEmitter
            {
                get
                {
                    if (_eventEmitter == null)
                    {
                        _eventEmitter = (dynamic)_engine.Evaluate("_x=function EventEmitter(){ return require('events');}; ");
                    }
                    return _eventEmitter;
                }
            }
        }

        private Microsoft.ClearScript.V8.V8ScriptEngine _engine;
        public NodeEvents(Microsoft.ClearScript.V8.V8ScriptEngine engine)
        {
            _engine = engine;
        }

        public NodeEvents()
        {
            int f = 0;
        }
        private Dictionary<string, List<Tuple<dynamic,bool>>> _listeners = new Dictionary<string, List<Tuple<dynamic, bool>>>();

        public void addListener(string eventName, DynamicObject listener)
        {
            List<Tuple<dynamic,bool>>events;
            if (!_listeners.TryGetValue(eventName, out events))
            {
                events = _listeners[eventName] = new List<Tuple<dynamic, bool>>();
            }

            events.Add( new Tuple<dynamic, bool>(listener, false));
        }

        public void on(string eventName, DynamicObject listener)
        {
            this.addListener(eventName, listener);
        }



        public bool emit(string eventName, params object[] args)
        {
             List<Tuple<dynamic,bool>>events;
            if (_listeners.TryGetValue(eventName, out events))
            {
                events.ForEach(listener =>
                {
                    listener.Item1.apply(null, args);
                });
                events.Where(x=>x.Item2).ToList().ForEach(x=> events.Remove(x));
           
                return true;
            }
          
            return false;

        }

//emitter.once(event, listener)
//emitter.removeListener(event, listener)
//emitter.removeAllListeners([event])
//emitter.setMaxListeners(n)
//emitter.listeners(event)
//emitter.emit(event, [arg1], [arg2], [...])
//Class Method: EventEmitter.listenerCount(emitter, event)
//Event: 'newListener'
//Event: 'removeListener'
    }
}