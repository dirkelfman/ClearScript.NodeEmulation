using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ClearHost.NodeEmulation
{
    public static class DynamicObjectExtensions
    {
        public static bool HasField(this DynamicObject obj, string field)
        {
            return !(obj.AsDynamic().field is Microsoft.ClearScript.Undefined);
        }

        public static bool TryGetField(this DynamicObject obj, string field, out object outField)
        {
           
            outField = obj.AsDynamic().field;
            return !(outField is Microsoft.ClearScript.Undefined);

        }

        public static dynamic AsDynamic(this DynamicObject obj)
        {
            return (dynamic)obj ;
        }
        public static IEnumerable<KeyValuePair<string, object>> GetProperties(this DynamicObject obj  )
        {


            var d = obj.AsDynamic();
            if (obj == null)
            {
                yield break;
            }
            foreach (var varName in obj.GetDynamicMemberNames())
            {
                object prop;
                if (obj.TryGetField(varName, out prop))
                {
                    yield return new KeyValuePair<string, object>(varName,prop);
                }

            }
            
        }

        class MyGetMemberBinder : GetMemberBinder
        {
            public MyGetMemberBinder(string name)
                : base(name, false)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                return null;
            }
        }
        class MySetMemberBinder : SetMemberBinder
        {
            public MySetMemberBinder(string name) : base(name, false)
            {

            }

            public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
            {
                return null;
            }
        }

        public static T GetField<T>(this DynamicObject obj , string field, T defaultValue = default(T))
        {
            Object outField;

            if (obj.TryGetMember(new MyGetMemberBinder(field), out outField))
            {
                if (outField is Microsoft.ClearScript.Undefined)
                {
                    return defaultValue;
                }
                return (T) outField;
            }
           
            return defaultValue;
        }

        public static bool TrySetField(this DynamicObject obj, string field, object value)
        {
            return obj.TrySetMember(new MySetMemberBinder(field), value);
        }
        public static T GetField<T>(this DynamicObject obj, string field, Func<object,T> converter, T defaultValue = default(T))
        {
            //object  outField = obj.field;

            //if (outField is Microsoft.ClearScript.Undefined)
            //{
            //    return defaultValue;
            //}

            //return converter(outField);


            Object outField;

            if (obj.TryGetMember(new MyGetMemberBinder(field), out outField))
            {
                if (outField is Microsoft.ClearScript.Undefined)
                {
                    return defaultValue;
                }
                return converter(outField);
            }
            
            return defaultValue;
        }

        //public static Func<object, PropertyBag> PropertyBagConverter = new Func<object, PropertyBag>((x)=>
        //                                                               {
        //                                                                   if (x == null)
        //                                                                   {
        //                                                                       return new PropertyBag();
        //                                                                   }
        //                                                                   var dx = (dynamic) x;
                                                                          
        //                                                               });
        
            
        
    }
}